using System.Globalization;

namespace CanDoItAll.Components.BaseLib;

/// <summary>
/// The 118 chemical elements with their place in the conventional 18-column periodic table, as drawn by
/// <see cref="PeriodicTable"/>. Symbols and English names are the current IUPAC ones (Nh, Mc, Ts, Og included).
/// </summary>
/// <remarks>
/// The layout is the conventional one of IUPAC's own table: group 3 of periods 6 and 7 holds the "57–71" and "89–103"
/// placeholders, and the fifteen lanthanoids (La to Lu) and fifteen actinoids (Ac to Lr) are shown in display rows 9 and
/// 10 under columns 3 to 17, with group 0 and block <c>'f'</c>.
/// </remarks>
public static class PeriodicTableElements
{
    private static readonly string[] Symbols =
    [
        "H", "He", "Li", "Be", "B", "C", "N", "O", "F", "Ne", "Na", "Mg", "Al", "Si", "P", "S", "Cl", "Ar", "K", "Ca",
        "Sc", "Ti", "V", "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn", "Ga", "Ge", "As", "Se", "Br", "Kr", "Rb", "Sr", "Y", "Zr",
        "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd", "In", "Sn", "Sb", "Te", "I", "Xe", "Cs", "Ba", "La", "Ce", "Pr", "Nd",
        "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu", "Hf", "Ta", "W", "Re", "Os", "Ir", "Pt", "Au", "Hg",
        "Tl", "Pb", "Bi", "Po", "At", "Rn", "Fr", "Ra", "Ac", "Th", "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm",
        "Md", "No", "Lr", "Rf", "Db", "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn", "Nh", "Fl", "Mc", "Lv", "Ts", "Og"
    ];

    private static readonly string[] Names =
    [
        "Hydrogen", "Helium", "Lithium", "Beryllium", "Boron", "Carbon", "Nitrogen", "Oxygen", "Fluorine", "Neon",
        "Sodium", "Magnesium", "Aluminium", "Silicon", "Phosphorus", "Sulfur", "Chlorine", "Argon", "Potassium", "Calcium",
        "Scandium", "Titanium", "Vanadium", "Chromium", "Manganese", "Iron", "Cobalt", "Nickel", "Copper", "Zinc",
        "Gallium", "Germanium", "Arsenic", "Selenium", "Bromine", "Krypton", "Rubidium", "Strontium", "Yttrium", "Zirconium",
        "Niobium", "Molybdenum", "Technetium", "Ruthenium", "Rhodium", "Palladium", "Silver", "Cadmium", "Indium", "Tin",
        "Antimony", "Tellurium", "Iodine", "Xenon", "Caesium", "Barium", "Lanthanum", "Cerium", "Praseodymium", "Neodymium",
        "Promethium", "Samarium", "Europium", "Gadolinium", "Terbium", "Dysprosium", "Holmium", "Erbium", "Thulium", "Ytterbium",
        "Lutetium", "Hafnium", "Tantalum", "Tungsten", "Rhenium", "Osmium", "Iridium", "Platinum", "Gold", "Mercury",
        "Thallium", "Lead", "Bismuth", "Polonium", "Astatine", "Radon", "Francium", "Radium", "Actinium", "Thorium",
        "Protactinium", "Uranium", "Neptunium", "Plutonium", "Americium", "Curium", "Berkelium", "Californium", "Einsteinium", "Fermium",
        "Mendelevium", "Nobelium", "Lawrencium", "Rutherfordium", "Dubnium", "Seaborgium", "Bohrium", "Hassium", "Meitnerium", "Darmstadtium",
        "Roentgenium", "Copernicium", "Nihonium", "Flerovium", "Moscovium", "Livermorium", "Tennessine", "Oganesson"
    ];

    // Other accepted names or spellings that the search matches as well.
    private static readonly Dictionary<int, string> OtherNames = new()
    {
        [13] = "Aluminum",
        [16] = "Sulphur",
        [55] = "Cesium",
        [74] = "Wolfram"
    };

