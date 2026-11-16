namespace CanDoItAll.Components.BaseLib;

/// <summary>One chemical element and its cell in the conventional 18-column periodic table.</summary>
/// <param name="AtomicNumber">The atomic number Z, 1 to 118.</param>
/// <param name="Symbol">The IUPAC symbol, such as <c>Fe</c>.</param>
/// <param name="Name">The IUPAC English name, such as <c>Iron</c> (IUPAC spellings: aluminium, caesium, sulfur).</param>
/// <param name="Period">The period, 1 to 7.</param>
/// <param name="Group">
/// The group, 1 to 18; 0 for the lanthanoids (La to Lu) and actinoids (Ac to Lr) shown in the two rows below the main
/// table.
/// </param>
/// <param name="Block">
/// The block, <c>'s'</c>, <c>'p'</c>, <c>'d'</c> or <c>'f'</c>. Helium is <c>'s'</c>; the fifteen elements of each of
/// the two rows below the main table are <c>'f'</c>.
/// </param>
/// <param name="Row">
/// The display row: 1 to 7 for the periods, 9 for the lanthanoids and 10 for the actinoids (row 8 is the small gap above
/// them).
/// </param>
/// <param name="Column">The display column, 1 to 18: the group in the main table, 3 to 17 in rows 9 and 10.</param>
public sealed record PeriodicTableElement(
    int AtomicNumber,
    string Symbol,
    string Name,
    int Period,
    int Group,
    char Block,
    int Row,
    int Column)
{
    /// <summary>
    /// Another accepted name or spelling that the search also matches (Aluminum, Sulphur, Cesium, Wolfram); null for
    /// most elements.
    /// </summary>
    public string? OtherName { get; init; }
}

/// <summary>What the host says about one element of a <see cref="PeriodicTable"/>.</summary>
/// <param name="Available">
/// False marks the element unavailable: it is dashed, hatched and marked with a cross, and its button carries
/// <c>aria-disabled="true"</c>. It stays focusable and choosable; the host decides what choosing it means.
/// </param>
/// <param name="Note">
/// Optional explanation, such as why the element is unavailable or what is known about it. It is part of the element's
/// accessible name and tooltip.
/// </param>
public sealed record PeriodicTableElementState(bool Available = true, string? Note = null);

/// <summary>An element chosen in a <see cref="PeriodicTable"/> and where it was chosen.</summary>
/// <param name="Element">The chosen element.</param>
/// <param name="Source">The table itself or its searchable list.</param>
public sealed record PeriodicTableChoice(PeriodicTableElement Element, PeriodicTableChoiceSource Source);

/// <summary>Where a <see cref="PeriodicTable"/> element was chosen.</summary>
public enum PeriodicTableChoiceSource
{
    /// <summary>An element button of the table: a click, Enter or Space.</summary>
    Table,

    /// <summary>The searchable list: a click, Enter or Space on an option, or Enter in the search field.</summary>
    List
}
