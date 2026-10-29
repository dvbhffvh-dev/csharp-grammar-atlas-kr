param([ValidateSet('Debug', 'Release')][string]$Configuration = 'Release')

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

function Assert-Output {
    param([string]$AssemblyPath, [string]$ExpectedPath, [string]$Label)
    $lines = @(& dotnet $AssemblyPath)
    if ($LASTEXITCODE -ne 0) { throw "$Label failed with exit code $LASTEXITCODE" }
    $actual = ($lines -join "`n").TrimEnd("`r", "`n")
    $expected = ([IO.File]::ReadAllText($ExpectedPath) -replace "`r`n", "`n").TrimEnd("`r", "`n")
    if ($actual -cne $expected) {
        throw "$Label output mismatch.`nEXPECTED:`n$expected`nACTUAL:`n$actual"
    }
    Write-Host "PASS $Label (expected output)"
}

Push-Location $repoRoot
try {
    & dotnet build CSharpGrammarAtlas.sln --configuration $Configuration --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Solution build failed.' }

    $chapters = @(Get-ChildItem -LiteralPath (Join-Path $repoRoot 'chapters') -Directory | Sort-Object Name)
    if ($chapters.Count -ne 24) { throw "Expected 24 chapters, found $($chapters.Count)." }
    foreach ($chapter in $chapters) {
        $projects = @(Get-ChildItem -LiteralPath $chapter.FullName -Filter '*.csproj')
        if ($projects.Count -ne 1) { throw "Expected one project in $($chapter.Name)." }
        $projectName = $projects[0].BaseName
        $assembly = Join-Path $chapter.FullName "bin/$Configuration/net10.0/$projectName.dll"
        Assert-Output $assembly (Join-Path $chapter.FullName 'expected-output.txt') $chapter.Name
        $answerOutput = @(& dotnet $assembly --answers)
        if ($LASTEXITCODE -ne 0 -or $answerOutput.Count -eq 0) {
            throw "Exercise answers failed: $($chapter.Name)"
        }
        Write-Host "PASS $($chapter.Name) (exercise answers)"
    }

    $gradebookRoot = Join-Path $repoRoot 'projects/Gradebook'
    $gradebookAssembly = Join-Path $gradebookRoot "bin/$Configuration/net10.0/Gradebook.dll"
    Assert-Output $gradebookAssembly (Join-Path $gradebookRoot 'expected-output.txt') 'Gradebook'
    & dotnet $gradebookAssembly --self-test
    if ($LASTEXITCODE -ne 0) { throw 'Gradebook self-tests failed.' }
    Write-Host 'SUCCESS: 25 example outputs, 24 answer runs, and Gradebook self-tests verified.'
}
finally {
    Pop-Location
}
