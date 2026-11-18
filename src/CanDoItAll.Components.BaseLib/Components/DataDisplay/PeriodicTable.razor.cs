using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib;

/// <summary>
/// An interactive periodic table of the 118 elements (<see cref="PeriodicTableElements"/>) on the conventional 18-column
/// grid, coloured by block, with a searchable list alternative. The host decides what an element means: it marks
/// elements unavailable or adds a note through <see cref="States"/>, binds <see cref="SelectedNumber"/> and handles
/// <see cref="ElementChosen"/>.
/// </summary>
/// <remarks>
/// The table is a single tab stop (a roving tab index on the chosen element, else hydrogen). The arrow keys Left and
/// Right move to the next element in the row, Up and Down to the nearest element (by column) in the next row that has
/// elements, Home and End to the ends of the row; Enter, Space or a click choose. In the list, ArrowDown in the search
/// field moves into the matches, ArrowUp and ArrowDown move within them, Enter or Space choose, Escape returns to the
/// field, and Enter in the field chooses the first match. Narrow containers scroll the table sideways inside the
/// component, never the page.
/// </remarks>
public partial class PeriodicTable
{
    private const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/DataDisplay/PeriodicTable.razor.js";
    private const string SearchSelector = "input[role=\"combobox\"]";
    private const int MaximumMatches = 12;
    private const int LastRow = 10;
    private const int LastColumn = 18;

    private static readonly PeriodicTableElementState DefaultState = new();

    private readonly string idPrefix = $"ui-periodic-table-{Guid.NewGuid():N}";
    private ElementCell[] cells = [];
    private ElementReference root;
    private Task<IJSObjectReference?>? moduleLoad;
    private IReadOnlyList<PeriodicTableElement> matches = [];
    private string query = string.Empty;
    private int activeNumber = 1;
    private int? activeOption;
    private int? appliedSelection;
    private string? pendingFocus;
    private bool disposed;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    /// <summary>
    /// The atomic number of the chosen element, or null when none is chosen; bind it with <c>@bind-SelectedNumber</c>.
    /// The chosen element is marked with a check mark and <c>aria-pressed="true"</c> and holds the table's tab stop.
    /// Values outside 1 to 118 choose nothing.
    /// </summary>
    [Parameter]
    public int? SelectedNumber { get; set; }

    /// <summary>Raised with the atomic number when the user chooses an element other than the chosen one.</summary>
    [Parameter]
    public EventCallback<int?> SelectedNumberChanged { get; set; }

    /// <summary>
    /// Raised every time the user chooses an element (an unavailable one and the one already chosen included), with the
    /// element and whether it was chosen in the table or in the list; after <see cref="SelectedNumberChanged"/>.
    /// </summary>
    [Parameter]
    public EventCallback<PeriodicTableChoice> ElementChosen { get; set; }

    /// <summary>
    /// The host's state of the elements, keyed by atomic number: whether each is available and an optional note.
    /// Elements without an entry are available and have no note. An unavailable element stays focusable and choosable.
    /// </summary>
    [Parameter]
    public IReadOnlyDictionary<int, PeriodicTableElementState>? States { get; set; }

    /// <summary>Shows the list alternative below the table: a search field listing up to 12 matching elements.</summary>
    [Parameter]
    public bool ShowSearch { get; set; } = true;

    /// <summary>
    /// Optional prefix of test hooks. When set, each element button gets <c>data-testid="{prefix}{Symbol}"</c>, the search
    /// field <c>{prefix}search</c>, the list <c>{prefix}list</c> and each option <c>{prefix}option-{Symbol}</c>. Element
    /// buttons always carry <c>data-z</c> and <c>data-symbol</c>.
    /// </summary>
    [Parameter]
    public string? ElementTestIdPrefix { get; set; }

    /// <summary>Accessible name of the group of element buttons.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Periodic table: arrow keys move between elements, Enter chooses one";

    /// <summary>Visible label of the search field.</summary>
    [Parameter]
    public string SearchLabel { get; set; } = "Find an element by name, symbol or atomic number";

    /// <summary>Accessible name of the list of matching elements.</summary>
    [Parameter]
    public string ListAriaLabel { get; set; } = "Matching elements";

    /// <summary>The word naming an unavailable element's state in its accessible name, tooltip and list option.</summary>
    [Parameter]
    public string UnavailableText { get; set; } = "unavailable";

    private string SearchId => $"{idPrefix}-search";

    private string ListId => $"{idPrefix}-list";

    private string Query
    {
        get => query;
        set
        {
            query = value ?? string.Empty;
            matches = PeriodicTableElements.Search(query, MaximumMatches);
            activeOption = null;
        }
    }

    protected override void OnInitialized()
    {
        cells = PeriodicTableElements.All
            .Select(element => new ElementCell(element, () => ChooseAsync(element, PeriodicTableChoiceSource.Table)))
            .ToArray();
    }

