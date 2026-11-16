using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib;

public interface IPageHeaderOutlet {
    void Register(object owner, PageHeaderPresentation presentation);
    void Clear(object owner);
}

public sealed record PageHeaderPresentation(
    string Title,
    string? Description,
    string Eyebrow,
    bool Compact,
    RenderFragment? Actions,
    RenderFragment? Stats);
