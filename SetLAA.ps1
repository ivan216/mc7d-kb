# Enable Large Address Aware flag for 32-bit executable to allow 4GB memory usage.
# Sets the IMAGE_FILE_LARGE_ADDRESS_AWARE bit (0x0020) in the PE header directly,
# so no external tools (editbin/corflags) are needed.
param([string]$TargetPath)

$bytes = [IO.File]::ReadAllBytes($TargetPath)
$peOffset = [BitConverter]::ToUInt32($bytes, 0x3C)
$bytes[$peOffset + 22] = $bytes[$peOffset + 22] -bor 0x20
[IO.File]::WriteAllBytes($TargetPath, $bytes)
Write-Host "Large Address Aware enabled (4GB memory limit)"
