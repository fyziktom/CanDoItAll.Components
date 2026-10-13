# Canvas composer acknowledgements

Consumers with asynchronous writes can set `RequiresComposerAcknowledgement="true"` and handle `ComposerOpened`. Create/edit events then include `ComposerOpeningId` and `ComposerSubmissionId`. After deciding the write outcome, the host calls `CanvasWorkbench.AcknowledgeComposerAsync` with a `CanvasWorkbenchComposerResult` containing those same identities.

`Committed` closes an unchanged submitted draft. `HandedOff` means another editor accepted responsibility for the request, not that a write committed. `Rejected` and `Unconfirmed` preserve the draft with the host's message. Missing acknowledgement or transport failure is explicitly unconfirmed. Further typing remains visible even after a successful save. A completed opening cannot dispatch another submission; copy retained text and reopen against current state.

CanvasLib owns only the opening, local revision, text, pending state and identity checks. The consumer owns validation, concurrency preconditions, durable outcomes and reconciliation. Closing/replacing the opening or replacing the canvas fences delayed acknowledgements. Optional acknowledgement mode leaves existing event-only consumers compatible.

Validation: `npm run canvaslib:test-composer`, `npm run assets:verify`, and `CanvasWorkbenchComposerContractTests`. Consumer browser proof must cover a rejected save, a delayed commit with continued typing, and a replaced opening.
