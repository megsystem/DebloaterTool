Set fso = CreateObject("Scripting.FileSystemObject")
Set shell = CreateObject("WScript.Shell")

' Current script path
currentPath = WScript.ScriptFullName

' Startup folder path
startupPath = shell.SpecialFolders("Startup") & "\ThankYou.vbs"

' Check if already in Startup folder
If LCase(currentPath) <> LCase(startupPath) Then
    ' Not in startup: copy itself there
    On Error Resume Next
    fso.CopyFile currentPath, startupPath, True
    On Error GoTo 0
Else
    ' Create a temp text file with the thank-you message
    tempPath = shell.ExpandEnvironmentStrings("%TEMP%") & "\ThankYou.txt"
    Set textFile = fso.CreateTextFile(tempPath, True)
    
    ' Write the message to the text file
    textFile.WriteLine "________     ______ ______            _____            ________           ______"
    textFile.WriteLine "___  __ \_______  /____  /___________ __  /_______________  __/______________  /"
    textFile.WriteLine "__  / / /  _ \_  __ \_  /_  __ \  __ `/  __/  _ \_  ___/_  /  _  __ \  __ \_  / "
    textFile.WriteLine "_  /_/ //  __/  /_/ /  / / /_/ / /_/ // /_ /  __/  /   _  /   / /_/ / /_/ /  /  "
    textFile.WriteLine "/_____/ \___//_.___//_/  \____/\__,_/ \__/ \___//_/    /_/    \____/\____//_/   "
    textFile.WriteLine "--------------------------------------------------------------------------------"
    textFile.WriteLine ""
    textFile.WriteLine "                Thank You for Using DebloaterTool!"
    textFile.WriteLine ""
    textFile.WriteLine "You've just taken the first step toward a cleaner, faster, and more efficient "
    textFile.WriteLine "Windows experience. Whether you removed bloatware, stopped annoying background "
    textFile.WriteLine "services, or gave your desktop a fresh new look — we hope your system now "
    textFile.WriteLine "runs smoother than ever. ⚡"
    textFile.WriteLine ""
    textFile.WriteLine "What you just did:"
    textFile.WriteLine "- 🗑️ Removed pre-installed bloatware"
    textFile.WriteLine "- 🚫 Disabled unnecessary background services"
    textFile.WriteLine "- ⚡ Improved system speed & responsiveness"
    textFile.WriteLine "- 🌐 Installed Ungoogled Chromium Browser"
    textFile.WriteLine "- 🖼️ Changed the wallpaper"
    textFile.WriteLine "- 🔧 Modified the boot logo"
    textFile.WriteLine "- 🛡️ Disabled Windows Defender"
    textFile.WriteLine "- 📡 Disabled Windows Update"
    textFile.WriteLine "- 🧬 Disabled Spectre and Meltdown mitigations"
    textFile.WriteLine "- 🛠️ Disabled unnecessary security features"
    textFile.WriteLine ""
    textFile.WriteLine "--------------------------------------------------------------------------------"
    textFile.WriteLine ""
    textFile.WriteLine "Have feedback, suggestions, or just want to say hi?"
    textFile.WriteLine "Visit: https://github.com/megsystem/DebloaterTool"
    textFile.WriteLine ""
    textFile.WriteLine "Stay snappy,"
    textFile.WriteLine "– The MEGSystem Team! (@_giovannigiannone)"
    textFile.Close

    ' Open the thank-you message in Notepad
    shell.Run "notepad.exe """ & tempPath & """", 1, False

    ' Delete itself from Startup
    On Error Resume Next
    fso.DeleteFile currentPath
    On Error GoTo 0
End If