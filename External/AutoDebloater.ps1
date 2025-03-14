$webClient = New-Object System.Net.WebClient
$url1 = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/DebloaterTool.exe"
$filePath1 = "$env:TEMP\DebloaterTool.exe"
$webClient.DownloadFile($url1, $filePath1)
$arguments = "--skipEULA --autoRestart --mode=A"
Start-Process -FilePath $filePath1 -ArgumentList $arguments -Verb RunAs