# OpenClaw SOC Voice Console (`living-ai-talk`)

A DarkCoders voice, chat, and 3D WebXR console for security operations log triage, backed by **OpenClaw**.

The SOC analyst reads authentication logs, distinguishes observed facts from likely explanations, and recommends next steps. It is read-only: investigation does not change accounts or systems, and consequential actions require human approval.

## SOC Agent

- Select **SOC analyst · log triage** under **General → Voice & Agents**.
- Ask about failed logins, suspicious activity, or whether an account or IP looks compromised. The agent should use the `log-triage` OpenClaw skill and its file-reading tool before reaching a conclusion.
- The bundled [`auth.log`](./openclaw/skills/log-triage/data/auth.log) is a test fixture using documentation-only IP ranges. It is not live telemetry and must not be described as a real incident.
- The skill treats log contents as untrusted data, reports facts separately from inference, and does not execute remediation.

See [`openclaw/README.md`](./openclaw/README.md) for gateway, skill, and agent setup. The console's SOC profile is a role in the app; an isolated OpenClaw agent is a separate gateway configuration.

## Quick Start

```bash
# Install dependencies
npm install

# Copy environment template and configure your OpenClaw Gateway URL
cp .env.example .env

# Verify types, tests, and production build
npm run typecheck
npm test
npm run build

# Start development server
npm run dev
```

## Key Scripts

- `npm run dev` — Start the Vite + TanStack Start dev server
- `npm run build` — Build client, SSR, and Nitro server bundles
- `npm run typecheck` — Run TypeScript compiler checks (`tsc --noEmit`)
- `npm test` — Run the unit test suite (`node --test`)
- `npm run lint` — Run ESLint

## Architecture Reference

- **`ASSIGNMENT.md`** — Candidate challenge brief, requirements, and grading rubric
- **`src/config/unified-plan.yaml`** — `VoiceChatOpenClawArchitecture` (`darkcoders.ai/v1alpha1`) specification
- **`src/routes/index.tsx`** — Main voice/chat/spatial console and run lifecycle controller
- **`src/routes/api/agent.ts`** — Server route connecting the console to your OpenClaw Gateway
- **`src/lib/settings/voice.ts`** — OpenClaw agent roster (`AGENTS`) and voice/ASR settings
- **`src/lib/voice/screen.ts`** — Structured screen, voice command (`app_commands`), and `action_proposal` parser

## AI Assistance

GitHub Copilot helped trace the console-to-gateway prompt flow, separate agent role guidance from untrusted plan data, add focused tests, and prepare the OpenClaw handoff. The log findings are derived from the supplied fixture; no live security data or completed remediation is claimed.

## Assignment

Read [`ASSIGNMENT.md`](./ASSIGNMENT.md) for the original requirements and grading rubric.
