param(
    [string]$ProjectRoot = "D:\SandBox\AIUB-Uni-Project",
    [int]$ApiPort = 8081,
    [int]$MvcPort = 8080
)

$ErrorActionPreference = "Stop"

$runtimeDir = Join-Path $ProjectRoot ".runtime"
$null = New-Item -ItemType Directory -Force -Path $runtimeDir

$apiOut = Join-Path $runtimeDir "api-demo.log"
$mvcOut = Join-Path $runtimeDir "mvc-demo.log"
$apiPidPath = Join-Path $runtimeDir "api-demo.pid"
$mvcPidPath = Join-Path $runtimeDir "mvc-demo.pid"

foreach ($path in @($apiOut, $mvcOut, $apiPidPath, $mvcPidPath)) {
    if (Test-Path $path) {
        Remove-Item -LiteralPath $path -Force
    }
}

$api = Start-Process dotnet -ArgumentList "run --project .\BatterySwap.API\BatterySwap.API.csproj --no-launch-profile --urls http://localhost:$ApiPort" `
    -WorkingDirectory $ProjectRoot `
    -RedirectStandardOutput $apiOut `
    -RedirectStandardError $apiOut `
    -PassThru

$mvc = Start-Process dotnet -ArgumentList "run --project .\BatterySwap.MVC\BatterySwap.MVC.csproj --no-launch-profile --urls http://localhost:$MvcPort" `
    -WorkingDirectory $ProjectRoot `
    -RedirectStandardOutput $mvcOut `
    -RedirectStandardError $mvcOut `
    -PassThru

$api.Id | Set-Content -Path $apiPidPath
$mvc.Id | Set-Content -Path $mvcPidPath

Write-Host "BatterySwap demo processes started."
Write-Host "MVC: http://localhost:$MvcPort"
Write-Host "API Swagger: http://localhost:$ApiPort"
Write-Host "API Log: $apiOut"
Write-Host "MVC Log: $mvcOut"
Write-Host "Use scripts\Stop-Demo.ps1 to stop both processes."
