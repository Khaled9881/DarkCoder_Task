/// <reference types="node" />
import assert from "node:assert/strict";
import { test } from "node:test";
import {
  actionProposalDecisionText,
  parseActionProposalDecision,
  type ActionProposal,
} from "./screen.ts";

const proposal: ActionProposal = {
  id: "block_ip_01",
  title: "Block suspicious IP",
  tool: "ops.block_ip",
  summary: "Block 203.0.113.45 at the network edge.",
  risk: "high",
};

test("approval is recorded without claiming execution", () => {
  const message = actionProposalDecisionText(proposal, "approved");

  assert.match(message, /approved|Approval recorded/i);
  assert.match(message, /not executed/i);
  assert.match(message, /no system changes were made/i);
  assert.match(message, /no remediation executor is configured/i);
});

test("denial confirms that no action was executed", () => {
  const message = actionProposalDecisionText(proposal, "denied");

  assert.match(message, /denied/i);
  assert.match(message, /no action was executed/i);
});

test("dismissal confirms that no action was executed", () => {
  const message = actionProposalDecisionText(proposal, "dismissed");

  assert.match(message, /dismissed/i);
  assert.match(message, /no action was executed/i);
});

test("only clear approval and denial replies are treated as proposal decisions", () => {
  assert.equal(parseActionProposalDecision("I approve the proposal"), "approved");
  assert.equal(parseActionProposalDecision("Yes, approve it."), "approved");
  assert.equal(parseActionProposalDecision("I deny this action"), "denied");
  assert.equal(parseActionProposalDecision("Dismiss the proposal"), "dismissed");
  assert.equal(parseActionProposalDecision("Please prepare proposals for my approval"), null);
  assert.equal(parseActionProposalDecision("I approve, but explain the impact first"), null);
});
