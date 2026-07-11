namespace CanDoItAll.Components.FileBrowser.Core.Tests;

public sealed class UriSafetyTests
{
    [Theory]
    [InlineData("https://files.example.test/view/spec.pdf")]
    [InlineData("HTTP://files.example.test/view/spec.pdf?download=1#page-2")]
    [InlineData("/viewer/spec")]
    [InlineData("viewer/spec")]
    [InlineData("./viewer/spec")]
    [InlineData("../viewer/spec")]
    [InlineData("?preview=true")]
    [InlineData("#details")]
    public void NormalizerAcceptsHttpAndSameHostRelativeTargets(string value)
    {
        Assert.Equal(value, FileBrowserUriNormalizer.Normalize($"  {value}  "));
    }

    [Theory]
    [InlineData("javascript:alert(document.domain)")]
    [InlineData("data:text/html,<script>alert(1)</script>")]
    [InlineData("vbscript:msgbox(1)")]
    [InlineData("file:///etc/passwd")]
    [InlineData("ftp://files.example.test/spec.pdf")]
    [InlineData("blob:https://files.example.test/id")]
    [InlineData("//attacker.example/file")]
    [InlineData("///attacker.example/file")]
    [InlineData("\\\\attacker.example\\file")]
    [InlineData("/\\attacker.example/file")]
    [InlineData("https:\\attacker.example\\file")]
    [InlineData("http:attacker.example/file")]
    [InlineData("http:///missing-host")]
    [InlineData("http://[invalid")]
    [InlineData("/viewer/has space")]
    [InlineData("/viewer/%")]
    public void NormalizerRejectsActiveAmbiguousAndMalformedTargets(string value)
    {
        Assert.Throws<ArgumentException>(() => FileBrowserUriNormalizer.Normalize(value));
    }

    [Fact]
    public void NormalizerRejectsControlCharacters()
    {
        Assert.Throws<ArgumentException>(() =>
            FileBrowserUriNormalizer.Normalize("https://files.example.test/view\u0000javascript:alert(1)"));
        Assert.Throws<ArgumentException>(() =>
            FileBrowserUriNormalizer.Normalize("/viewer/spec\r\nLocation:https://attacker.example"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizerTreatsMissingOptionalTargetsAsNull(string? value)
    {
        Assert.Null(FileBrowserUriNormalizer.Normalize(value));
    }

    [Fact]
    public void ItemValidatesBothProviderControlledLinkFields()
    {
        var key = TestFileBrowserFactory.Key("unsafe-link");

        Assert.Throws<ArgumentException>(() => new FileBrowserItem(
            key,
            null,
            "unsafe.txt",
            FileBrowserItemKind.File,
            FileBrowserItemCategory.Document,
            openUri: "javascript:alert(1)"));
        Assert.Throws<ArgumentException>(() => new FileBrowserItem(
            key,
            null,
            "unsafe.txt",
            FileBrowserItemKind.File,
            FileBrowserItemCategory.Document,
            downloadUri: "data:text/html,unsafe"));
    }

    [Fact]
    public void ActionResultValidatesNavigationTargetsFromConstructorAndFactory()
    {
        var safe = FileBrowserActionResult.Success(navigationUri: "/viewer/spec");

        Assert.Equal("/viewer/spec", safe.NavigationUri);
        Assert.Throws<ArgumentException>(() =>
            new FileBrowserActionResult(true, navigationUri: "javascript:alert(1)"));
        Assert.Throws<ArgumentException>(() =>
            FileBrowserActionResult.Success(navigationUri: "//attacker.example/file"));
    }
}