    /// <summary>The 118 elements, ordered by atomic number (<c>All[z - 1]</c> is element <c>z</c>).</summary>
    public static IReadOnlyList<PeriodicTableElement> All { get; } = Build();

    /// <summary>The element with this atomic number, or null outside 1 to 118.</summary>
    public static PeriodicTableElement? ByNumber(int atomicNumber)
        => atomicNumber is >= 1 and <= 118 ? All[atomicNumber - 1] : null;

    /// <summary>The element with this symbol, ignoring case (no two symbols differ only in case), or null.</summary>
    public static PeriodicTableElement? BySymbol(string? symbol)
    {
        var text = symbol?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        foreach (var element in All)
        {
            if (string.Equals(element.Symbol, text, StringComparison.OrdinalIgnoreCase))
            {
                return element;
            }
        }

        return null;
    }

    /// <summary>
    /// The elements a search text names, as the <see cref="PeriodicTable"/> list shows them: the exact symbol first
    /// (ignoring case), then the exact atomic number, then every element whose name or other accepted name starts with
    /// the text, by atomic number; each element once, at most <paramref name="maxResults"/>. An empty text finds nothing.
    /// </summary>
    public static IReadOnlyList<PeriodicTableElement> Search(string? query, int maxResults = 12)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxResults);
        var text = query?.Trim();
        if (string.IsNullOrEmpty(text) || maxResults == 0)
        {
            return [];
        }

        var found = new List<PeriodicTableElement>(maxResults);
        AddOnce(found, BySymbol(text), maxResults);
        if (int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var number))
        {
            AddOnce(found, ByNumber(number), maxResults);
        }

        foreach (var element in All)
        {
            if (element.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase)
                || element.OtherName?.StartsWith(text, StringComparison.OrdinalIgnoreCase) == true)
            {
                AddOnce(found, element, maxResults);
            }
        }

        return found;
    }

    private static void AddOnce(List<PeriodicTableElement> found, PeriodicTableElement? element, int maxResults)
    {
        if (element is not null && found.Count < maxResults && !found.Contains(element))
        {
            found.Add(element);
        }
    }

    // Periods end at Z 2, 10, 18, 36, 54, 86 and 118. Periods 2 and 3 skip groups 3 to 12; periods 6 and 7 hold two
    // s-block elements, fifteen elements shown in rows 9 and 10 (lanthanum to lutetium, actinium to lawrencium, under
    // columns 3 to 17), then groups 4 to 18.
    private static PeriodicTableElement[] Build()
    {
        int[] periodEnds = [2, 10, 18, 36, 54, 86, 118];
        var elements = new PeriodicTableElement[118];
        int period = 1, periodStart = 1;
        for (var z = 1; z <= 118; z++)
        {
            while (z > periodEnds[period - 1])
            {
                periodStart = periodEnds[period - 1] + 1;
                period++;
            }

            var index = z - periodStart;
            var group = period switch
            {
                1 => z == 1 ? 1 : 18,
                2 or 3 => index < 2 ? index + 1 : index + 11,
                4 or 5 => index + 1,
                _ => index < 2 ? index + 1 : index < 17 ? 0 : index - 13
            };

            var element = group == 0
                ? new PeriodicTableElement(z, Symbols[z - 1], Names[z - 1], period, 0, 'f', period + 3, index + 1)
                : new PeriodicTableElement(z, Symbols[z - 1], Names[z - 1], period, group, BlockOf(z, group), period, group);
            elements[z - 1] = OtherNames.TryGetValue(z, out var otherName) ? element with { OtherName = otherName } : element;
        }

        return elements;
    }

    private static char BlockOf(int atomicNumber, int group)
        => atomicNumber == 2 || group <= 2 ? 's' : group >= 13 ? 'p' : 'd';
}
