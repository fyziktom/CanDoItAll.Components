using CanDoItAll.Components.Common;

namespace CanDoItAll.Components.Common.Tests;

public sealed class CssClassBuilderTests
{
    [Fact]
    public void Join_TrimsValuesAndSkipsBlankFragments()
    {
        var value = CssClassBuilder.Join(" card ", null, "", " gap-2 ");

        Assert.Equal("card gap-2", value);
    }

    [Fact]
    public void JoinStyles_TrimsTrailingSemicolonsAndReturnsNullForEmptyInput()
    {
        var value = CssClassBuilder.JoinStyles("display: flex;", " ", "gap: 0.5rem;");

        Assert.Equal("display: flex; gap: 0.5rem", value);
        Assert.Null(CssClassBuilder.JoinStyles(null, " "));
    }
}
