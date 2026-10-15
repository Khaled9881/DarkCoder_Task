/// <reference types="node" />
import assert from "node:assert/strict";
import { test } from "node:test";
import { toolCallNameFromEvent } from "./agent.ts";

test("reads only tool names from OpenClaw and Responses API call events", () => {
  assert.equal(
    toolCallNameFromEvent({
      type: "response.tool_call.started",
      name: "read_file",
      arguments: "secret",
    }),
    "read_file",
  );
  assert.equal(
    toolCallNameFromEvent({
      type: "response.output_item.added",
      item: { type: "function_call", name: "search_logs", arguments: "secret" },
    }),
    "search_logs",
  );
  assert.equal(
    toolCallNameFromEvent({ type: "response.output_text.delta", delta: "answer" }),
    null,
  );
  assert.equal(
    toolCallNameFromEvent({ type: "response.output_item.added", item: { type: "message" } }),
    null,
  );
});
