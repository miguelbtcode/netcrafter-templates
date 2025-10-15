param(
    [Parameter(Mandatory = $false)]
    [string]$Version = "1.0.0-dev.$(Get-Date -Format 'yyyyMMddHHmmss')"
)

Write-Host "🔨 Building NetCrafter Templates..." -ForegroundColor Green
Write-Host "📦 Version: $Version" -ForegroundColor Cyan

# Step 1: Clean previous builds
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "packages/nuget/output") {
    Remove-Item -Recurse -Force "packages/nuget/output"
}
New-Item -ItemType Directory -Path "packages/nuget/output" -Force | Out-Null
Write-Host "✅ Output directory cleaned" -ForegroundColor Green

# Clean bin/obj folders in template
Write-Host "🧹 Cleaning template bin/obj folders..." -ForegroundColor Yellow
Get-ChildItem -Path "templates/clean-architecture" -Recurse -Directory -Include "bin","obj" | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "✅ Template bin/obj folders cleaned" -ForegroundColor Green

# Step 2: Validate template structure
Write-Host "🔍 Validating template structure..." -ForegroundColor Yellow
if (!(Test-Path "templates/clean-architecture")) {
    Write-Error "❌ Template directory not found"
    exit 1
}
if (!(Test-Path "templates/clean-architecture/.template.config/template.json")) {
    Write-Error "❌ template.json not found"
    exit 1
}
Write-Host "✅ Template structure validated" -ForegroundColor Green

# Step 3: Build template
Write-Host "🔨 Building template..." -ForegroundColor Yellow
Push-Location templates/clean-architecture
try {
    & dotnet restore
    if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed" }

    & dotnet build --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

    Write-Host "✅ Template built successfully" -ForegroundColor Green
} finally {
    Pop-Location
}

# Step 4: Run template tests
Write-Host "🧪 Running template tests..." -ForegroundColor Yellow
Push-Location templates/clean-architecture
try {
    $testProjects = Get-ChildItem -Recurse -Include "*.Tests.csproj", "*Test*.csproj" -ErrorAction SilentlyContinue
    if ($testProjects) {
        & dotnet test --configuration Release --no-build --verbosity normal
        if ($LASTEXITCODE -ne 0) { throw "dotnet test failed" }
        Write-Host "✅ Tests executed successfully" -ForegroundColor Green
    } else {
        Write-Host "⚠️  No test projects found, skipping tests" -ForegroundColor Yellow
    }
} finally {
    Pop-Location
}

# Step 5: Clean build artifacts
Write-Host "🧹 Cleaning build artifacts..." -ForegroundColor Yellow
Get-ChildItem -Path "templates/clean-architecture" -Recurse -Directory -Include "bin","obj" | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "✅ Build artifacts cleaned" -ForegroundColor Green

# Step 6: Create NuGet package
Write-Host "📦 Creating NuGet package..." -ForegroundColor Yellow
Push-Location packages/nuget
try {
    & nuget pack NetCrafter.Templates.nuspec -Version $Version -OutputDirectory output
    if ($LASTEXITCODE -ne 0) { throw "nuget pack failed" }

    Write-Host "✅ NuGet package created successfully" -ForegroundColor Green
} finally {
    Pop-Location
}

# Step 7: Test package locally
Write-Host "🧪 Testing package locally..." -ForegroundColor Yellow
$packageFile = "packages/nuget/output/NetCrafter.Templates.$Version.nupkg"

if (!(Test-Path $packageFile)) {
    Write-Error "❌ Package file not found for testing"
    exit 1
}

# Install package locally
& dotnet new install $packageFile
if ($LASTEXITCODE -ne 0) { throw "dotnet new install failed" }

# Verify template is available
$templates = & dotnet new list
Write-Host "Available templates:" -ForegroundColor Yellow
$templates | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }

if ($templates -match "clean-arch") {
    Write-Host "✅ Template 'clean-arch' is available" -ForegroundColor Green
} else {
    Write-Host "❌ Template 'clean-arch' not found" -ForegroundColor Red
    Write-Host "Available templates:" -ForegroundColor Yellow
    & dotnet new list
    exit 1
}

# Create test project
$testDir = "test-output"
if (Test-Path $testDir) { Remove-Item -Recurse -Force $testDir }
New-Item -ItemType Directory -Path $testDir | Out-Null
Push-Location $testDir
try {
    & dotnet new clean-arch -n TestProject
    if ($LASTEXITCODE -ne 0) { throw "dotnet new failed" }

    if ((Test-Path "TestProject") -and (Test-Path "TestProject/TestProject.sln")) {
        Write-Host "✅ Test project created successfully" -ForegroundColor Green
    } else {
        Write-Error "❌ Test project creation failed"
        exit 1
    }
} finally {
    Pop-Location
    Remove-Item -Recurse -Force $testDir
}

Write-Host "✅ Local package testing completed successfully" -ForegroundColor Green

# Step 8: Display package info
Write-Host "📦 Package Information:" -ForegroundColor Blue
Write-Host "   Package: NetCrafter.Templates"
Write-Host "   Version: $Version"
Write-Host "   Location: $packageFile"
$size = (Get-Item $packageFile).Length / 1MB
Write-Host ("   Size: {0:N2} MB" -f $size)
Write-Host "✅ Package ready for publishing!" -ForegroundColor Green

Write-Host "📦 Package ready for publishing: $packageFile" -ForegroundColor Cyan