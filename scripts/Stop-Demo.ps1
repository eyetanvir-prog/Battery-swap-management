param(
    [string]$ProjectRoot = "D:\SandBox\AIUB-Uni-Project"
)

$ErrorActionPreference = "Stop"

$runtimeDir = Join-Path $ProjectRoot ".runtime"
$pidFiles = @(
    Join-Path $runtimeDir "api-demo.pid",
    Join-Path $runtimeDir "mvc-demo.pid"
)

foreach ($pidFile in $pidFiles) {
    if (-not (Test-Path $pidFile)) {
        continue
    }

    $pidValue = Get-Content -LiteralPath $pidFile -ErrorAction SilentlyContinue
    $parsedPid = 0
    if ([int]::TryParse($pidValue, [ref]$parsedPid)) {
        $process = Get-Process -Id $parsedPid -ErrorAction SilentlyContinue
        if ($null -ne $process) {
            Stop-Process -Id $process.Id -Force
            Write-Host "Stopped process $($process.Id)."
        }
    }

    Remove-Item -LiteralPath $pidFile -Force -ErrorAction SilentlyContinue
}

Write-Host "BatterySwap demo processes stopped."
