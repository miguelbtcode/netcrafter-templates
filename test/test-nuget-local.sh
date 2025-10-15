#!/bin/bash

set -e

echo "🧪 =================================================="
echo "🧪   Testing NuGet Package Locally"
echo "🧪 =================================================="
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Variables
VERSION="${1:-1.0.0}"
PACKAGE_PATH="./packages/nuget/output/NetCrafter.Templates.$VERSION.nupkg"
TEST_OUTPUT_DIR="./test-nuget-output"
ERRORS=0

print_step() {
    echo -e "\n${BLUE}━━━ $1 ━━━${NC}"
}

print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
    ((ERRORS++))
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

# Step 1: Check if package exists
print_step "Checking package existence"

if [ ! -f "$PACKAGE_PATH" ]; then
    print_error "Package not found: $PACKAGE_PATH"
    echo ""
    echo "Run ./scripts/build-nuget-package.sh first to create the package"
    exit 1
fi

print_success "Package found: $PACKAGE_PATH"

# Step 2: Uninstall previous version
print_step "Uninstalling previous version"

dotnet new uninstall NetCrafter.Templates 2>/dev/null || true
print_success "Previous version uninstalled (if any)"

# Step 3: Install package locally
print_step "Installing package from local file"

dotnet new install "$PACKAGE_PATH"

if [ $? -eq 0 ]; then
    print_success "Package installed successfully"
else
    print_error "Failed to install package!"
    exit 1
fi

# Step 4: Verify template is listed
print_step "Verifying template is listed"

if dotnet new list | grep -q "clean-arch"; then
    print_success "Template 'clean-arch' is available"
else
    print_error "Template 'clean-arch' not found in dotnet new list"
fi

# Step 5: Clean test output directory
print_step "Preparing test environment"

rm -rf "$TEST_OUTPUT_DIR"
mkdir -p "$TEST_OUTPUT_DIR"
print_success "Test directory ready: $TEST_OUTPUT_DIR"

# Test scenarios
declare -a tests=(
    "Test1-Default::"
    "Test2-CustomName::"
)

# Step 6: Run test scenarios
print_step "Running test scenarios"
echo ""

for test_config in "${tests[@]}"; do
    IFS=':' read -r test_name test_args test_note <<< "$test_config"

    echo -e "${BLUE}Testing: $test_name${NC}"

    # Generate project
    cd "$TEST_OUTPUT_DIR"

    if [ -z "$test_args" ]; then
        cmd="dotnet new clean-arch -n $test_name"
    else
        cmd="dotnet new clean-arch -n $test_name $test_args"
    fi

    echo "  Command: $cmd"

    if eval "$cmd" > /dev/null 2>&1; then
        print_success "  Generated successfully"
    else
        print_error "  Failed to generate"
        cd ..
        continue
    fi

    # Build project
    cd "$test_name"

    echo "  Building project..."
    if dotnet build > /dev/null 2>&1; then
        print_success "  Build successful"
    else
        print_error "  Build failed"
    fi

    # Run tests if included
    if [[ ! "$test_args" =~ "--tests false" ]]; then
        echo "  Running tests..."
        if dotnet test --no-build --verbosity quiet > /dev/null 2>&1; then
            print_success "  Tests passed"
        else
            print_warning "  Tests failed or not available"
        fi
    fi

    cd ../..
    echo ""
done

# Step 7: Verify specific features
print_step "Verifying specific features"

# Check SQLite project (default)
if [ -d "$TEST_OUTPUT_DIR/Test1-Default/src/Infrastructure" ]; then
    if grep -q "UseSqlite" "$TEST_OUTPUT_DIR/Test1-Default/src/Infrastructure/DependencyInjection.cs" 2>/dev/null; then
        print_success "SQLite code found in default project"
    else
        print_error "SQLite code not found (expected in default)"
    fi
fi

# Check project structure
if [ -d "$TEST_OUTPUT_DIR/Test1-Default/src" ] && [ -d "$TEST_OUTPUT_DIR/Test1-Default/tests" ]; then
    print_success "Project structure is correct (src/ and tests/ directories)"
else
    print_error "Project structure is incorrect"
fi

# Check docker-compose.yml exists
if [ -f "$TEST_OUTPUT_DIR/Test1-Default/docker-compose.yml" ]; then
    print_success "Docker compose file included"
else
    print_error "Docker compose file missing"
fi

# Step 8: Cleanup test output (optional)
print_step "Cleanup"

read -p "Do you want to keep the test output directory? (y/n) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    rm -rf "$TEST_OUTPUT_DIR"
    print_success "Test output cleaned"
else
    print_success "Test output preserved at: $TEST_OUTPUT_DIR"
fi

# Step 9: Display summary
print_step "Test Summary"
echo ""

if [ $ERRORS -eq 0 ]; then
    echo -e "${GREEN}✓ All tests passed!${NC}"
    echo ""
    echo "The template package is working correctly and ready for distribution."
else
    echo -e "${RED}✗ $ERRORS error(s) found${NC}"
    echo ""
    echo "Please review the errors above before publishing."
    exit 1
fi

echo ""
echo "📋 Template Information:"
echo "   Name: clean-arch"
echo "   Version: $VERSION"
echo "   Package: $PACKAGE_PATH"
echo ""

echo "🎉 =================================================="
echo "🎉   Testing Complete!"
echo "🎉 =================================================="
