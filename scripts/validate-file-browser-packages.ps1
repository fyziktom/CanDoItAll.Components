param(
    [Parameter(Mandatory = $true)]
    [string]$PackagePath,

    [Parameter(Mandatory = $true)]
    [string]$PackageVersion
)

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.IO.Compression.FileSystem

$definitions = @(
    @{
        Id = "CanDoItAll.Components.FileBrowser.Core"
        RequiredEntries = @(
            "README.md",
            "CanDoItAll.Components.FileBrowser.Core.nuspec",
            "lib/net10.0/CanDoItAll.Components.FileBrowser.Core.dll",
            "lib/net10.0/CanDoItAll.Components.FileBrowser.Core.xml"
        )
        Dependencies = @()
    },
    @{
        Id = "CanDoItAll.Components.FileBrowser.Providers.FileSystem"
        RequiredEntries = @(
            "README.md",
            "CanDoItAll.Components.FileBrowser.Providers.FileSystem.nuspec",
            "lib/net10.0/CanDoItAll.Components.FileBrowser.Providers.FileSystem.dll",
            "lib/net10.0/CanDoItAll.Components.FileBrowser.Providers.FileSystem.xml"
        )
        Dependencies = @(
            "CanDoItAll.Components.FileBrowser.Core"
        )
    },
    @{
        Id = "CanDoItAll.Components.FileBrowser.BaseLib"
        RequiredEntries = @(
            "README.md",
            "CanDoItAll.Components.FileBrowser.BaseLib.nuspec",
            "lib/net10.0/CanDoItAll.Components.FileBrowser.BaseLib.dll",
            "lib/net10.0/CanDoItAll.Components.FileBrowser.BaseLib.xml",
            "staticwebassets/css/file-browser.css",
            "staticwebassets/Components/FileBrowserItemActions.razor.js",
            "buildTransitive/CanDoItAll.Components.FileBrowser.BaseLib.props"
        )
        Dependencies = @(
            "CanDoItAll.Components.BaseLib",
            "CanDoItAll.Components.FileBrowser.Core"
        )
    }
)

foreach ($definition in $definitions)
{
    $packageFile = Join-Path $PackagePath "$($definition.Id).$PackageVersion.nupkg"
    $symbolsFile = Join-Path $PackagePath "$($definition.Id).$PackageVersion.snupkg"
    if (-not (Test-Path -LiteralPath $packageFile -PathType Leaf))
    {
        throw "Expected package was not produced: $packageFile"
    }

    if (-not (Test-Path -LiteralPath $symbolsFile -PathType Leaf))
    {
        throw "Expected symbols package was not produced: $symbolsFile"
    }

    $archive = [System.IO.Compression.ZipFile]::OpenRead((Resolve-Path -LiteralPath $packageFile).Path)
    try
    {
        $entries = @($archive.Entries | ForEach-Object { $_.FullName })
        foreach ($requiredEntry in $definition.RequiredEntries)
        {
            if ($entries -notcontains $requiredEntry)
            {
                throw "$($definition.Id) is missing required package entry '$requiredEntry'."
            }
        }

        $nuspecEntry = $archive.Entries |
            Where-Object { $_.FullName -eq "$($definition.Id).nuspec" } |
            Select-Object -First 1
        $reader = [System.IO.StreamReader]::new($nuspecEntry.Open())
        try
        {
            $nuspec = $reader.ReadToEnd()
        }
        finally
        {
            $reader.Dispose()
        }

        foreach ($dependency in $definition.Dependencies)
        {
            $escapedDependency = [Regex]::Escape($dependency)
            if ($nuspec -notmatch "<dependency\s+id=`"$escapedDependency`"")
            {
                throw "$($definition.Id) is missing package dependency '$dependency'."
            }
        }

        $readmeEntry = $archive.Entries |
            Where-Object { $_.FullName -eq "README.md" } |
            Select-Object -First 1
        $reader = [System.IO.StreamReader]::new($readmeEntry.Open())
        try
        {
            $readme = $reader.ReadToEnd()
        }
        finally
        {
            $reader.Dispose()
        }

        if ($readme -match '\]\(\.\./')
        {
            throw "$($definition.Id) README contains a repository-relative link that will break at package root."
        }
    }
    finally
    {
        $archive.Dispose()
    }

    Write-Host "Validated package: $($definition.Id) $PackageVersion"
}
