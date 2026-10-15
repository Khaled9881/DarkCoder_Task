# Local Whisper ASR

This CPU-only sidecar exposes the OpenAI-compatible endpoint used by the console at `http://localhost:8000/v1/audio/transcriptions`. It binds only to loopback and accepts recordings up to 13 MB. The default `base.en` model is downloaded on first launch and cached outside the repository.

## Start on Windows

From the repository root, run:

```powershell
.\openclaw\asr-server\start.ps1
```

Keep that terminal open while using voice input. The first launch creates an isolated environment under `%LOCALAPPDATA%\DarkCoders\whisper-asr-venv`, installs the dependencies, and downloads the model weights. No API key is needed.

Check readiness from another PowerShell window:

```powershell
Invoke-RestMethod http://127.0.0.1:8000/health
```

The response should report `status: ok` and `loaded: true`. In the console, select **General → Voice & Agents → Speech recognition → Local · Whisper-compatible server** and set the base URL to `http://localhost:8000/v1`.

Set `$env:WHISPER_MODEL = "tiny.en"` before launch for a smaller, faster model, or `base.en` for the default accuracy/speed balance. Model inference is CPU-only with int8 quantization.
