# Define the 7-Zip base URL
$baseUrl = 'https://7-zip.org/'

Write-Host "Fetching latest 7-Zip x64 installer URL..."

# Get the latest x64 installer link from the 7-Zip homepage
try {
    $response = Invoke-WebRequest -Uri $baseUrl
    $link = $response.Links |
        Where-Object {
            $_.outerHTML -match 'Download' -and
            $_.href -match '^a/.*-x64\.exe$'
        } |
        Select-Object -First 1

    if (-not $link -or -not $link.href) {
        throw "Could not find a valid x64 installer link on the 7-Zip website."
    }

    # Correct URL extraction
    $downloadUrl = "https://7-zip.org/$($link.href)"
    Write-Host "Found installer: $downloadUrl"
}
catch {
    Write-Error "Error retrieving download link: $_"
    exit 1
}

# Define installer path in temp directory
$installerFile = Split-Path $link.href -Leaf
$installerPath = Join-Path $env:TEMP $installerFile

# Download the installer
Write-Host "Downloading 7-Zip installer to $installerPath..."
try {
    Invoke-WebRequest -Uri $downloadUrl -OutFile $installerPath
}
catch {
    Write-Error "Failed to download the installer: $_"
    exit 1
}

# Run the installer silently
Write-Host "Starting silent installation..."
try {
    Start-Process -FilePath $installerPath -ArgumentList "/S" -Verb RunAs -Wait
    Write-Host "Installation complete."
}
catch {
    Write-Error "Failed to install 7-Zip: $_"
    exit 1
}

# Clean up
Write-Host "Removing installer..."
Remove-Item $installerPath -Force

Write-Host "Done."