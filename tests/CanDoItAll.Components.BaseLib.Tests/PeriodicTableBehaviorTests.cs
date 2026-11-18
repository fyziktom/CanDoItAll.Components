using Bunit;
using Microsoft.AspNetCore.Components.Web;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class PeriodicTableBehaviorTests {
    private const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/DataDisplay/PeriodicTable.razor.js";

    private static BunitContext CreateContext() {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    [Fact]
    public void The_element_data_holds_118_elements_in_the_conventional_layout() {
        var all = PeriodicTableElements.All;

        Assert.Equal(Enumerable.Range(1, 118), all.Select(element => element.AtomicNumber));
        Assert.Equal(118, all.Select(element => element.Symbol).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal(118, all.Select(element => (element.Row, element.Column)).Distinct().Count());
        AssertCell("H", row: 1, column: 1, period: 1, group: 1, block: 's');
        AssertCell("He", row: 1, column: 18, period: 1, group: 18, block: 's');
        AssertCell("B", row: 2, column: 13, period: 2, group: 13, block: 'p');
        AssertCell("Fe", row: 4, column: 8, period: 4, group: 8, block: 'd');
        AssertCell("Ba", row: 6, column: 2, period: 6, group: 2, block: 's');
        AssertCell("Hf", row: 6, column: 4, period: 6, group: 4, block: 'd');
        AssertCell("La", row: 9, column: 3, period: 6, group: 0, block: 'f');
        AssertCell("Ce", row: 9, column: 4, period: 6, group: 0, block: 'f');
        AssertCell("Lu", row: 9, column: 17, period: 6, group: 0, block: 'f');
        AssertCell("Ac", row: 10, column: 3, period: 7, group: 0, block: 'f');
        AssertCell("Th", row: 10, column: 4, period: 7, group: 0, block: 'f');
        AssertCell("Lr", row: 10, column: 17, period: 7, group: 0, block: 'f');
        AssertCell("Rf", row: 7, column: 4, period: 7, group: 4, block: 'd');
        AssertCell("Og", row: 7, column: 18, period: 7, group: 18, block: 'p');
        Assert.Equal(
            "1:2 2:8 3:8 4:18 5:18 6:17 7:17 9:15 10:15",
            string.Join(' ', all.GroupBy(element => element.Row).OrderBy(row => row.Key).Select(row => $"{row.Key}:{row.Count()}")));
        Assert.Equal(["Nh", "Fl", "Mc", "Lv", "Ts", "Og"], Symbols(all.Skip(112)));
        Assert.Equal(
            ["Nihonium", "Flerovium", "Moscovium", "Livermorium", "Tennessine", "Oganesson"],
            all.Skip(112).Select(element => element.Name).ToArray());
        Assert.Equal(["Aluminium", "Sulfur", "Caesium"], new[] { 13, 16, 55 }.Select(z => PeriodicTableElements.ByNumber(z)!.Name).ToArray());
        Assert.Equal("Aluminum", PeriodicTableElements.ByNumber(13)!.OtherName);
        Assert.Equal(26, PeriodicTableElements.BySymbol("fe")?.AtomicNumber);
        Assert.Null(PeriodicTableElements.ByNumber(0));
        Assert.Null(PeriodicTableElements.ByNumber(119));
        Assert.Null(PeriodicTableElements.BySymbol("Xx"));
    }

    [Fact]
    public void Search_puts_the_exact_symbol_and_number_before_name_prefixes() {
        Assert.Equal(["S", "Na", "Si", "Sc", "Se", "Sr", "Ag", "Sm", "Sg"], Symbols(PeriodicTableElements.Search("s")));
        Assert.Equal(["Fe"], Symbols(PeriodicTableElements.Search(" iron ")));
        Assert.Equal(["Fe"], Symbols(PeriodicTableElements.Search("26")));
        Assert.Equal(["Cs"], Symbols(PeriodicTableElements.Search("cesium")));
        Assert.Equal(["W"], Symbols(PeriodicTableElements.Search("Wolfram")));
        Assert.Equal(["S"], Symbols(PeriodicTableElements.Search("sulph")));
        var carbon = PeriodicTableElements.Search("c");
        Assert.Equal(12, carbon.Count);
        Assert.Equal("C", carbon[0].Symbol);
        Assert.Equal(["C", "Cl", "Ca"], Symbols(PeriodicTableElements.Search("c", 3)));
        Assert.Empty(PeriodicTableElements.Search("  "));
        Assert.Empty(PeriodicTableElements.Search("119"));
    }

    [Fact]
    public async Task Every_element_is_a_placed_button_with_its_number_symbol_and_accessible_name() {
        await using var context = CreateContext();
        var table = context.Render<PeriodicTable>();

        var buttons = table.FindAll("button[data-z]");
        Assert.Equal(118, buttons.Count);
        var iron = table.Find("button[data-symbol='Fe']");
        Assert.Equal("26", iron.GetAttribute("data-z"));
        Assert.Equal("grid-row:4;grid-column:8", iron.GetAttribute("style"));
        Assert.Equal("26", iron.QuerySelector(".ui-periodic-table__number")!.TextContent);
        Assert.Equal("Fe", iron.QuerySelector(".ui-periodic-table__symbol")!.TextContent);
        Assert.Equal("26 Fe Iron", iron.GetAttribute("aria-label"));
        Assert.Equal("26 Fe Iron", iron.GetAttribute("title"));
        Assert.Equal("false", iron.GetAttribute("aria-pressed"));
        Assert.False(iron.HasAttribute("aria-disabled"));
        Assert.Contains("ui-periodic-table__element--block-d", iron.ClassList);
        Assert.Equal("grid-row:9;grid-column:3", table.Find("button[data-symbol='La']").GetAttribute("style"));
        Assert.Equal("grid-row:10;grid-column:17", table.Find("button[data-symbol='Lr']").GetAttribute("style"));

        var series = table.FindAll(".ui-periodic-table__series");
        Assert.Equal(["57–71", "89–103"], series.Select(placeholder => placeholder.TextContent).ToArray());
        Assert.All(series, placeholder => Assert.Equal("true", placeholder.GetAttribute("aria-hidden")));
        Assert.Equal("group", table.Find(".ui-periodic-table__grid").GetAttribute("role"));

        var tabStop = Assert.Single(buttons, button => button.GetAttribute("tabindex") == "0");
        Assert.Equal("H", tabStop.GetAttribute("data-symbol"));
        Assert.Empty(table.FindAll("[data-testid]"));
    }

    [Fact]
    public async Task An_unavailable_element_is_marked_says_why_and_stays_choosable() {
        await using var context = CreateContext();
        int? bound = null;
        var choices = new List<PeriodicTableChoice>();
        var table = context.Render<PeriodicTable>(parameters => parameters
            .Add(component => component.States, new Dictionary<int, PeriodicTableElementState> {
                [101] = new(Available: false, Note: "no evaluated data above Z 100"),
                [26] = new(Note: "density 7.874 g/cm3")
            })
            .Add(component => component.SelectedNumberChanged, (int? number) => bound = number)
            .Add(component => component.ElementChosen, (PeriodicTableChoice choice) => choices.Add(choice)));

        var mendelevium = table.Find("button[data-symbol='Md']");
        Assert.Equal("true", mendelevium.GetAttribute("aria-disabled"));
        Assert.Equal("101 Md Mendelevium: unavailable, no evaluated data above Z 100", mendelevium.GetAttribute("aria-label"));
        Assert.Equal(mendelevium.GetAttribute("aria-label"), mendelevium.GetAttribute("title"));
        Assert.Contains("ui-periodic-table__element--unavailable", mendelevium.ClassList);
        Assert.Equal("×", mendelevium.QuerySelector(".ui-periodic-table__mark")!.TextContent);
        var iron = table.Find("button[data-symbol='Fe']");
        Assert.Equal("26 Fe Iron: density 7.874 g/cm3", iron.GetAttribute("aria-label"));
        Assert.Null(iron.QuerySelector(".ui-periodic-table__mark"));

        mendelevium.Click();

        Assert.Equal(101, bound);
        var choice = Assert.Single(choices);
        Assert.Equal(("Md", PeriodicTableChoiceSource.Table), (choice.Element.Symbol, choice.Source));
        mendelevium = table.Find("button[data-symbol='Md']");
        Assert.Equal("true", mendelevium.GetAttribute("aria-pressed"));
        Assert.Equal("✓", mendelevium.QuerySelector(".ui-periodic-table__check")!.TextContent);
        Assert.Contains("ui-periodic-table__element--chosen", mendelevium.ClassList);
        Assert.Equal("0", mendelevium.GetAttribute("tabindex"));
        Assert.Equal("-1", table.Find("button[data-symbol='H']").GetAttribute("tabindex"));

        table.Find("button[data-symbol='Md']").Click();
        Assert.Equal(2, choices.Count);
    }

    [Fact]
    public async Task The_bound_choice_is_pressed_marked_and_holds_the_tab_stop() {
        await using var context = CreateContext();
        var table = context.Render<PeriodicTable>(parameters => parameters.Add(component => component.SelectedNumber, 26));

        var iron = table.Find("button[data-symbol='Fe']");
        Assert.Equal("true", iron.GetAttribute("aria-pressed"));
        Assert.Equal("0", iron.GetAttribute("tabindex"));
        Assert.Single(table.FindAll("button[aria-pressed='true']"));
        Assert.Single(table.FindAll("button[tabindex='0']"));

        table.Render(parameters => parameters.Add(component => component.SelectedNumber, 79));
        Assert.Equal("true", table.Find("button[data-symbol='Au']").GetAttribute("aria-pressed"));
        Assert.Equal("0", table.Find("button[data-symbol='Au']").GetAttribute("tabindex"));
        Assert.Equal("false", table.Find("button[data-symbol='Fe']").GetAttribute("aria-pressed"));

        table.Render(parameters => parameters.Add(component => component.SelectedNumber, (int?)null));
        Assert.Empty(table.FindAll("button[aria-pressed='true']"));
        Assert.Empty(table.FindAll(".ui-periodic-table__check"));
        Assert.Equal("0", table.Find("button[data-symbol='Au']").GetAttribute("tabindex"));
    }

    [Fact]
    public async Task Arrow_keys_home_and_end_move_the_single_tab_stop_and_the_focus() {
        await using var context = CreateContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        var table = context.Render<PeriodicTable>();
        Assert.Single(module.Invocations, invocation => invocation.Identifier == "attach");

        Press(table, "ArrowDown", "ArrowDown", "ArrowDown");
        Press(table, Enumerable.Repeat("ArrowRight", 7).ToArray());
        AssertTabStop(table, module, "Fe");
        Press(table, "Home");
        AssertTabStop(table, module, "K");
        Press(table, "End");
        AssertTabStop(table, module, "Kr");
        Press(table, "ArrowRight");
        AssertTabStop(table, module, "Kr");
        Press(table, "ArrowUp", "ArrowUp", "ArrowUp", "ArrowUp");
        AssertTabStop(table, module, "He");
        Press(table, "ArrowLeft");
        AssertTabStop(table, module, "H");
        Press(table, "ArrowUp");
        AssertTabStop(table, module, "H");

        table.Find("button[data-symbol='Ba']").Click();
        AssertTabStop(table, module, "Ba");
        Press(table, "ArrowRight");
        AssertTabStop(table, module, "Hf");
        Press(table, "ArrowLeft", "ArrowDown", "ArrowDown");
        AssertTabStop(table, module, "La");
        Press(table, "End");
        AssertTabStop(table, module, "Lu");
        Press(table, "ArrowDown");
        AssertTabStop(table, module, "Lr");
        Press(table, "ArrowDown");
        AssertTabStop(table, module, "Lr");
        Press(table, "Home", "ArrowUp", "ArrowUp");
        AssertTabStop(table, module, "Ra");

        Press(table, new KeyboardEventArgs { Key = "ArrowRight", AltKey = true });
        AssertTabStop(table, module, "Ra");
    }

    [Fact]
    public async Task Enter_and_space_keep_the_native_click_instead_of_choosing_twice() {
        await using var context = CreateContext();
        var choices = new List<PeriodicTableChoice>();
        var table = context.Render<PeriodicTable>(parameters => parameters
            .Add(component => component.ElementChosen, (PeriodicTableChoice choice) => choices.Add(choice)));

        Press(table, "Enter", " ");

        Assert.Empty(choices);
        Assert.Empty(table.FindAll("button[aria-pressed='true']"));
    }

    [Fact]
    public async Task The_list_finds_elements_and_chooses_them_like_the_table() {
        await using var context = CreateContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        int? bound = null;
        var choices = new List<PeriodicTableChoice>();
        var table = context.Render<PeriodicTable>(parameters => parameters
            .Add(component => component.ElementTestIdPrefix, "ptable-")
            .Add(component => component.SelectedNumberChanged, (int? number) => bound = number)
            .Add(component => component.ElementChosen, (PeriodicTableChoice choice) => choices.Add(choice)));

        var search = table.Find("[data-testid='ptable-search']");
        var list = table.Find("[data-testid='ptable-list']");
        Assert.Equal("combobox", search.GetAttribute("role"));
        Assert.Equal("list", search.GetAttribute("aria-autocomplete"));
        Assert.Equal("false", search.GetAttribute("aria-expanded"));
        Assert.Equal(list.Id, search.GetAttribute("aria-controls"));
        Assert.Equal(search.Id, table.Find(".ui-periodic-table__find-label").GetAttribute("for"));
        Assert.Equal("listbox", list.GetAttribute("role"));
        Assert.True(list.HasAttribute("hidden"));

        search.Input("iron");

        Assert.Equal("true", table.Find("[data-testid='ptable-search']").GetAttribute("aria-expanded"));
        Assert.False(table.Find("[data-testid='ptable-list']").HasAttribute("hidden"));
        var option = Assert.Single(table.FindAll("[role='option']"));
        Assert.Equal("ptable-option-Fe", option.GetAttribute("data-testid"));
        Assert.Equal("26 Fe Iron", option.TextContent.Trim());
        Assert.Equal("false", option.GetAttribute("aria-selected"));

        table.Find("[data-testid='ptable-search']").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Equal("li[role=\"option\"][data-z=\"26\"]", LastFocus(module));

        table.Find("[data-testid='ptable-option-Fe']").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal(26, bound);
        var choice = Assert.Single(choices);
        Assert.Equal((26, PeriodicTableChoiceSource.List), (choice.Element.AtomicNumber, choice.Source));
        Assert.Equal("true", table.Find("[data-testid='ptable-option-Fe']").GetAttribute("aria-selected"));
        Assert.Equal("true", table.Find("[data-testid='ptable-Fe']").GetAttribute("aria-pressed"));
        Assert.Equal("0", table.Find("[data-testid='ptable-Fe']").GetAttribute("tabindex"));
        Assert.Equal("li[role=\"option\"][data-z=\"26\"]", LastFocus(module));

        table.Find("[data-testid='ptable-option-Fe']").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Equal("input[role=\"combobox\"]", LastFocus(module));
    }

    [Fact]
    public async Task Enter_in_the_field_chooses_the_first_match_and_the_arrows_walk_the_list() {
        await using var context = CreateContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        var choices = new List<PeriodicTableChoice>();
        var table = context.Render<PeriodicTable>(parameters => parameters
            .Add(component => component.ElementTestIdPrefix, "ptable-")
            .Add(component => component.States, new Dictionary<int, PeriodicTableElementState> { [106] = new(Available: false) })
            .Add(component => component.ElementChosen, (PeriodicTableChoice choice) => choices.Add(choice)));

        table.Find("[data-testid='ptable-search']").Input("s");

        var options = table.FindAll("[role='option']");
        Assert.Equal(
            ["S", "Na", "Si", "Sc", "Se", "Sr", "Ag", "Sm", "Sg"],
            options.Select(option => option.GetAttribute("data-testid")!["ptable-option-".Length..]).ToArray());
        var seaborgium = table.Find("[data-testid='ptable-option-Sg']");
        Assert.Equal("106 Sg Seaborgium (unavailable)", seaborgium.TextContent.Trim());
        Assert.Contains("ui-periodic-table__option--unavailable", seaborgium.ClassList);

        table.Find("[data-testid='ptable-search']").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        Assert.Equal(("S", PeriodicTableChoiceSource.List), (choices[^1].Element.Symbol, choices[^1].Source));

        table.Find("[data-testid='ptable-search']").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Equal("li[role=\"option\"][data-z=\"16\"]", LastFocus(module));
        table.Find("[data-testid='ptable-option-S']").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Equal("li[role=\"option\"][data-z=\"11\"]", LastFocus(module));
        // A key pressed before the focus arrived still counts from the option the previous key reached.
        table.Find("[data-testid='ptable-option-S']").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Equal("li[role=\"option\"][data-z=\"14\"]", LastFocus(module));
        table.Find("[data-testid='ptable-option-Si']").KeyDown(new KeyboardEventArgs { Key = " " });
        Assert.Equal(("Si", PeriodicTableChoiceSource.List), (choices[^1].Element.Symbol, choices[^1].Source));
        Assert.Equal("true", table.Find("[data-testid='ptable-option-Si']").GetAttribute("aria-selected"));
        Assert.Equal("✓", table.Find("[data-testid='ptable-option-Si'] .ui-periodic-table__option-check").TextContent);
        table.Find("[data-testid='ptable-option-Si']").KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });
        table.Find("[data-testid='ptable-option-Na']").KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });
        Assert.Equal("li[role=\"option\"][data-z=\"16\"]", LastFocus(module));
        table.Find("[data-testid='ptable-option-S']").KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });
        Assert.Equal("input[role=\"combobox\"]", LastFocus(module));

        table.Find("[data-testid='ptable-option-Sg']").Click();
        Assert.Equal(("Sg", PeriodicTableChoiceSource.List), (choices[^1].Element.Symbol, choices[^1].Source));
        Assert.Equal(3, choices.Count);
    }

    [Fact]
    public async Task Test_hooks_labels_and_root_attributes_are_configurable() {
        await using var context = CreateContext();
        var table = context.Render<PeriodicTable>(parameters => parameters
            .Add(component => component.ElementTestIdPrefix, "ptable-")
            .Add(component => component.UnavailableText, "not available for simulation")
            .Add(component => component.AriaLabel, "Elements")
            .Add(component => component.States, new Dictionary<int, PeriodicTableElementState> { [118] = new(Available: false) })
            .Add(component => component.Class, "my-table")
            .AddUnmatched("data-testid", "ptable"));

        var root = table.Find("[data-testid='ptable']");
        Assert.Contains("ui-periodic-table", root.ClassList);
        Assert.Contains("my-table", root.ClassList);
        Assert.Equal("Elements", table.Find("[role='group']").GetAttribute("aria-label"));
        Assert.Equal(118, table.FindAll("button[data-testid^='ptable-']").Count);
        Assert.Equal("Og", table.Find("[data-testid='ptable-Og']").GetAttribute("data-symbol"));
        Assert.Equal("118 Og Oganesson: not available for simulation", table.Find("[data-testid='ptable-Og']").GetAttribute("aria-label"));

        var withoutSearch = context.Render<PeriodicTable>(parameters => parameters.Add(component => component.ShowSearch, false));
        Assert.Empty(withoutSearch.FindAll("[role='combobox'], [role='listbox']"));
        Assert.Equal(118, withoutSearch.FindAll("button[data-z]").Count);
    }

    [Fact]
    public async Task The_module_is_released_on_disposal_and_a_refusing_test_renderer_breaks_nothing() {
        await using (var context = CreateContext()) {
            var module = context.JSInterop.SetupModule(ModulePath);
            context.Render<PeriodicTable>();
            await context.DisposeComponentsAsync();
            Assert.Single(module.Invocations, invocation => invocation.Identifier == "detach");
        }

        await using var strict = new BunitContext();
        int? bound = null;
        var table = strict.Render<PeriodicTable>(parameters => parameters
            .Add(component => component.SelectedNumberChanged, (int? number) => bound = number));
        table.Find("button[data-symbol='Fe']").Click();
        Press(table, "ArrowRight");

        Assert.Equal(26, bound);
        Assert.Equal("Co", Assert.Single(table.FindAll("button[tabindex='0']")).GetAttribute("data-symbol"));
    }

    [Fact]
    public void The_browser_module_exports_the_calls_the_component_makes() {
        var source = File.ReadAllText(Path.Combine(
            FindRepoRoot(), "src", "CanDoItAll.Components.BaseLib", "Components", "DataDisplay", "PeriodicTable.razor.js"));

        Assert.Contains("export function attach(root)", source, StringComparison.Ordinal);
        Assert.Contains("export function detach(root)", source, StringComparison.Ordinal);
        Assert.Contains("export function focus(root, selector)", source, StringComparison.Ordinal);
        Assert.Contains("['ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown', 'Home', 'End']", source, StringComparison.Ordinal);
        Assert.Contains("event.preventDefault()", source, StringComparison.Ordinal);
    }

    private static void AssertCell(string symbol, int row, int column, int period, int group, char block) {
        var element = PeriodicTableElements.BySymbol(symbol);
        Assert.NotNull(element);
        Assert.Equal((symbol, row, column, period, group, block), (element.Symbol, element.Row, element.Column, element.Period, element.Group, element.Block));
    }

    private static string[] Symbols(IEnumerable<PeriodicTableElement> elements)
        => elements.Select(element => element.Symbol).ToArray();

    private static void Press(IRenderedComponent<PeriodicTable> table, params string[] keys) {
        foreach (var key in keys) {
            Press(table, new KeyboardEventArgs { Key = key });
        }
    }

    // The key goes to the button holding the tab stop, where the browser's focus is.
    private static void Press(IRenderedComponent<PeriodicTable> table, KeyboardEventArgs args)
        => table.Find("button[tabindex='0']").KeyDown(args);

    private static void AssertTabStop(IRenderedComponent<PeriodicTable> table, BunitJSModuleInterop module, string symbol) {
        var tabStop = Assert.Single(table.FindAll("button[tabindex='0']"));
        Assert.Equal(symbol, tabStop.GetAttribute("data-symbol"));
        Assert.Equal($"button[data-z=\"{PeriodicTableElements.BySymbol(symbol)!.AtomicNumber}\"]", LastFocus(module));
    }

    private static string? LastFocus(BunitJSModuleInterop module)
        => module.Invocations.Last(invocation => invocation.Identifier == "focus").Arguments[1] as string;

    private static string FindRepoRoot() {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CanDoItAll.Components.slnx"))) {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("Could not locate the repository root.");
    }
}
