---
name: release-packaging
description: "Automated mod release packaging: builds release zip and creates GitHub Release on version tag."
on:
  push:
    tags:
      - "v*.*.*"
  workflow_dispatch:
    inputs:
      version:
        description: "Version to release (e.g. 0.2.0)"
        required: true
        type: string
permissions:
  contents: read
  issues: read
  pull-requests: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
tools:
  github:
    toolsets: [default]
  dotnet: {}
safe-outputs:
  noop:
    report-as-issue: false
  create-issue:
    title-prefix: "[release] "
    labels: [agentic-workflow, release, needs-review]
    max: 1
---

# Release Packaging — Automated Mod Distribution

Build a release zip artifact and create a GitHub Release whenever a version tag is pushed.

## Context

- Repository: `${{ github.repository }}`
- Tag: `${{ github.event.release.tag_name }}`
- Manifest: `manifest.json`
- Main project: `JojaAutoTasks.csproj`
- Release build command: `dotnet build JojaAutoTasks.csproj -c Release -p:EnableModDeploy=false -p:EnableModZip=true`
- Output location: `bin/Release/net6.0/JojaAutoTasks <VERSION>.zip`

## Pipeline Steps

1. **Validate tag** format matches `vMAJOR.MINOR.PATCH`
2. **Extract version** from tag (strip leading `v`)
3. **Validate manifest.json:**
   - All required fields present (`Name`, `Author`, `Version`, `UniqueID`, `EntryDll`, `MinimumApiVersion`)
   - No `%ProjectVersion%` placeholder present (must be replaced before tagging)
   - `MinimumApiVersion` ≥ 4.4.0
4. **Validate csproj version** matches tag version
5. **Build** release zip with `EnableModZip=true`
6. **Verify zip** contains the expected mod files
7. **Create release issue** via `safe-outputs.create-issue`:
   - Title: `JojaAutoTasks v<VERSION> — Release Ready`
   - Body: Auto-generated changelog from commits since last tag, zip artifact name, and instructions for the maintainer to manually create the GitHub Release and attach the zip
   - Include release checklist and tag reference
   - Note: GitHub Release creation requires a maintainer to publish manually after reviewing the issue

## Changelog Generation

Auto-generate release notes from commits since the previous tag:

- Commits with `feat:` prefix → **New Features** section
- Commits with `fix:` prefix → **Bug Fixes** section
- Commits with `tests:`, `docs:`, `refactor:` prefixes → **Other Changes** section
- Skip maintenance/agentic-workflow commits

## Failure Behavior

- **Manifest validation fails:** Block release, create issue with validation errors
- **Version mismatch (tag vs csproj):** Block release, create issue
- **Build fails:** Block release, create issue
- **Zip verification fails:** Block release, create issue

## Notes

- Release preparation should happen on a short-lived `release/*` branch cut from `development`
- Create the release tag from the promoted `main` commit after the release branch is merged to stable
- Hotfix tags should likewise be created from the promoted `main` commit
- Workflow must be triggered from the tagged `main` lineage (not from a detached HEAD)
- Tags must follow semantic versioning (`v0.1.0`, `v1.0.0-beta.1`)
- Release drafts require manual publish; full tags auto-publish
- GitHub Release creation is a manual step; this workflow creates a `[release]` issue with all details for the maintainer to act on
- The NuGet package cache should be warmed before this workflow runs (use pr-ci cache)
- Stardew Valley Nexus Mods distribution can be done manually after GitHub Release is created
