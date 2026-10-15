import asyncio
import os
import tempfile
import threading
from contextlib import asynccontextmanager
from pathlib import Path
from typing import Annotated

from fastapi import FastAPI, File, Form, UploadFile
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
from faster_whisper import WhisperModel

MAX_AUDIO_BYTES = 13 * 1024 * 1024
MODEL_NAME = os.environ.get("WHISPER_MODEL", "base.en")
_model: WhisperModel | None = None
_inference_lock = threading.Lock()


@asynccontextmanager
async def lifespan(_app: FastAPI):
    global _model
    _model = WhisperModel(MODEL_NAME, device="cpu", compute_type="int8")
    yield
    _model = None


app = FastAPI(title="Local Whisper ASR", lifespan=lifespan)
app.add_middleware(
    CORSMiddleware,
    allow_origin_regex=r"^https?://(localhost|127\.0\.0\.1)(:\d+)?$",
    allow_methods=["POST", "OPTIONS"],
    allow_headers=["content-type"],
)


@app.get("/health")
def health():
    return {"status": "ok", "model": MODEL_NAME, "loaded": _model is not None}


def _transcribe_file(path: str) -> str:
    if _model is None:
        raise RuntimeError("Whisper model is not loaded")
    with _inference_lock:
        segments, _info = _model.transcribe(
            path,
            language="en",
            beam_size=3,
            vad_filter=True,
            condition_on_previous_text=False,
        )
        return " ".join(segment.text.strip() for segment in segments).strip()


@app.post("/v1/audio/transcriptions")
async def transcribe(
    file: Annotated[UploadFile, File()],
    response_format: Annotated[str, Form()] = "json",
    requested_model: Annotated[str, Form(alias="model")] = "whisper-1",
):
    if response_format != "json":
        return JSONResponse({"error": "Only JSON transcription responses are supported."}, status_code=400)

    audio = await file.read(MAX_AUDIO_BYTES + 1)
    if not audio:
        return JSONResponse({"error": "No audio received."}, status_code=400)
    if len(audio) > MAX_AUDIO_BYTES:
        return JSONResponse({"error": "Recording is too large (13 MB maximum)."}, status_code=413)

    temp_path: str | None = None
    try:
        with tempfile.NamedTemporaryFile(suffix=Path(file.filename or "audio.wav").suffix or ".wav", delete=False) as temp:
            temp.write(audio)
            temp_path = temp.name
        text = await asyncio.to_thread(_transcribe_file, temp_path)
        if not text:
            return JSONResponse({"error": "No speech was detected in the recording."}, status_code=422)
        return {"text": text}
    except Exception as error:
        detail = str(error).strip()[:250] or "Unknown transcription error"
        return JSONResponse({"error": f"Whisper transcription failed: {detail}"}, status_code=422)
    finally:
        await file.close()
        if temp_path is not None:
            Path(temp_path).unlink(missing_ok=True)