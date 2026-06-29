# Original Request

Use [$candoitall-bundle-workflow](C:\Users\dell\.codex\skills\candoitall-bundle-workflow\SKILL.md) to solve this:

Main goal:
Preparation of repository for publishing.

Architect notes:
- for final publishing we have new pure repositories. But now we must do all polishing before transfer.
- You must do detailed study of the actual implementation. You must identify all parts that need refactoring or hardening. Especially isolation of bases, helpers and things we should isolate and used as shared for easier maintenance.
- focus in this bundle only to parts with standard components and not webgl and canvas. We will do webgl and canvas preparation separatelly.
- our components sandbox might missing some components. Analyze also splitting of components into groups. Do we have them logically similar as best practice in components frameworks? If not we must improve our sandbox.
- we still might have lots of styles in some custom css. Our goal is to use tailwind for styling of components. But maybe some of them still using some "hacks". I also seen that in our tailwind inputs css we use more pure css to define some prepared style rather then using prepared tailwind system for same thing. I am not frontend specialist, so I cannot say if it is correct or not, but I guess, that in tailwind input files we should use more prepared tailwind constructions (for example when we define some align, etc).
- in "C:\repositories\CanDoItAll\src\CanDoItAll.AppComponents\Components" we still have some duplicities components I think. Some of them might be old just forgotten there and we already have implementation in Components libs, but some of them might have some improvements and might be used. We will need to reduce all basic type of components in main candoitall repo. AppComponents are more for kind of "complex" components related to web app (some prepared forms that repeats and things like this). All basic kind of components must be in here. This must be generic component base of whole framework. It must be usable also for building other projects.

Mandatory steps:
- it is very complex and it consider lots of components. You must map all in xlsx with correct references and explanations.
- you must identify correct phases of the refactoring and hardening. During planning phase it is important to do first parts that are totally general for all components (like refactor of input tailwinds) isolation of bases and things like this.
- you can make bundle larger. It is complex long run.
- design subbundles with refactoring checkpoints that will force to analyze implementation and do refactoring each few subbundles.
- you must do real validations with playwright mcp and screenshots to analyze real look of the component one by one. It means also in action (in cases of dropdowns and things like that). Sometimes happens that components has trouble with wrapping texts, and then it overflow component, or they behavie weird in layout and do not use available space (they are not stretched over width or height). those things are ususally visible from screenshots. You must not just estimate that something will help in case of styling. Styling requires visual real inspection.