    protected override void OnParametersSet()
    {
        if (SelectedNumber == appliedSelection)
        {
            return;
        }

        // A choice made by the host moves the tab stop to it, so Tab enters the table at the chosen element.
        appliedSelection = SelectedNumber;
        if (SelectedNumber is int number && PeriodicTableElements.ByNumber(number) is not null)
        {
            activeNumber = number;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeModuleAsync("attach", root);
        }

        if (pendingFocus is { } selector)
        {
            pendingFocus = null;
            await InvokeModuleAsync("focus", root, selector);
        }
    }

    private void HandleTableKeyDown(KeyboardEventArgs args)
    {
        if (args.AltKey || args.CtrlKey || args.MetaKey)
        {
            return;
        }

        // Moves from the element holding the tab stop rather than the event's button: keys pressed faster than the
        // focus follows still count from where the previous key arrived.
        var current = PeriodicTableElements.ByNumber(activeNumber) ?? PeriodicTableElements.All[0];
        var next = args.Key switch
        {
            "ArrowRight" => NextInRow(current, 1),
            "ArrowLeft" => NextInRow(current, -1),
            "ArrowDown" => NearestInNextRow(current, 1),
            "ArrowUp" => NearestInNextRow(current, -1),
            "Home" => RowEnd(current.Row, last: false),
            "End" => RowEnd(current.Row, last: true),
            _ => null
        };

        if (next is null)
        {
            return;
        }

        // Enter and Space are not handled here: they keep the button's native click, which chooses.
        activeNumber = next.AtomicNumber;
        pendingFocus = ElementSelector(next.AtomicNumber);
    }

