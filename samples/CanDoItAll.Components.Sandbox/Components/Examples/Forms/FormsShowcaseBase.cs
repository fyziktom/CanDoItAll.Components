using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using CanDoItAll.Components.BaseLib;

namespace CanDoItAll.Components.Sandbox.Components.Examples.Forms;

/// <summary>Shared state for the Forms documentation examples.</summary>
public abstract class FormsShowcaseBase : ComponentBase
{
    protected bool IsDisabled => false;
    protected InputFormState FormState { get; } = new("Quarterly migration review with extended ownership and escalation notes for all teams involved", "Platform architecture review board", "Review", 90, "long-secret", true, true, false, "This example deliberately uses a longer note so the textarea, labels, and spacing prove that shared field components can tolerate editorial copy without becoming unreadable.");

    protected string TitlePlaceholder => "Long titles should wrap predictably and still read well.";
    protected string NotesPlaceholder => "Explain context, risk, and reviewer expectations.";
    protected string BudgetPrefix => "fiscal-reference";
    protected string BudgetSuffix => "approved-usd";
    protected string BudgetCodePlaceholder => "Long references must not force the control wider than its column.";
    protected string ReviewerPickerDescription => "Long labels and supporting copy must wrap inside the picker without horizontal scrolling.";
    protected IReadOnlyList<SelectOption> StatusOptions { get; } = [new("Draft", "Draft"), new("Review", "Review"), new("Blocked", "Blocked"), new("Approved", "Approved")];
    protected IReadOnlyList<string> TagSuggestions { get; } = ["accessibility", "api-contract", "density", "mobile-layout", "publishing", "visual-proof", "wrapping"];
    protected IReadOnlyList<EntityPickerItem> ReviewerOptions { get; } =
    [
        new() { Id = "design-system", Label = "Design system", Description = "Checks visual consistency, spacing, and shared component ownership.", Meta = "UI", Icon = "design_services" },
        new() { Id = "platform", Label = "Platform foundation", Description = "Reviews base behavior, package readiness, accessibility, and API shape.", Meta = "Base", Icon = "hub" },
        new() { Id = "release", Label = "Release readiness with a deliberately longer lane name", Description = "Validates publishing notes, proof links, and downstream transfer risks.", Meta = "Publish", Icon = "fact_check" }
    ];

    protected AdvancedInputState AdvancedState { get; private set; } = CreateAdvancedState();
    protected EditableReviewItem EditableState { get; private set; } = CreateEditableState();
    protected int advancedChangeCount;
    protected int editableSaveCount;
    protected int uploadSelectionCount;
    protected int uploadedFileCount;
    protected long uploadedTotalSizeBytes;
    protected IReadOnlyList<string> uploadedFileNames = [];
    protected int fileInputSelectionCount;
    protected string? fileInputFileName;
    protected bool CanReadStorage { get; set; } = true;
    protected bool CanWriteStorage { get; set; }

    protected string PriorityLabel => AdvancedState.Priority switch { >= 85 => "High review priority", >= 55 => "Standard review priority", _ => "Low review priority" };
    protected string SelectedReviewerLabel => ReviewerOptions.FirstOrDefault(item => item.Id == AdvancedState.ReviewerId)?.Label ?? "No lane";
    protected string AdvancedSummary => $"{AdvancedState.Tags.Count} tags | {SelectedReviewerLabel} | {(AdvancedState.NotifyLeads ? "notify" : "quiet")}";
    protected string UploadPrimaryText => "Drag and drop a longer MusicXML review package without losing the reading rhythm of the surface.";
    protected string UploadSecondaryText => "or browse for a plain `.musicxml` / `.xml` source while keeping the helper copy, button, and drop state aligned.";
    protected string UploadButtonText => IsDisabled ? "Upload locked" : "Choose MusicXML";
    protected string UploadedTotalSizeLabel => uploadedTotalSizeBytes == 0 ? "0 B" : FormatBytes(uploadedTotalSizeBytes);
    protected string EditableSummary => $"{EditableState.Title} | {EditableState.Confidence}% | {EditableState.EffortHours:0.0} h | {(EditableState.RequiresApproval ? "approval" : "self-review")}";

