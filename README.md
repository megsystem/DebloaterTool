![BANNER](https://raw.githubusercontent.com/megsystem/megsystem/refs/heads/main/banner.png)
> [!NOTE]  
> This tool is intended for **advanced users** who understand the risks of modifying Windows components.  
> Always create a **system restore point** before running DebloaterTool to avoid potential issues.

# DebloaterTool 🚀🛠️

DebloaterTool is a powerful and lightweight Windows debloater designed to remove unnecessary bloatware, disable unwanted services, and optimize system performance with ease. 💨✨

![DebloaterTool UI](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/1.png)

---

## Table of Contents
- [Features ✨](#features) (_Explore the core functionalities like bloatware removal and service optimization._)
- [Installation 📥](#installation) (_Step-by-step methods to install DebloaterTool on your system._)
  - [Compiled Version 🔧](#compiled-version) (_Download and run the executable for a simple installation process._)
  - [Running the Command 🚀](#use-command-line) (_Quick installation with command execution._)
- [Screenshots 📸](#screenshots) (_Visual previews of DebloaterTool in action._)
  - [DebloaterTool Interface 🧰](#debloatertool-interface) (_See the user interface and design details of the tool._)
  - [Windows 10 Examples 🖼️](#windows-10-examples) (_Examples and results from running the tool on Windows 10._)
  - [Windows 11 Examples 🖼️](#windows-11-examples) (_Examples and results from running the tool on Windows 11._)
- [Contributing 🤝](#contributing) (_Guidelines and information on how to contribute to the project._)
- [License 📜](#license) (_Review the MIT License details for DebloaterTool._)
- [Disclaimer ⚠️](#disclaimer) (_Important warnings and disclaimers regarding the usage of the tool._)

---

<a id="features"></a>
## Features ✅

- 🗑️ **Remove pre-installed bloatware**  
  Get rid of unnecessary apps that come with Windows.

- 🚫 **Disable unnecessary background services**  
  Free up system resources by stopping unused services.

- ⚡ **Improve system speed & responsiveness**  
  Optimize settings for faster boot times and better performance.

- 🌐 **Install Ungoogled Chromium Browser**  
  A lightweight, privacy-focused alternative to Google Chrome.

- 🖼️ **Change the wallpaper**  
  Refresh your desktop with a new look — a custom wallpaper included with the tool.

- 🔧 **Modify the boot logo**  
  Replace the default Windows boot logo with a custom one provided by the tool.

- 🛡️ **Disable Windows Defender**  
  Turn off real-time protection for performance (use with caution).

- 📡 **Disable Windows Update**  
  Stop automatic updates that can interrupt your workflow.

- 🧬 **Disable Spectre and Meltdown mitigations**  
  Boost performance on older CPUs by disabling certain security patches.

- 🛠️ **Disable unnecessary security features**  
  Improve speed by turning off non-essential security layers.

> [!WARNING]  
> * ⚠️ *Disabling essential services may cause system instability. Use with caution!* ⚠️<br>
> * 🛑 *Some features disable Windows Update and Defender, affecting system security.* 🛑<br>
> * 🛟 *It is recommended to create a system restore point before running the tool.* 🛟<br>

<a id="installation"></a>
## Installation 📥

<a id="compiled-version"></a>
### 📦 Option 1: Using the `Compiled Version` (For Ease of Use)

1. **Download the Executable:**
   - Visit the [DebloaterTool Releases Page](https://github.com/megsystem/DebloaterTool/releases) and download `DebloaterTool.exe`.  
   - Alternatively, you can download the experimental version from [this link](https://github.com/megsystem/DebloaterTool/blob/main/DebloaterTool.exe) *(Note: This version is updated whenever the code changes)*.

2. **Run the Application:**
   - Double-click on `DebloaterTool.exe` to launch the tool. **Ensure you run it as an administrator.**

<a id="use-command-line"></a>
### ⌨️ Option 2: Running the Command Using the `Command Line`

1. **Open a Command Execution Tool:**  
   - You can use **Command Prompt**, **PowerShell**, or even the **Windows Run Dialog** (`Win + R`) — whichever you prefer for running commands.

2. **Execute the Command:**
   - For the standard mode, enter:
     ```powershell
     powershell.exe -w hidden -Command "iex (iwr 'https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/DebloaterTool.ps1')"
     ```
   - For AutoDebloater (Complete Mode), use:
     ```powershell
     powershell.exe -w hidden -Command "iex (iwr 'https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/AutoDebloater.ps1')"
     ```
   - Press **Enter** to execute.

3. **Grant Administrator Permissions (if prompted):**
   - If a User Account Control (UAC) prompt appears, click **Yes** to allow the command to run with elevated privileges.

4. **Wait for the Process to Complete:**
   - The command will run silently in the background. Once finished, the DebloaterTool should be ready for use.

<a id="screenshots"></a>
## 📸 Screenshots

<a id="debloatertool-interface"></a>
### 🧰 DebloaterTool Interface

| UI |
|----|
| ![DebloaterTool UI](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/1.png) |

| Process |
|---------|
| ![DebloaterTool Process](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/2.png) |

---

<a id="windows-10-examples"></a>
### 💻 Windows 10

| Bootloader |
|------------|
| ![Bootloader - WIN10](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win10.bootloader.png) |

| Desktop |
|---------|
| ![Desktop - WIN10](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win10.desktop.png) |

| Result |
|--------|
| ![Result - WIN10](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win10.result.png) |

---

<a id="windows-11-examples"></a>
### 💻 Windows 11

| Bootloader |
|------------|
| ![Bootloader - WIN11](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win11.bootloader.png) |

| Desktop |
|---------|
| ![Desktop - WIN11](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win11.desktop.png) |

| Result |
|--------|
| ![Result - WIN11](https://raw.githubusercontent.com/megsystem/DebloaterTool/refs/heads/main/Screenshot/win11.result.png) |

<a id="contributing"></a>
## Contributing 🤝

Pull requests are welcome! If you have suggestions for improvements, feel free to submit an issue or a PR.

<a id="license"></a>
## License 📜

This project is licensed under the MIT License. See `LICENSE` for details.

<a id="disclaimer"></a>
## Disclaimer 🛑

**Use DebloaterTool at your own risk.**  
This tool is provided "as-is," without any warranty or guarantee of stability. Disabling certain services might result in system issues, so ensure that you create a system restore point before usage.