    private async Task HandleSearchKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.AltKey || args.CtrlKey || args.MetaKey || args.ShiftKey || args.IsComposing || matches.Count == 0)
        {
            return;
        }

        if (args.Key == "ArrowDown")
        {
            MoveToOption(matches[0]);
        }
        else if (args.Key == "Enter")
        {
            await ChooseAsync(matches[0], PeriodicTableChoiceSource.List);
        }
    }

    private async Task HandleOptionKeyDownAsync(KeyboardEventArgs args, PeriodicTableElement option)
    {
        if (args.AltKey || args.CtrlKey || args.MetaKey)
        {
            return;
        }

        var index = activeOption is int active ? IndexOfMatch(active) : -1;
        if (index < 0)
        {
            index = IndexOfMatch(option.AtomicNumber);
        }

        if (index < 0)
        {
            return;
        }

        switch (args.Key)
        {
            case "ArrowDown":
                if (index + 1 < matches.Count)
                {
                    MoveToOption(matches[index + 1]);
                }

                break;
            case "ArrowUp":
                if (index > 0)
                {
                    MoveToOption(matches[index - 1]);
                }
                else
                {
                    MoveToSearch();
                }

                break;
            case "Enter":
            case " ":
            case "Spacebar":
                await ChooseAsync(matches[index], PeriodicTableChoiceSource.List);
                break;
            case "Escape":
                MoveToSearch();
                break;
        }
    }

    private async Task ChooseAsync(PeriodicTableElement element, PeriodicTableChoiceSource source)
    {
        activeNumber = element.AtomicNumber;
        if (source == PeriodicTableChoiceSource.Table)
        {
            pendingFocus = ElementSelector(element.AtomicNumber);
        }
        else
        {
            activeOption = element.AtomicNumber;
        }

        if (SelectedNumber != element.AtomicNumber)
        {
            SelectedNumber = element.AtomicNumber;
            appliedSelection = element.AtomicNumber;
            await SelectedNumberChanged.InvokeAsync(element.AtomicNumber);
        }

        await ElementChosen.InvokeAsync(new PeriodicTableChoice(element, source));
    }

    private void MoveToOption(PeriodicTableElement option)
    {
        activeOption = option.AtomicNumber;
        pendingFocus = $"li[role=\"option\"][data-z=\"{option.AtomicNumber}\"]";
    }

    private void MoveToSearch()
    {
        activeOption = null;
        pendingFocus = SearchSelector;
    }

    private int IndexOfMatch(int atomicNumber)
    {
        for (var index = 0; index < matches.Count; index++)
        {
            if (matches[index].AtomicNumber == atomicNumber)
            {
                return index;
            }
        }

        return -1;
    }

    private async Task InvokeModuleAsync(string identifier, params object?[] arguments)
    {
        if (disposed)
        {
            return;
        }

        try
        {
            moduleLoad ??= JS.InvokeAsync<IJSObjectReference?>("import", ModulePath).AsTask();
            var module = await moduleLoad;
            if (module is null || disposed)
            {
                return;
            }

            await module.InvokeVoidAsync(identifier, arguments);
        }
        catch (Exception)
        {
            // The module only moves the focus and keeps handled keys from scrolling the page; the table renders,
            // chooses and moves its tab stop without it, so it must never break rendering. Static or prerendered
            // output has no JS interop (InvalidOperationException), a closed circuit throws JSDisconnectedException,
            // a failed module load throws JSException, and test renderers refuse unplanned interop calls with their
            // own exception types (bUnit's strict mode).
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        var load = moduleLoad;
        moduleLoad = null;
        if (load is null)
        {
            return;
        }

        try
        {
            var module = await load;
            if (module is null)
            {
                return;
            }

            try
            {
                await module.InvokeVoidAsync("detach", root);
            }
            finally
            {
                await module.DisposeAsync();
            }
        }
        catch (Exception)
        {
            // Same reasons as above: a gone circuit or a refused interop call leaves nothing to release.
        }
    }

    private PeriodicTableElementState StateOf(PeriodicTableElement element)
        => States is not null && States.TryGetValue(element.AtomicNumber, out var state) && state is not null
            ? state
            : DefaultState;

    private string LabelOf(PeriodicTableElement element, PeriodicTableElementState state)
    {
        var label = $"{element.AtomicNumber} {element.Symbol} {element.Name}";
        var note = string.IsNullOrWhiteSpace(state.Note) ? null : state.Note.Trim();
        var unavailable = state.Available || string.IsNullOrWhiteSpace(UnavailableText) ? null : UnavailableText.Trim();
        return (unavailable, note) switch
        {
            (null, null) => label,
            (null, _) => $"{label}: {note}",
            (_, null) => $"{label}: {unavailable}",
            _ => $"{label}: {unavailable}, {note}"
        };
    }

    private string OptionText(PeriodicTableElement element, PeriodicTableElementState state)
        => state.Available || string.IsNullOrWhiteSpace(UnavailableText)
            ? $"{element.AtomicNumber} {element.Symbol} {element.Name}"
            : $"{element.AtomicNumber} {element.Symbol} {element.Name} ({UnavailableText.Trim()})";

    private string OptionId(PeriodicTableElement element) => $"{idPrefix}-option-{element.AtomicNumber}";

    private static string OptionClass(PeriodicTableElementState state)
        => state.Available ? "ui-periodic-table__option" : "ui-periodic-table__option ui-periodic-table__option--unavailable";

    private string? TestId(string suffix)
        => string.IsNullOrEmpty(ElementTestIdPrefix) ? null : ElementTestIdPrefix + suffix;

    private static string ElementClass(ElementCell cell, PeriodicTableElementState state, bool chosen)
        => (state.Available, chosen) switch
        {
            (true, false) => cell.Class,
            (true, true) => cell.Class + " ui-periodic-table__element--chosen",
            (false, false) => cell.Class + " ui-periodic-table__element--unavailable",
            _ => cell.Class + " ui-periodic-table__element--unavailable ui-periodic-table__element--chosen"
        };

    private static string ElementSelector(int atomicNumber) => $"button[data-z=\"{atomicNumber}\"]";

    private static PeriodicTableElement? At(int row, int column)
    {
        foreach (var element in PeriodicTableElements.All)
        {
            if (element.Row == row && element.Column == column)
            {
                return element;
            }
        }

        return null;
    }

    private static PeriodicTableElement? NextInRow(PeriodicTableElement from, int step)
    {
        for (var column = from.Column + step; column is >= 1 and <= LastColumn; column += step)
        {
            if (At(from.Row, column) is { } next)
            {
                return next;
            }
        }

        return null;
    }

    // The row gap (row 8) and the ends of the table are skipped; on a tie the element with the lower column wins.
    private static PeriodicTableElement? NearestInNextRow(PeriodicTableElement from, int step)
    {
        for (var row = from.Row + step; row is >= 1 and <= LastRow; row += step)
        {
            PeriodicTableElement? nearest = null;
            foreach (var element in PeriodicTableElements.All)
            {
                if (element.Row == row
                    && (nearest is null || Math.Abs(element.Column - from.Column) < Math.Abs(nearest.Column - from.Column)))
                {
                    nearest = element;
                }
            }

            if (nearest is not null)
            {
                return nearest;
            }
        }

        return null;
    }

    private static PeriodicTableElement? RowEnd(int row, bool last)
    {
        PeriodicTableElement? end = null;
        foreach (var element in PeriodicTableElements.All)
        {
            if (element.Row == row && (end is null || (last ? element.Column > end.Column : element.Column < end.Column)))
            {
                end = element;
            }
        }

        return end;
    }

    // One instance per element for the component's lifetime: re-renders reuse the same click delegate, so they do not
    // re-register 118 event handlers each time the search text changes.
    private sealed class ElementCell(PeriodicTableElement element, Func<Task> choose)
    {
        public PeriodicTableElement Element { get; } = element;

        public string Class { get; } = $"ui-periodic-table__element ui-periodic-table__element--block-{element.Block}";

        public string Placement { get; } = $"grid-row:{element.Row};grid-column:{element.Column}";

        public Func<Task> Choose { get; } = choose;
    }
}
