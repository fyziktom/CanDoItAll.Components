# SB04 node route browser proof

Route: `http://localhost:56429/economy/simulation-sandbox` after local development admin login.

Actions exercised: Step, Last, First, Apply frame, Snapshot.

## Runtime diagnostics
- Initial: step=0; appliedFrameIndexes=``; requiresSceneReset=`False`; runnerState=`idle`; failureReason=``; runtimeErrorCount=``
- Step: step=1; appliedFrameIndexes=`0,1`; requiresSceneReset=`True`; runnerState=`explicit-replay-applied`; failureReason=``; runtimeErrorCount=`0`
- Last: step=2; appliedFrameIndexes=`0,1,2`; requiresSceneReset=`True`; runnerState=`explicit-replay-applied`; failureReason=``; runtimeErrorCount=`0`
- First: step=0; appliedFrameIndexes=`0`; requiresSceneReset=`True`; runnerState=`explicit-replay-applied`; failureReason=``; runtimeErrorCount=`0`
- Apply frame: step=0; appliedFrameIndexes=`0`; requiresSceneReset=`True`; runnerState=`explicit-replay-applied`; failureReason=``; runtimeErrorCount=`0`
- Snapshot: step=0; appliedFrameIndexes=`0`; requiresSceneReset=`True`; runnerState=`explicit-replay-applied`; failureReason=``; runtimeErrorCount=`0`

## Console review
- Console warning/error/log entries captured: 0
- Artifact: `proof/SB04/browser/console-review.json`
- Artifact: `proof/SB04/browser/runtime-diagnostics.json`
