using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using CanDoItAll.Components.BaseLib;

namespace CanDoItAll.Components.Sandbox.Components.Examples.Forms;

/// <summary>Shared scenario-aware state for the Forms documentation examples.</summary>
public abstract class FormsShowcaseBase : ComponentBase
{
    [CascadingParameter] public SandboxToolbarState? ToolbarState { get; set; }

    protected SandboxScenarioKey CurrentScenario => ToolbarState?.Scenario ?? SandboxScenarioKey.Default;
    protected bool IsDisabled => CurrentScenario == SandboxScenarioKey.DisabledState;
    protected InputFormState FormState => CurrentScenario switch
    {
        SandboxScenarioKey.DenseContent => new("Quarterly migration review", "Architecture steward", "Review", 85, "dense-secret", true, true, false, "Carry audit notes, dependent packages, and release guardrails through a tighter review lane so the screen proves it can hold more context without collapsing."),
        SandboxScenarioKey.LongText => new("Quarterly migration review with extended ownership and escalation notes for all teams involved", "Platform architecture review board", "Review", 90, "long-secret", true, true, false, "This scenario intentionally uses a longer note so the textarea, labels, and spacing show whether the shared field components can tolerate more editorial copy without turning the page into an unreadable wall of text."),
        _ => new("Component validation pass", "Design systems owner", "Draft", 70, "secret-token", true, false, true, "Capture enough context for the reviewer to understand the request without reading surrounding application code.")
    };

    protected string TitlePlaceholder => CurrentScenario == SandboxScenarioKey.LongText ? "Long titles should wrap predictably and still read well." : "Short, clear summary";
    protected string NotesPlaceholder => CurrentScenario == SandboxScenarioKey.DenseContent ? "Dense scenario notes stay compact but legible." : "Explain context, risk, and reviewer expectations.";
    protected string BudgetPrefix => CurrentScenario == SandboxScenarioKey.LongText ? "fiscal-reference" : "ref";
    protected string BudgetSuffix => CurrentScenario == SandboxScenarioKey.LongText ? "approved-usd" : "USD";
    protected string BudgetCodePlaceholder => CurrentScenario == SandboxScenarioKey.LongText ? "Long references must not force the control wider than its column." : "Review budget code";
    protected string ReviewerPickerDescription => CurrentScenario == SandboxScenarioKey.LongText ? "Long labels and supporting copy must wrap inside the picker without horizontal scrolling." : "Choose the owner lane for this validation pass.";
    protected IReadOnlyList<SelectOption> StatusOptions { get; } = [new("Draft", "Draft"), new("Review", "Review"), new("Blocked", "Blocked"), new("Approved", "Approved")];
    protected IReadOnlyList<string> TagSuggestions { get; } = ["accessibility", "api-contract", "density", "mobile-layout", "publishing", "visual-proof", "wrapping"];
    protected IReadOnlyList<EntityPickerItem> ReviewerOptions { get; } =
    [
        new() { Id = "design-system", Label = "Design system", Description = "Checks visual consistency, spacing, and shared component ownership.", Meta = "UI", Icon = "design_services" },
        new() { Id = "platform", Label = "Platform foundation", Description = "Reviews base behavior, package readiness, accessibility, and API shape.", Meta = "Base", Icon = "hub" },
        new() { Id = "release", Label = "Release readiness with a deliberately longer lane name", Description = "Validates publishing notes, proof links, and downstream transfer risks.", Meta = "Publish", Icon = "fact_check" }
    ];

    protected AdvancedInputState AdvancedState { get; private set; } = CreateAdvancedState(SandboxScenarioKey.Default);
    protected EditableReviewItem EditableState { get; private set; } = CreateEditableState(SandboxScenarioKey.Default);
    protected int advancedChangeCount;
    protected int editableSaveCount;
    protected int uploadSelectionCount;
    protected int uploadedFileCount;
    protected long uploadedTotalSizeBytes;
    protected IReadOnlyList<string> uploadedFileNames = [];
    private SandboxScenarioKey stateScenario = SandboxScenarioKey.Default;

    protected string PriorityLabel => AdvancedState.Priority switch { >= 85 => "High review priority", >= 55 => "Standard review priority", _ => "Low review priority" };
    protected string SelectedReviewerLabel => ReviewerOptions.FirstOrDefault(item => item.Id == AdvancedState.ReviewerId)?.Label ?? "No lane";
    protected string AdvancedSummary => $"{AdvancedState.Tags.Count} tags | {SelectedReviewerLabel} | {(AdvancedState.NotifyLeads ? "notify" : "quiet")}";
    protected string UploadPrimaryText => CurrentScenario switch { SandboxScenarioKey.LongText => "Drag and drop a longer MusicXML review package without losing the reading rhythm of the surface.", SandboxScenarioKey.DenseContent => "Drag and drop the current MusicXML intake package.", SandboxScenarioKey.DisabledState => "Uploads are currently locked for this scenario.", _ => "Drag and drop MusicXML" };
    protected string UploadSecondaryText => CurrentScenario switch { SandboxScenarioKey.LongText => "or browse for a plain `.musicxml` / `.xml` source while keeping the helper copy, button, and drop state aligned.", SandboxScenarioKey.DisabledState => "Browsing and drag and drop are disabled here so the component can prove its inactive state.", _ => "or browse for a plain `.musicxml` / `.xml` source." };
    protected string UploadButtonText => IsDisabled ? "Upload locked" : "Choose MusicXML";
    protected string UploadedTotalSizeLabel => uploadedTotalSizeBytes == 0 ? "0 B" : FormatBytes(uploadedTotalSizeBytes);
    protected string EditableSummary => $"{EditableState.Title} | {EditableState.Confidence}% | {EditableState.EffortHours:0.0} h | {(EditableState.RequiresApproval ? "approval" : "self-review")}";

