# OpenClaw Setup (`openclaw/`)

This folder contains the SOC skill and its portable agent role. The console connects to an OpenAI-compatible OpenClaw Gateway configured in the project-root `.env`; do not commit secrets. Start the gateway using your local OpenClaw setup, then run `npm run dev`.

## Gateway Configuration

The relevant project settings are `OPENCLAW_GATEWAY_URL`, `OPENCLAW_API_KEY`, and `OPENCLAW_DEFAULT_MODEL`. The repository does not include a `gateway.yaml`; gateway configuration stays in your local OpenClaw installation.

## Local Voice Input

The OpenClaw gateway in this setup returns 404 for `/v1/audio/transcriptions`. For speech input, run the CPU-only local Whisper sidecar in [`asr-server/README.md`](./asr-server/README.md), then select **Local · Whisper-compatible server** with base URL `http://localhost:8000/v1` in General settings.

## Skill Installation

Install the skill in the workspace used by the gateway agent. For the default Windows workspace:

```powershell
Copy-Item -Recurse .\openclaw\skills\log-triage "$HOME\.openclaw\workspace\skills\"
openclaw skills info log-triage --agent main
```

Keep `data/auth.log` beside `SKILL.md`. The repository's log is a test fixture, not live telemetry.

## Agent Profiles

[`agents/soc.md`](./agents/soc.md) documents the portable SOC role and safety boundary. It is not an OpenClaw-native config file and is not auto-loaded by the gateway. The console also has a SOC profile in `src/lib/settings/voice.ts`; its role guidance is sent separately from planning data.

The console profile and an OpenClaw isolated agent are different things. Check `openclaw agents list` before creating one. If a separate gateway agent is needed, create it with `openclaw agents add soc --workspace "$HOME\.openclaw\workspace-soc"`, install the skill into that workspace, and apply the role in `agents/soc.md`. Do not bind it to a messaging channel unless that routing is intended.

## Verification

Run `npm run typecheck`, `npm test`, `npm run lint`, and `npm run build`. With the gateway running, select the SOC profile and ask it to triage the bundled fixture. Confirm it reads the entire file, identifies the suspicious login and later privilege changes, flags the embedded instruction as untrusted, and recommends rather than performs remediation.

## Assignment

```text
openclaw/
├── README.md               # How to start your OpenClaw gateway & required env vars
├── gateway.yaml            # OpenClaw gateway configuration & model routing
├── agents/                 # Your custom OpenClaw agent definitions (orchestrator + specialists)
└── skills/                 # Custom OpenClaw skills, tool scripts, or MCP server configs
```

See **[`../ASSIGNMENT.md`](../ASSIGNMENT.md)** and **`../DarkCoders-OpenClaw-Assignment.pdf`** for full details.