    protected Task HandleTitleChanged(string? value) => Task.CompletedTask;
    protected Task HandleOwnerChanged(string? value) => Task.CompletedTask;
    protected Task HandleStatusChanged(string? value) => Task.CompletedTask;
    protected Task HandleConfidenceChanged(int value) => Task.CompletedTask;
    protected Task HandleSecretChanged(string? value) => Task.CompletedTask;
    protected Task HandleReminderChanged(bool value) => Task.CompletedTask;
    protected Task HandleNotesChanged(string? value) => Task.CompletedTask;
    protected Task HandleApprovalChanged(bool value) => Task.CompletedTask;
    protected Task HandlePublishChanged(bool value) => Task.CompletedTask;
    protected Task HandleCanReadStorageChanged(bool value) { CanReadStorage = value; return Task.CompletedTask; }
    protected Task HandleCanWriteStorageChanged(bool value) { CanWriteStorage = value; return Task.CompletedTask; }
    protected Task HandlePriorityChanged(int value) { AdvancedState.Priority = value; advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleBudgetCodeChanged(string? value) { AdvancedState.BudgetCode = value ?? string.Empty; advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleTagsChanged(IReadOnlyList<string> value) { AdvancedState.Tags = value; advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleReviewerChanged(string value) { AdvancedState.ReviewerId = value; advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleNotifyLeadsChanged(bool value) { AdvancedState.NotifyLeads = value; advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleAdvancedReset() { AdvancedState = CreateAdvancedState(); advancedChangeCount++; return Task.CompletedTask; }
    protected Task HandleEditableChanged(EditableReviewItem item) { EditableState = item; editableSaveCount++; return Task.CompletedTask; }
    protected Task HandleUploadChanged(InputFileChangeEventArgs args) { var files = args.GetMultipleFiles(10); uploadSelectionCount++; uploadedFileCount = files.Count; uploadedTotalSizeBytes = files.Sum(file => file.Size); uploadedFileNames = files.Select(file => $"{file.Name} ({FormatBytes(file.Size)})").ToArray(); return Task.CompletedTask; }
    protected Task HandleFileInputChanged(InputFileChangeEventArgs args) { fileInputSelectionCount++; fileInputFileName = args.File.Name; return Task.CompletedTask; }
    private static string FormatBytes(long bytes) { string[] suffixes = ["B", "KB", "MB", "GB"]; double value = bytes; var index = 0; while (value >= 1024d && index < suffixes.Length - 1) { value /= 1024d; index++; } return index == 0 ? $"{value:0} {suffixes[index]}" : $"{value:0.0} {suffixes[index]}"; }
    private static AdvancedInputState CreateAdvancedState() => new() { Priority = 92, BudgetCode = "COMPONENT-PUBLISHING-READINESS-TRANSFER-REFERENCE", Tags = ["accessibility", "mobile-layout", "publishing", "visual-proof"], ReviewerId = "release", NotifyLeads = true };
    private static EditableReviewItem CreateEditableState() => new() { Title = "Quarterly migration review with extended ownership and escalation notes", Confidence = 91, EffortHours = 18.75, RequiresApproval = true };

    protected NativeInputModel NativeModel { get; } = new();
    protected NativeInputModel NativeInvalidModel { get; } = new() { Title = null };

    protected sealed record SelectOption(string Text, string Value);
    protected sealed record InputFormState(string Title, string Owner, string Status, int Confidence, string Secret, bool RemindersEnabled, bool RequiresApproval, bool PublishExternally, string Notes);
    protected sealed class AdvancedInputState { public int Priority { get; set; } public string BudgetCode { get; set; } = string.Empty; public IReadOnlyList<string> Tags { get; set; } = []; public string ReviewerId { get; set; } = string.Empty; public bool NotifyLeads { get; set; } }
    public sealed class EditableReviewItem { public string Title { get; set; } = string.Empty; public int Confidence { get; set; } public double EffortHours { get; set; } public bool RequiresApproval { get; set; } }

    /// <summary>Mutable model backing the EditForm-integrated form examples (validation state, DateInput, enum-typed SelectInput).</summary>
    public sealed class NativeInputModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(80)]
        public string? Title { get; set; } = "Quarterly migration review";

        public DateOnly? DueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public ReviewStatus? Status { get; set; } = ReviewStatus.Review;
    }

    public enum ReviewStatus { Draft, Review, Blocked, Approved }
}
