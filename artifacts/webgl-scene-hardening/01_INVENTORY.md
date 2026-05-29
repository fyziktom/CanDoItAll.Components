# WebGL Scene Hardening Inventory

Large-screen-only hard rule: this follow-up bundle intentionally does not tune small-screen layouts.

## GLB/GLTF Asset Table

| asset file | size | logical proposed id | recommended use | fallback id | status |
| --- | ---: | --- | --- | --- | --- |
| 3DModels/glb/buildings/Blacksmith.glb | 618248 | asset.external.building.blacksmith | optional high-detail building variant | asset.building.house.default | wired in sandbox/catalog |
| 3DModels/glb/buildings/House_1.glb | 391772 | asset.external.building.house-1 | optional high-detail building variant | asset.building.house.default | wired in sandbox/catalog |
| 3DModels/glb/buildings/House_2.glb | 483576 | asset.external.building.house-2 | optional high-detail building variant | asset.building.house.default | wired in sandbox/catalog |
| 3DModels/glb/buildings/Inn.glb | 466280 | asset.external.building.inn | optional high-detail building variant | asset.building.house.default | wired in sandbox/catalog |
| 3DModels/glb/buildings/Mill.glb | 518868 | asset.external.building.mill | optional high-detail building variant | asset.building.house.default | catalog alternative |
| 3DModels/glb/buildings/Stable.glb | 409288 | asset.external.building.stable | optional high-detail building variant | asset.building.house.default | catalog alternative |
| 3DModels/glb/people/Female_LookingUp.glb | 98040 | asset.external.person.female-lookingup | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Female_PickingUp.glb | 102016 | asset.external.person.female-pickingup | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Female_Running.glb | 99932 | asset.external.person.female-running | optional agent/person variant | asset.agent.person.default | wired in sandbox/catalog |
| 3DModels/glb/people/Female_Sitting_Cheering.glb | 101700 | asset.external.person.female-sitting-cheering | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Female_Sitting.glb | 101692 | asset.external.person.female-sitting | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Female_Standing_CoveringEyes.glb | 101840 | asset.external.person.female-standing-coveringeyes | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Female_Standing_Hips.glb | 98192 | asset.external.person.female-standing-hips | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Female_Standing.glb | 101508 | asset.external.person.female-standing | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Female_Walking.glb | 101888 | asset.external.person.female-walking | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Male_LookingUp.glb | 69440 | asset.external.person.male-lookingup | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Male_PickingUp.glb | 69508 | asset.external.person.male-pickingup | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Male_Running.glb | 69440 | asset.external.person.male-running | optional agent/person variant | asset.agent.person.default | wired in sandbox/catalog |
| 3DModels/glb/people/Male_Sitting_Cheering.glb | 69456 | asset.external.person.male-sitting-cheering | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Male_Sitting.glb | 69444 | asset.external.person.male-sitting | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Male_Standing_CoveringEyes.glb | 69456 | asset.external.person.male-standing-coveringeyes | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Male_Standing_Hips.glb | 69420 | asset.external.person.male-standing-hips | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Male_Standing_Waving.glb | 69484 | asset.external.person.male-standing-waving | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Male_Standing.glb | 69452 | asset.external.person.male-standing | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Male_Walking.glb | 69412 | asset.external.person.male-walking | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/people/Woman_Standing_Waving.glb | 99880 | asset.external.person.woman-standing-waving | optional agent/person variant | asset.agent.person.default | catalog alternative |
| 3DModels/glb/props/Bags.glb | 20556 | asset.external.prop.bags | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Barrel.glb | 51332 | asset.external.prop.barrel | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Bonfire_Lit.glb | 48744 | asset.external.prop.bonfire-lit | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Bonfire.glb | 29308 | asset.external.prop.bonfire | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Cart.glb | 106888 | asset.external.prop.cart | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Cauldron.glb | 45424 | asset.external.prop.cauldron | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Crate.glb | 34968 | asset.external.prop.crate | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Hay.glb | 26700 | asset.external.prop.hay | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/MarketStand_2.glb | 52948 | asset.external.prop.marketstand-2 | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Path_Square.glb | 19032 | asset.external.prop.path-square | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Path_Straight.glb | 37464 | asset.external.prop.path-straight | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Smoke.glb | 23228 | asset.external.prop.smoke | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| 3DModels/glb/props/Well.glb | 107864 | asset.external.prop.well | optional prop catalog asset | asset.primitive.fallback | catalog alternative |
| src/CanDoItAll.Components.WebGlLib/wwwroot/assets/model/1gears.glb | 2747328 | asset.library.1gears | shared WebGlLib model asset | asset.primitive.fallback | wired in sandbox/catalog |
| src/CanDoItAll.Components.WebGlLib/wwwroot/assets/model/gears.glb | 480552 | asset.library.gears | shared WebGlLib model asset | asset.primitive.fallback | wired in sandbox/catalog |
| src/CanDoItAll.Components.WebGlLib/wwwroot/assets/model/lowpoly_person_boxing.glb | 363328 | asset.library.lowpoly-person-boxing | shared WebGlLib model asset | asset.primitive.fallback | wired in sandbox/catalog |
| src/CanDoItAll.Components.WebGlLib/wwwroot/assets/model/question_box.glb | 405612 | asset.library.question-box | shared WebGlLib model asset | asset.primitive.fallback | wired in sandbox/catalog |
