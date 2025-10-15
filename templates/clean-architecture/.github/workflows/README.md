# CI/CD Workflows

This directory contains GitHub Actions workflows for automated testing, building, and deployment of the Clean Architecture solution.

## Overview

The CI/CD pipeline is separated into three main workflows:

1. **Continuous Integration (CI)** - `ci.yml`
2. **Continuous Deployment (CD)** - `cd.yml`
3. **Pull Request Validation** - `pr-validation.yml`

---

## 1. Continuous Integration (`ci.yml`)

**Triggers:**

- Push to `main`, `develop`, `feature/**` branches
- Pull requests to `main` or `develop`
- Manual trigger via `workflow_dispatch`

**Jobs:**

### Build Solution

- Restores NuGet packages
- Builds the solution in Release mode
- Caches NuGet packages for faster builds
- Uploads build artifacts

### Unit Tests

- Runs Domain unit tests
- Runs Application unit tests
- Collects code coverage
- Uploads test results and coverage reports

### Architecture Tests

- Validates architectural rules and dependencies
- Ensures Clean Architecture principles are maintained

### Integration Tests

- Uses **Testcontainers** to spin up SQL Server
- Runs integration tests against real database
- Tests full stack including EF Core

### E2E Tests

- Uses **Testcontainers** for database
- Tests complete user workflows
- Validates end-to-end functionality

### Code Quality & Security

- Runs `dotnet format` to check code formatting
- Scans for vulnerable NuGet packages
- Reports security issues

### Test Results Summary

- Aggregates all test results
- Provides summary in GitHub Actions UI

---

## 2. Continuous Deployment (`cd.yml`)

**Triggers:**

- Automatically after successful CI pipeline on `main` branch
- Manual trigger via `workflow_dispatch` with environment selection

**Jobs:**

### Build & Push Docker Image

- Builds Docker image using multi-stage Dockerfile
- Pushes to Docker Hub
- Tags with version, branch, and latest
- Runs Trivy security scanner on image
- Caches Docker layers for faster builds

### Deploy to Staging

- Deploys to staging environment
- Runs smoke tests
- Requires successful build

### Deploy to Production

- **Requires manual approval** via GitHub Environments
- Creates backup before deployment
- Deploys to production
- Runs smoke tests and verification
- Only runs after successful staging deployment

### Create GitHub Release

- Generates version tag (YYYY.MM.DD-commit)
- Creates GitHub release with changelog
- Lists all commits since last release
- Includes Docker image information

### Rollback

- Manual rollback capability
- Triggers on deployment failure
- Restores previous version

---

## 3. Pull Request Validation (`pr-validation.yml`)

**Triggers:**

- When PR is opened, synchronized, or reopened
- Targets `main` or `develop` branches

**Jobs:**

### Validate PR

- Validates PR title follows semantic conventions (feat, fix, docs, etc.)
- Checks for merge conflicts
- Validates file sizes

### Build and Test PR

- Builds solution
- Runs all tests
- Generates code coverage report
- Posts coverage comment on PR

### Code Quality Check

- Runs code formatting validation
- Ensures code standards are met

### Security Check

- Scans for vulnerable packages
- Fails PR if critical vulnerabilities found

### PR Summary

- Generates summary of all checks
- Provides quick overview of PR status

---

## Required GitHub Secrets

To use these workflows, configure the following secrets in your GitHub repository:

### Docker Hub (for CD pipeline)

```
DOCKER_USERNAME - Your Docker Hub username
DOCKER_PASSWORD - Your Docker Hub password or access token
```

### Optional Secrets

```
DEPLOYMENT_SSH_KEY - SSH key for deployment servers
SLACK_WEBHOOK_URL - For deployment notifications
```

---

## GitHub Environments

Configure the following environments in your repository settings:

### Staging Environment

- **Name:** `staging`
- **URL:** https://staging.yourapp.com
- **Protection rules:** None (auto-deploy)

### Production Environment

- **Name:** `production`
- **URL:** https://yourapp.com
- **Protection rules:**
  - ✅ Required reviewers (1-2 people)
  - ✅ Wait timer: 5 minutes
  - ✅ Limit to protected branches only

---

## Workflow Diagram

```
┌─────────────────┐
│  Push to main   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   CI Pipeline   │
│  (Build & Test) │
└────────┬────────┘
         │ success
         ▼
┌─────────────────┐
│  CD Pipeline    │
│ (Build Docker)  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Deploy Staging  │
└────────┬────────┘
         │ success
         ▼
┌─────────────────┐
│Deploy Production│
│(Manual Approval)│
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Create Release  │
└─────────────────┘
```

---

## Local Development

To test workflows locally, use [act](https://github.com/nektos/act):

```bash
# Install act
brew install act

# Test CI workflow
act -W .github/workflows/ci.yml

# Test specific job
act -j build

# Test with secrets
act -s DOCKER_USERNAME=myuser -s DOCKER_PASSWORD=mypass
```

---

## Best Practices

1. **Branch Protection**

   - Enable required status checks
   - Require PR reviews before merging
   - Require CI to pass before merge

2. **Semantic Commit Messages**

   - Use conventional commits: `feat:`, `fix:`, `docs:`, etc.
   - PR titles follow the same convention

3. **Code Coverage**

   - Maintain at least 80% code coverage
   - Review coverage reports in PRs

4. **Security**

   - Fix vulnerable packages before merging
   - Review Trivy scan results

5. **Deployment**
   - Always test in staging before production
   - Use manual approval for production
   - Keep rollback capability ready

---

## Troubleshooting

### Build Failures

- Check NuGet package compatibility
- Verify .NET SDK version
- Review error logs in Actions tab

### Test Failures

- Check if Testcontainers can start Docker
- Verify test isolation
- Review test logs

### Docker Build Failures

- Verify Dockerfile syntax
- Check base image availability
- Review Docker build logs

### Deployment Failures

- Verify secrets are configured
- Check environment URLs
- Review deployment logs
- Use rollback if needed

---

## Monitoring

Monitor your workflows:

- GitHub Actions tab shows all runs
- Email notifications on failure
- Slack integration (optional)
- Status badges in README

---

## Support

For issues with workflows:

1. Check Actions logs
2. Review this documentation
3. Open an issue in the repository
4. Contact DevOps team

---

**Last Updated:** 2025-10-13
