# Playnite FSE Launcher

A native Windows 11 launcher that bypasses Xbox Full Screen Experience (FSE) restrictions to seamlessly replace the default Xbox gaming dashboard with **Playnite**.

Whether you are using a Lenovo Legion Go, ASUS ROG Ally, or a custom Windows HTPC, this tool allows you to map your hardware's "Console/Xbox" button directly to Playnite without Windows burying it in the background.

## ⚠️ Prerequisites

Before installing, ensure your setup meets the following requirements:
1. **Windows 10 or Windows 11** with the Xbox Gaming Home / FSE feature enabled.
2. **Playnite** must be installed in its default user directory. The launcher explicitly looks for the executable here:
   `%LOCALAPPDATA%\Playnite\Playnite.FullscreenApp.exe`

## 🚀 Installation

Because this tool uses custom system-level capabilities, it is distributed as a Sideloaded MSIX package. The included Microsoft script will automatically trust the local developer certificate and install the app.

1. Go to the **[Releases](../../releases)** page and download the latest `PlayniteFSE_v1.0.zip`.
2. Extract the `.zip` file to a folder on your device.
3. Open the extracted folder, right-click on the **`Install.ps1`** file, and select **Run with PowerShell**.
4. Windows will ask for administrator permission to install the developer certificate. Type `Y` and hit **Enter** when prompted.
5. Once the installation script finishes successfully, press your controller's Xbox/Home button to open the FSE menu.
6. Select **Playnite FSE** from the list of available dashboards to set it as your default!

## 🛠️ How It Works (The Technical Details)

Standard workarounds for launching Playnite via the Xbox FSE menu (like basic UWP bridges or standard shortcut scripts) fail because Windows FSE operates as a highly restricted sandbox. FSE acts like a console dashboard and actively suppresses unauthorized Win32 background processes from stealing window focus.

This launcher bypasses that sandbox natively:
* **The VIP Pass (.SCCD):** The app is packaged with a Custom Capability Descriptor (`.SCCD`) file that explicitly claims the `Microsoft.appCategory.gamingHome_8wekyb3d8bbwe` restricted capability. 
* **Native Win32 Execution:** Because the package holds this authorized "VIP Pass," Windows FSE recognizes it as a legitimate gaming shell. When executed, it natively runs a lean .NET 4.8 process that triggers `Playnite.FullscreenApp.exe` via `Process.Start()`. 
* **Zero Suppression:** Playnite inherits the foreground privileges of the authorized shell, taking over the screen immediately without being blocked by the OS.
