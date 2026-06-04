# Enable Large Address Aware flag for 32-bit executable to allow 4GB memory usage
param([string]$TargetPath)

# Use vswhere.exe to find Visual Studio installation
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (Test-Path $vswhere) {
    $vsPath = & $vswhere -latest -property installationPath 2>$null
    if ($vsPath) {
        $vcTools = Join-Path $vsPath "VC\Tools\MSVC"
        if (Test-Path $vcTools) {
            $latestVersion = Get-ChildItem $vcTools | Sort-Object Name -Descending | Select-Object -First 1
            if ($latestVersion) {
                $editbin = Join-Path $latestVersion.FullName "bin\Hostx64\x86\editbin.exe"
                if (Test-Path $editbin) {
                    & $editbin /LARGEADDRESSAWARE $TargetPath | Out-Null
                    if ($LASTEXITCODE -eq 0) {
                        Write-Host "Large Address Aware enabled (4GB memory limit)"
                    }
                }
            }
        }
    }
}
