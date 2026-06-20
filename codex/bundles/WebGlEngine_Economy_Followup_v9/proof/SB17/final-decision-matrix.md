# SB17 Final Decision Matrix

Final catalog run used --no-oracle, so oracleValid is false for every scenario even when headless artifacts are otherwise valid.

| Scenario | Exploratory | Headless valid | Oracle valid | Browser observer valid | Research ready | Final status | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `farmer-land` | True | False | False | False | False | `failed` | Strict headless status is failed. Oracle not exercised in final catalog run. Browser observer proof not run for this scenario in v9 closure. Scenario errors: invalid-value:scenario.expansionProfileId: Strict mode requires an explicit behavior expansion profile. Simulation errors: missing-actor:events.event.expander.invest.actorIds: Actor 'crop-output' was not found in state indexes. \\| missing-resource:events.event.expander.invest.resourceIds: Resource 'crop-output' was not found in state store indexes. \\| unknown-event-handler:events.event.expander.invest.eventKind: No simulation event handler is registered for event kind 'actor.work.perform'. \\| unknown-event-handler:events.event.land-board.check.eventKind: No simulation event handler is registered for event kind 'rule.check.evaluate'. |
| `multi-goods-elite` | True | True | False | True | False | `headless-valid` | Oracle not exercised in final catalog run. |
| `shared-well` | True | False | False | False | False | `failed` | Strict headless status is failed. Oracle not exercised in final catalog run. Browser observer proof not run for this scenario in v9 closure. Scenario errors: invalid-value:scenario.expansionProfileId: Strict mode requires an explicit behavior expansion profile. |
