# Scenario pack hash policy

Scenario manifests must bind all files that define a scenario. Required policy:

- manifest schema version
- scenario id equals directory id
- file list with relative path, bytes, sha256
- manifest-declared pack hash computed over path+content of all non-manifest files
- negative tests for changed, removed, added, oversized, and path-traversal files

The catalog may expose local paths for compatibility, but portable exports should use scenario id + pack hash + source kind.
