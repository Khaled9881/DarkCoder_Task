import { readError } from "./agent.ts";
export function transcriptionUrl(base: string) {
  const url = new URL(base);
  if (!["http:", "https:"].includes(url.protocol) || url.username || url.password)
    throw new Error("Use an HTTP or HTTPS transcription server without embedded credentials.");
  return `${url.href.replace(/\/$/, "")}/audio/transcriptions`;
}
export async function transcribeLocal(
  file: File,
  base: string,
  model: string,
  onPartial: (text: string) => void,
  signal: AbortSignal,
) {
  const form = new FormData();
  form.append("file", file);
  form.append("model", model);
  form.append("response_format", "json");
  let response: Response;
  try {
    response = await fetch(transcriptionUrl(base), { method: "POST", body: form, signal });
  } catch (error) {
    if (signal.aborted) throw error;
    throw new Error(
      `Couldn't reach local ASR at ${base}. Start its Whisper-compatible server or choose Built-in cloud transcription in General settings.`,
      { cause: error },
    );
  }
  if (!response.ok)
    throw new Error(await readError(response, `Local ASR returned ${response.status}`));
  const data: unknown = await response.json();
  const text =
    data && typeof data === "object" && "text" in data && typeof data.text === "string"
      ? data.text.trim()
      : "";
  if (!text) throw new Error("Local ASR returned no transcript.");
  onPartial(text);
  return text;
}
