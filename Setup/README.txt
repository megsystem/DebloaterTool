==================================================
                 DebloaterTool 
==================================================

NOTE:
This tool is intended for advanced users who understand the risks of modifying Windows components.
Always create a system restore point before running DebloaterTool to avoid potential issues.

DESCRIPTION:
DebloaterTool is a powerful and lightweight Windows debloater designed to remove unnecessary bloatware,
disable unwanted services, and optimize system performance with ease.

==================================================
                     FEATURES 
==================================================
- Remove pre-installed bloatware
  Get rid of unnecessary apps that come with Windows.

- Disable unnecessary background services
  Free up system resources by stopping unused services.

- Improve system speed & responsiveness
  Optimize settings for faster boot times and better performance.

- Install Ungoogled Chromium Browser
  A lightweight, privacy-focused alternative to Google Chrome.

- Change the wallpaper
  Refresh your desktop with a new look — a custom wallpaper included with the tool.

- Modify the boot logo
  Replace the default Windows boot logo with a custom one provided by the tool.

- Disable Windows Defender
  Turn off real-time protection for performance (use with caution).

- Disable Windows Update
  Stop automatic updates that can interrupt your workflow.

- Disable Spectre and Meltdown mitigations
  Boost performance on older CPUs by disabling certain security patches.

- Disable unnecessary security features
  Improve speed by turning off non-essential security layers.

WARNING:
- Disabling essential services may cause system instability. Use with caution!
- Some features disable Windows Update and Defender, affecting system security.
- It is recommended to create a system restore point before running the tool.

==================================================
                  INSTALLATION 
==================================================

Option 1: Using the Compiled Version (Ease of Use)
--------------------------------------------------
1. Download the Executable:
   - Visit the DebloaterTool Releases Page and download DebloaterTool.exe
   - Or download the experimental version from:
     https://github.com/megsystem/DebloaterTool/blob/main/DebloaterTool.exe

2. Run the Application:
   - Double-click DebloaterTool.exe to launch the tool.
   - Ensure you run it as an administrator.

Option 2: Running the Command Using the Command Line
----------------------------------------------------
1. Open Command Prompt, PowerShell, or Windows Run Dialog (Win + R).

2. Execute the Command:
   - Standard Mode:
     powershell.exe -w hidden -Command "iex (iwr 'https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Scripts/DebloaterTool.ps1')"
   - AutoDebloater (Complete Mode):
     powershell.exe -w hidden -Command "iex (iwr 'https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Scripts/AutoDebloater.ps1')"

3. Grant Administrator Permissions if prompted.

4. Wait for the Process to Complete.

==================================================
                     SCREENSHOTS 
==================================================
DebloaterTool Interface:
- UI Preview, Process, AlwaysOnTop, Take Ownership menu
- Windows 10 Bootloader, Desktop, Result
- Windows 11 Bootloader, Desktop, Result

(Images not included in TXT version)

==================================================
                 CONTRIBUTING 
==================================================
Pull requests are welcome! Submit issues or PRs for improvements.

==================================================
                     LICENSE 
==================================================
This project is licensed under the MIT License. See LICENSE file for details.

==================================================
                    DISCLAIMER 
==================================================
Use DebloaterTool at your own risk.
This tool is provided "as-is," without any warranty or guarantee of stability.
Disabling certain services might result in system issues.
Create a system restore point before usage.
