# QA prompt

Review as a senior QA inspector.

Reject the subbundle if:
- proof files are empty or placeholder-only,
- warnings are used where hard correctness gates are required,
- browser screenshots are not backed by diagnostic assertions,
- a UI/runtime failure is confused with an economic-model failure,
- generic Components packages receive Economy semantics,
- strict mode still allows unapproved semantic warnings,
- metric/invariant failures can silently evaluate to zero.
