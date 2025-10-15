param(
    [Parameter(Mandatory = $true)]
    [string]$NuGetApiKey,

    [Parameter(Mandatory = $false)]
    [string]$Version = "1.0.0-dev.$(Get-Date -Format 'yyyyMMddHHmmss')"
)

Write-Host "🚀 Publishing NetCrafter Templates..." -ForegroundColor Green
Write-Host "📦 Version: $Version" -ForegroundColor Cyan

$packageFile = "packages/nuget/output/NetCrafter.Templates.$Version.nupkg"

if (!(Test-Path $packageFile)) {
    Write-Error "❌ Package file not found: $packageFile"
    exit 1
}

Write-Host "📤 Pushing package to NuGet..." -ForegroundColor Yellow

try {
    & dotnet nuget push $packageFile --api-key $NuGetApiKey --source https://api.nuget.org/v3/index.json --skip-duplicate
    if ($LASTEXITCODE -ne 0) { throw "dotnet nuget push failed" }

    Write-Host "✅ Package published successfully to NuGet.org" -ForegroundColor Green
    Write-Host "🔗 Package URL: https://www.nuget.org/packages/NetCrafter.Templates/$Version" -ForegroundColor Cyan
} catch {
    Write-Error "❌ Failed to publish package: $_"
    exit 1
}