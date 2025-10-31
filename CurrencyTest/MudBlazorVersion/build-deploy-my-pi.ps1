Write-Host "Directory listing..."
Get-ChildItem  -Name

# First build the project files using arm64
Write-Host "Building project files for ARM64..."
docker build -t currency-mudblazor -f dockerfile-arm64 .
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed for ARM64."
    exit $LASTEXITCODE
}
