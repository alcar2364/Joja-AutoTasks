---
issue_number: 159
legacy_id: ""
type: "Review follow-up"
title: "Review follow-up:"
summary: "A bare catch block in Configuration/ConfigLoader.cs (line 26) suppresses all exceptions without discrimination during config deserialization. This means fatal CLR exceptions (StackOverflowException, OutOfMemoryException, ThreadAbortException) and genuine deserialization bugs are silently swallowed, falling back to null as if the config file simply didn't exist."
created_phase: "Phase 3"
source: "#86 (merged historical source reference)"
scheduled_target: "Phase 4"
status: "Open"
priority: "High"
github_url: "https://github.com/alcar2364/Joja-AutoTasks/issues/159"
resolution_pr: ""
created_by: "alcar2364"
created_at: "2026-03-13T00:02:45Z"
updated_at: "2026-03-14T00:00:00Z"
sync_state: "github-synced"
notes: "Canonical active tracker for merged #86 scope; #86 retained as historical merged-reference only.

conversion of security scanner review into new issues system

Additional Context
The rest of the codebase (e.g. TaskIdFormat.cs) already follows the correct pattern of catching only ArgumentException—this file should be brought in line with that convention.
Secret pattern scan: no findings (no hardcoded credentials, API keys, or connection strings in any .cs file).
NuGet vulnerability scan: could not run — the sandbox environment blocks outbound NuGet.org access (proxy 403). CI now includes a warning-only `dotnet list package --vulnerable` step.
Detection Method
Manual SAST review (CodeQL equivalent static analysis) — security-and-quality query suite scope."
---

# Implementation Issue Record

## Rationale And Context

<html>
<body>
<!--StartFragment--><h2 dir="auto" style="box-sizing: border-box; margin-top: 24px; margin-bottom: 16px; font-size: 1.5em; font-weight: 600; line-height: 1.25; border-bottom: 1px solid rgba(61, 68, 77, 0.7); padding-bottom: 0.3em; color: rgb(240, 246, 252); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, &quot;Noto Sans&quot;, Helvetica, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; white-space: normal; background-color: rgb(13, 17, 23); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial;">Affected File</h2><p dir="auto" style="box-sizing: border-box; margin-top: 0px; margin-bottom: 16px; color: rgb(240, 246, 252); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, &quot;Noto Sans&quot;, Helvetica, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;; font-size: 14px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; white-space: normal; background-color: rgb(13, 17, 23); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial;"><strong style="box-sizing: border-box; font-weight: 600;"><code class="notranslate" style="box-sizing: border-box; font-family: &quot;Monaspace Neon&quot;, ui-monospace, SFMono-Regular, &quot;SF Mono&quot;, Menlo, Consolas, &quot;Liberation Mono&quot;, monospace; font-size: 11.9px; tab-size: 4; white-space: break-spaces; background-color: rgba(101, 108, 118, 0.2); border-radius: 6px; margin: 0px; padding: 0.2em 0.4em;">Configuration/ConfigLoader.cs</code>, line 26</strong></p><div class="highlight highlight-source-cs notranslate position-relative overflow-auto" dir="auto" style="box-sizing: border-box; position: relative !important; overflow: auto !important; margin-bottom: 16px; color: rgb(240, 246, 252); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, &quot;Noto Sans&quot;, Helvetica, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;; font-size: 14px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; white-space: normal; background-color: rgb(13, 17, 23); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial;"><pre class="notranslate" style="box-sizing: border-box; font-family: &quot;Monaspace Neon&quot;, ui-monospace, SFMono-Regular, &quot;SF Mono&quot;, Menlo, Consolas, &quot;Liberation Mono&quot;, monospace; font-size: 11.9px; margin-top: 0px; margin-bottom: 0px; tab-size: 4; overflow-wrap: normal; padding: 16px; color: rgb(240, 246, 252); background-color: rgb(21, 27, 35); border-radius: 6px; line-height: 1.45; overflow: auto; word-break: normal; min-height: 52px;"><span class="pl-k" style="box-sizing: border-box; color: rgb(255, 123, 114);">try</span>
<span class="pl-kos" style="box-sizing: border-box;">{</span>
    <span class="pl-s1" style="box-sizing: border-box;">loadedConfig</span> <span class="pl-c1" style="box-sizing: border-box; color: rgb(121, 192, 255);">=</span> <span class="pl-s1" style="box-sizing: border-box;">_helper</span><span class="pl-kos" style="box-sizing: border-box;">.</span><span class="pl-en" style="box-sizing: border-box; color: rgb(210, 168, 255);">ReadConfig</span><span class="pl-kos" style="box-sizing: border-box;">(</span><span class="pl-s1" style="box-sizing: border-box;">ModConfig</span><span class="pl-kos" style="box-sizing: border-box;">)</span><span class="pl-kos" style="box-sizing: border-box;">(</span><span class="pl-kos" style="box-sizing: border-box;">)</span><span class="pl-kos" style="box-sizing: border-box;">;</span>
<span class="pl-kos" style="box-sizing: border-box;">}</span>
<span class="pl-k" style="box-sizing: border-box; color: rgb(255, 123, 114);">catch</span>   <span class="pl-c" style="box-sizing: border-box; color: rgb(145, 152, 161);">// ← catches everything, including fatal CLR exceptions</span>
<span class="pl-kos" style="box-sizing: border-box;">{</span>
    <span class="pl-s1" style="box-sizing: border-box;">loadedConfig</span> <span class="pl-c1" style="box-sizing: border-box; color: rgb(121, 192, 255);">=</span> <span class="pl-c1" style="box-sizing: border-box; color: rgb(121, 192, 255);">null</span><span class="pl-kos" style="box-sizing: border-box;">;</span>
<span class="pl-kos" style="box-sizing: border-box;">}</span></pre><div class="zeroclipboard-container position-absolute right-0 top-0" style="box-sizing: border-box; position: absolute !important; top: 0px !important; right: 0px !important; margin-bottom: 0px;"><clipboard-copy aria-label="Copy" class="ClipboardButton btn js-clipboard-copy m-2 p-0" data-copy-feedback="Copied!" data-tooltip-direction="w" value="try
{
    loadedConfig = _helper.ReadConfig(ModConfig)();
}
catch   // ← catches everything, including fatal CLR exceptions
{
    loadedConfig = null;
}" tabindex="0" role="button" style="box-sizing: border-box; padding: 0px !important; font-size: 14px; font-weight: 500; white-space: nowrap; vertical-align: middle; cursor: pointer; user-select: none; appearance: none; border: 1px solid rgb(61, 68, 77); border-radius: 6px; line-height: 20px; display: inline-block; position: relative; color: rgb(240, 246, 252); background-color: rgb(33, 40, 48); box-shadow: none; transition: color 80ms cubic-bezier(0.33, 1, 0.68, 1), background-color 80ms cubic-bezier(0.33, 1, 0.68, 1), box-shadow 80ms cubic-bezier(0.33, 1, 0.68, 1), border-color 80ms cubic-bezier(0.33, 1, 0.68, 1); margin: 8px !important;"><svg aria-hidden="true" height="16" viewBox="0 0 16 16" version="1.1" width="16" data-view-component="true" class="octicon octicon-copy js-clipboard-copy-icon m-2"></svg></clipboard-copy></div></div><h2 dir="auto" style="box-sizing: border-box; margin-top: 24px; margin-bottom: 16px; font-size: 1.5em; font-weight: 600; line-height: 1.25; border-bottom: 1px solid rgba(61, 68, 77, 0.7); padding-bottom: 0.3em; color: rgb(240, 246, 252); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, &quot;Noto Sans&quot;, Helvetica, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; white-space: normal; background-color: rgb(13, 17, 23); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial;">Risk</h2><markdown-accessiblity-table data-catalyst="" style="box-sizing: border-box; display: block; color: rgb(240, 246, 252); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, &quot;Noto Sans&quot;, Helvetica, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;; font-size: 14px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; white-space: normal; background-color: rgb(13, 17, 23); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial;">
Aspect | Detail
-- | --
OWASP category | A08 – Software and Data Integrity Failures
Severity | High (per security scanning policy)
Impact | Fatal CLR exceptions are masked; actual deserialization failures are indistinguishable from corrupt-config fallback; security-relevant errors during config load cannot be detected or logged

