# WebGlRunLib generic sample

This small console sample validates two generic `WebGlRunDocument` instances against `WebGlRunDocumentValidator`. It demonstrates the run-layer vocabulary: scenes, frames, action stages, motions, patches, barriers, and metadata, without requiring a browser host or product-specific domain package.

Run it with:

```powershell
dotnet run --project samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj
```

Use this sample to understand the shape of a valid playback document. A Blazor host should render the initial scene with WebGlLib and apply frames through `WebGlRunDocumentRunner` and the WebGlLib browser adapter. Keep business terms and persistence choices in the consuming application.