    protected override void OnParametersSet()
    {
        if (stateScenario == CurrentScenario) return;
        stateScenario = CurrentScenario;
        AdvancedState = CreateAdvancedState(CurrentScenario);
        EditableState = CreateEditableState(CurrentScenario);
        advancedChangeCount = editableSaveCount = 0;
    }

    protected Task HandleTitleChanged(string? value) => Task.CompletedTask;
    protected Task HandleOwnerChanged(string? value) => Task.CompletedTask;
    protected Task HandleStatusChanged(string? value) => Task.CompletedTask;
    protected Task HandleConfidenceChanged(int value) => Task.CompletedTask;
    protected Task HandleSecretChanged(string? value) => Task.CompletedTask;
    protected Task HandleReminderChanged(bool value) => Task.CompletedTask;
    protected Task HandleNotesChanged(string? value) => Task.CompletedTask;
    protected Task HandleApprovalChanged(bool value) => Task.CompletedTask;
    protected Task HandlePublishChanged(bool value) => Task.CompletedTask;
    protected Task HandlePriorityChanged(int value) { AdvancedState.Priority = value; advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleBudgetCodeChanged(string? value) { AdvancedState.BudgetCode = value ?? string.Empty; advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleTagsChanged(IReadOnlyList<string> value) { AdvancedState.Tags = value; advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleReviewerChanged(string value) { AdvancedState.ReviewerId = value; advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleNotifyLeadsChanged(bool value) { AdvancedState.NotifyLeads = value; advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleAdvancedReset() { AdvancedState = CreateAdvancedState(CurrentScenario); advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleEditableChanged(EditableReviewItem item) { EditableState = item; editableSaveCount++; return Task.CompletedTask; }
    protected Task HandleUploadChanged(InputFileChangeEventArgs args) { var files = args.GetMultipleFiles(10); uploadSelectionCount++; uploadedFileCount = files.Count; uploadedTotalSizeBytes = files.Sum(file => file.Size); uploadedFileNames = files.Select(file => $"{file.Name} ({FormatBytes(file.Size)})").ToArray(); return Task.CompletedTask; }
    private static string FormatBytes(long bytes) { string[] suffixes = ["B", "KB", "MB", "GB"]; double value = bytes; var index = 0; while (value >= 1024d && index < suffixes.Length - 1) { value /= 1024d; index++; } return index == 0 ? $"{value:0} {suffixes[index]}" : $"{value:0.0} {suffixes[index]}"; }
    private static AdvancedInputState CreateAdvancedState(SandboxScenarioKey scenario) => scenario switch
    {
        SandboxScenarioKey.DenseContent => new() { Priority = 82, BudgetCode = "MIG-2026-Q3", Tags = ["density", "publishing", "visual-proof", "wrapping"], ReviewerId = "platform", NotifyLeads = true },
        SandboxScenarioKey.LongText => new() { Priority = 92, BudgetCode = "COMPONENT-PUBLISHING-READINESS-TRANSFER-REFERENCE", Tags = ["accessibility", "mobile-layout", "publishing", "visual-proof"], ReviewerId = "release", NotifyLeads = true },
        SandboxScenarioKey.DisabledState => new() { Priority = 44, BudgetCode = "LOCKED-REVIEW", Tags = ["locked", "read-only"], ReviewerId = "design-system", NotifyLeads = false },
        _ => new() { Priority = 70, BudgetCode = "BASE-READY", Tags = ["accessibility", "publishing"], ReviewerId = "design-system", NotifyLeads = true }
    };
    private static EditableReviewItem CreateEditableState(SandboxScenarioKey scenario) => scenario switch
    {
        SandboxScenarioKey.DenseContent => new() { Title = "Migration gate review", Confidence = 88, EffortHours = 12.5, RequiresApproval = true },
        SandboxScenarioKey.LongText => new() { Title = "Quarterly migration review with extended ownership and escalation notes", Confidence = 91, EffortHours = 18.75, RequiresApproval = true },
        SandboxScenarioKey.DisabledState => new() { Title = "Locked review item", Confidence = 64, EffortHours = 4, RequiresApproval = false },
        _ => new() { Title = "Component validation pass", Confidence = 70, EffortHours = 6.5, RequiresApproval = true }
    };

    protected sealed record SelectOption(string Text, string Value);
    protected sealed record InputFormState(string Title, string Owner, string Status, int Confidence, string Secret, bool RemindersEnabled, bool RequiresApproval, bool PublishExternally, string Notes);
    protected sealed class AdvancedInputState { public int Priority { get; set; } public string BudgetCode { get; set; } = string.Empty; public IReadOnlyList<string> Tags { get; set; } = []; public string ReviewerId { get; set; } = string.Empty; public bool NotifyLeads { get; set; } }
    public sealed class EditableReviewItem { public string Title { get; set; } = string.Empty; public int Confidence { get; set; } public double EffortHours { get; set; } public bool RequiresApproval { get; set; } }
}
