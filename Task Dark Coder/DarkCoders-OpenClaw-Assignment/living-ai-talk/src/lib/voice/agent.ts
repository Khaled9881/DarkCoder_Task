import { createParser } from "eventsource-parser";

export type Turn = { role: "user" | "assistant"; text: string };

export function toolCallNameFromEvent(payload: unknown): string | null {
  if (!payload || typeof payload !== "object") return null;
  const event = payload as Record<string, unknown>;
  if (event["type"] === "response.tool_call.started" && typeof event["name"] === "string")
    return event["name"].trim().slice(0, 80) || null;
  if (event["type"] !== "response.output_item.added") return null;
  const item = event["item"];
  if (!item || typeof item !== "object") return null;
  const tool = item as Record<string, unknown>;
  return tool["type"] === "function_call" && typeof tool["name"] === "string"
    ? tool["name"].trim().slice(0, 80) || null
    : null;
}

export async function readSse(response: Response, onEvent: (data: string) => void) {
  if (!response.body) throw new Error("Empty response stream");
  const parser = createParser({ onEvent: (event) => onEvent(event.data) });
  const reader = response.body.pipeThrough(new TextDecoderStream()).getReader();
  try {
    while (true) {
      const next = await reader.read();
      if (next.done) break;
      parser.feed(next.value);
    }
    parser.reset({ consume: true });
  } finally {
    reader.releaseLock();
  }
}

export async function readError(response: Response, fallback: string) {
  try {
    const body = (await response.clone().json()) as { error?: unknown };
    if (typeof body.error === "string") return body.error;
  } catch {
    /* ignore */
  }
  return `${fallback} (HTTP ${response.status})`;
}

/** Uploads the recording and streams the transcript back. */
export async function transcribeRecording(
  file: File,
  onPartial: (text: string) => void,
  signal?: AbortSignal,
): Promise<string> {
  const form = new FormData();
  form.append("file", file, file.name);
  const response = await fetch("/api/transcribe", {
    method: "POST",
    body: form,
    ...(signal ? { signal } : {}),
  });
  if (!response.ok)
    throw new Error(await readError(response, "Could not understand that recording"));

  let text = "";
  await readSse(response, (data) => {
    const payload = JSON.parse(data) as {
      type?: string;
      delta?: string;
      text?: string;
      error?: unknown;
    };
    if (payload.error) throw new Error("Transcription failed");
    if (payload.type === "transcript.text.delta" && payload.delta) {
      text += payload.delta;
      onPartial(text);
    }
    if (payload.type === "transcript.text.done" && typeof payload.text === "string") {
      text = payload.text;
      onPartial(text);
    }
  });
  const final = text.trim();
  if (!final) throw new Error("I didn't hear any speech in that recording");
  return final;
}

/** Streams the agent answer, reporting reasoning summary text and answer deltas separately. */
export async function streamAgentAnswer(
  turns: Turn[],
  handlers: {
    onThinking: (text: string) => void;
    onAnswer: (text: string) => void;
    onToolCall: (name: string) => void;
  },
  signal?: AbortSignal,
  context?: string,
  model?: string,
  agentRole?: string,
): Promise<string> {
  const response = await fetch("/api/agent", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ turns, context, model, agentRole }),
    ...(signal ? { signal } : {}),
  });
  if (!response.ok) throw new Error(await readError(response, "The agent could not answer"));

  let answer = "";
  let thinking = "";
  const seenToolCalls = new Set<string>();
  await readSse(response, (data) => {
    const payload = JSON.parse(data) as {
      type?: string;
      delta?: string;
      text?: string;
      error?: unknown;
      index?: number;
      call_id?: string;
      item?: { id?: string; call_id?: string };
    };
    if (payload.error || payload.type === "error") throw new Error("The agent stream failed");
    const toolName = toolCallNameFromEvent(payload);
    if (toolName) {
      const key =
        payload.call_id ??
        payload.item?.call_id ??
        payload.item?.id ??
        String(payload.index ?? toolName);
      if (!seenToolCalls.has(key)) {
        seenToolCalls.add(key);
        handlers.onToolCall(toolName);
      }
    }
    if (payload.type === "response.reasoning_summary_text.delta" && payload.delta) {
      thinking += payload.delta;
      handlers.onThinking(thinking);
    }
    if (payload.type === "response.output_text.delta" && payload.delta) {
      answer += payload.delta;
      handlers.onAnswer(answer);
    }
    if (payload.type === "response.output_text.done" && typeof payload.text === "string") {
      answer = payload.text;
      handlers.onAnswer(answer);
    }
  });
  const final = answer.trim();
  if (!final) throw new Error("The agent returned an empty answer");
  return final;
}
