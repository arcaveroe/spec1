$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$changesRoot = Join-Path $repositoryRoot 'openspec\changes'
$specsRoot = Join-Path $repositoryRoot 'openspec\specs'
$featuresRoot = Join-Path $repositoryRoot 'FirmaDocs.Tests\Features'

New-Item -ItemType Directory -Path $featuresRoot -Force | Out-Null
Get-ChildItem -Path $featuresRoot -File |
    Where-Object { $_.Extension -eq '.feature' -or $_.Name -like '*.feature.cs' } |
    Remove-Item -Force

$specFiles = @()
$specFiles += Get-ChildItem -Path $specsRoot -Recurse -File -Filter 'spec.md'
$changeDirectories = Get-ChildItem -Path $changesRoot -Directory |
    Where-Object { $_.Name -ne 'archive' }
foreach ($changeDirectory in $changeDirectories) {
    $specFiles += Get-ChildItem -Path $changeDirectory.FullName -Recurse -File -Filter 'spec.md'
}

$featureNumbers = @{}
foreach ($specFile in ($specFiles | Sort-Object FullName)) {
    $content = Get-Content -Path $specFile.FullName -Raw -Encoding UTF8
    $capability = Split-Path -Leaf $specFile.DirectoryName
    $featureBlocks = [regex]::Matches(
        $content,
        '(?ms)^```gherkin[ \t]*\r?\n(.*?)^```[ \t]*\r?$'
    )

    foreach ($featureBlock in $featureBlocks) {
        if (-not $featureNumbers.ContainsKey($capability)) {
            $featureNumbers[$capability] = 0
        }

        $featureNumbers[$capability]++
        $featurePath = Join-Path $featuresRoot "$capability-$($featureNumbers[$capability]).feature"
        Set-Content -Path $featurePath -Value $featureBlock.Groups[1].Value -Encoding UTF8
    }
}
