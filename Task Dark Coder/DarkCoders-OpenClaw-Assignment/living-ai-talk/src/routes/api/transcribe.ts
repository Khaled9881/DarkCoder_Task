import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/api/transcribe")({
  server: {
    handlers: {
      POST: async ({ request }: { request: Request }) => {
        const { MAX_AUDIO_BYTES } = await import("@/lib/ai/gateway.server");
        const { transcribe } = await import("@/lib/ai/transcribe.server");

        const declared = Number(request.headers.get("content-length") ?? 0);
        if (declared && declared > MAX_AUDIO_BYTES) {
          return new Response(JSON.stringify({ error: "Recording is too long" }), {
            status: 413,
            headers: { "Content-Type": "application/json" },
          });
        }

        let file: File | null = null;
        try {
          const form = await request.formData();
          const value = form.get("file");
          if (value instanceof File) file = value;
        } catch {
          file = null;
        }
        if (!file || !file.size) {
          return new Response(JSON.stringify({ error: "No audio received" }), {
            status: 400,
            headers: { "Content-Type": "application/json" },
          });
        }

        try {
          const upstream = await transcribe(file, request.signal);
          if (!upstream.ok) {
            const rawDetail = await upstream.text();
            let detail = rawDetail.trim().slice(0, 300);
            try {
              const body = JSON.parse(rawDetail) as { error?: unknown };
              if (typeof body.error === "string") detail = body.error.slice(0, 300);
            } catch {
              /* Keep the bounded upstream text for non-JSON errors. */
            }
            return new Response(
              JSON.stringify({
                error: `OpenClaw transcription returned ${upstream.status}${detail ? `: ${detail}` : ""}`,
              }),
              {
                status: upstream.status,
                headers: { "Content-Type": "application/json" },
              },
            );
          }
          const headers = new Headers({ "Cache-Control": "no-cache" });
          headers.set("Content-Type", upstream.headers.get("Content-Type") ?? "text/event-stream");
          return new Response(upstream.body, { status: upstream.status, headers });
        } catch (error) {
          if (request.signal.aborted) return new Response(null, { status: 499 });
          const message = error instanceof Error ? error.message : "Transcription failed";
          return new Response(JSON.stringify({ error: message }), {
            status: 400,
            headers: { "Content-Type": "application/json" },
          });
        }
      },
    },
  },
});
