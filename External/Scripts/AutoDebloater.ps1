$webClient = New-Object System.Net.WebClient
$url1 = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/DebloaterTool.exe"
$filePath1 = "$env:TEMP\DebloaterTool.exe"
$webClient.DownloadFile($url1, $filePath1)
$arguments = "--skipEULA --restart=Y --noURLOpen --autoUAC --mode=A --browser=ungoogled"
Start-Process -FilePath $filePath1 -ArgumentList $arguments -Verb RunAs