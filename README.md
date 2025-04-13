# DebloaterTool 🚀🛠️

DebloaterTool is a powerful and lightweight Windows debloater designed to remove unnecessary bloatware, disable unwanted services, and optimize system performance with ease. 💨✨

![DebloaterTool UI](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/1.png)

## Features ✅
- 🗑️ **Remove pre-installed bloatware**
- 🚫 **Disable unnecessary background services**
- ⚡ **Improve system speed & responsiveness**

> [!WARNING]  
> * *Disabling essential services may cause system instability. Use with caution!* ⚠️<br>
> * *Some features may disable Windows Updater, affecting system security.* 🛑<br>
> * *It is recommended to create a system restore point before running the tool.* 🛟<br>

## Installation 📥

### Option 1: Using the `.exe` Version (For Ease of Use)

1. **Download the DebloaterTool.exe:**
   - Go to the [DebloaterTool Releases Page](https://github.com/megsystem/DebloaterTool/releases) and download `DebloaterTool.exe`.
   - Or download the experimental one: [`DebloaterTool.exe`](https://github.com/megsystem/DebloaterTool/blob/main/DebloaterTool.exe) *(Updated whenever the code changes)*.

2. **Run the executable:**
   - Double-click `DebloaterTool.exe` to start the application. Make sure to run it with **administrator privileges**.

### Option 2: Running the Command Using the `Windows Run Dialog`

1. **Open the Run Dialog:**
   - Press `Win + R` on your keyboard.

2. **Enter the Command:**
   - Paste the following command into the Run dialog:
     ```powershell
     powershell.exe -w hidden -Command "iex (iwr 'https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/DebloaterTool.ps1')"
     ```
      or use AutoDebloater `(Complete Mode)`
     ```powershell
     powershell.exe -w hidden -Command "iex (iwr 'https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/AutoDebloater.ps1')"
     ```

   - Press `Enter` to execute the command.

3. **Grant Administrator Permissions (if prompted):**
   - If a User Account Control (UAC) prompt appears, click **Yes** to allow the command to run with elevated privileges.

4. **Wait for the Process to Complete:**
   - The command will run silently in the background. Once finished, the DebloaterTool should be ready for use.

## Screenshots 📷
![DebloaterTool UI](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/1.png)
![DebloaterTool Process](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/2.png)
![Bootloader - WIN10](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win10.bootloader.png)
![Desktop - WIN10](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win10.desktop.png)
![Result - WIN10](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win10.result.png)
![Bootloader - WIN11](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win11.bootloader.png)
![Desktop - WIN11](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win11.desktop.png)
![Result - WIN11](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win11.result.png)

## Contributing 🤝
Pull requests are welcome! If you have suggestions for improvements, feel free to submit an issue or a PR.

## License 📜
This project is licensed under the MIT License. See `LICENSE` for details.

## Disclaimer 🛑
This tool is provided as-is with no guarantees. Use at your own risk!