</markdown-accessiblity-table><p dir="auto" style="box-sizing: border-box; margin-top: 0px; margin-bottom: 16px; color: rgb(240, 246, 252); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, &quot;Noto Sans&quot;, Helvetica, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;; font-size: 14px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; white-space: normal; background-color: rgb(13, 17, 23); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial;">Concretely:</p><ul dir="auto" style="box-sizing: border-box; margin-top: 0px; margin-bottom: 16px; padding-left: 2em; color: rgb(240, 246, 252); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, &quot;Noto Sans&quot;, Helvetica, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;; font-size: 14px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; white-space: normal; background-color: rgb(13, 17, 23); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial;"><li style="box-sizing: border-box;">A malformed config file that triggers a non-JSON exception (e.g. a stack overflow in a custom converter, or an<span> </span><code class="notranslate" style="box-sizing: border-box; font-family: &quot;Monaspace Neon&quot;, ui-monospace, SFMono-Regular, &quot;SF Mono&quot;, Menlo, Consolas, &quot;Liberation Mono&quot;, monospace; font-size: 11.9px; tab-size: 4; white-space: break-spaces; background-color: rgba(101, 108, 118, 0.2); border-radius: 6px; margin: 0px; padding: 0.2em 0.4em;">OutOfMemoryException</code><span> </span>on a huge payload) is silently discarded and replaced with defaults — the player and developer see no error.</li><li style="box-sizing: border-box; margin-top: 0.25em;">Any future deserialization code added to this path (e.g. reading player data, save-file state) would inherit this silent-failure behaviour.</li></ul><!--EndFragment-->

</body>
</html>

## Impact

Recommended Fix Catch only the specific, expected exception types that IModHelper.ReadConfig(T)() can realistically raise for a corrupt/missing config file. Since SMAPI uses Newtonsoft.Json internally, the narrowest safe catch is:

try { loadedConfig = \_helper.ReadConfig(ModConfig)(); } catch (Exception ex) when (ex is not (StackOverflowException or OutOfMemoryException or ThreadAbortException)) { \_logger.Log($"Failed to load config, reverting to defaults: {ex.Message}", LogLevel.Warn); loadedConfig = null; } Or, if logging in Load() is not yet available, at minimum restrict to expected exception types:

catch (Exception ex) when (ex is System.IO.IOException or Newtonsoft.Json.JsonException or InvalidOperationException) { loadedConfig = null;

## Implementation Notes

No response.

## Acceptance / Closing Criteria

will be resolved in phase 4 checklist

## History / Resolution Notes

- Canonical active tracker for merged scope previously represented by #86.
- #86 remains historical-only and non-active.
