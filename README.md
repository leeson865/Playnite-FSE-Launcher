# Playnite FSE Launcher

A native Windows 11 launcher that bypasses Xbox Full Screen Experience (FSE) restrictions to seamlessly replace the default Xbox gaming dashboard with **Playnite**.

Whether you are using a Lenovo Legion Go, ASUS ROG Ally, or a custom Windows HTPC, this tool allows you to map your hardware's FSE Launcher directly to Playnite without Windows burying it in the background, and with no hacky workarounds that require running the Xbox app and terminating the process.

## ⚠️ Prerequisites

Before installing, ensure your setup meets the following requirements:
1. **Windows 11** with the Xbox Gaming Home / FSE feature enabled.
2. **Playnite** must be installed in its default user directory. The launcher explicitly looks for the executable here:
   `%LOCALAPPDATA%\Playnite\Playnite.FullscreenApp.exe`

## 🚀 Installation

Because this tool uses custom system-level capabilities, it is distributed as a Sideloaded MSIX package. The included Microsoft script will automatically trust the local developer certificate and install the app.

1. Go to the **[Releases](../../releases)** page and download the latest `PlayniteFSE_v1.0.zip`.
2. Extract the `.zip` file to a folder on your device.
3. Open the extracted folder, right-click on the **`InstallPlayniteFSE.bat`** file, and select **Run as Administrator**.
4. Windows will ask for administrator permission to install the developer certificate. Type `Y` and hit **Enter** when prompted.
5. Windows may warn you about running a script externally. Select `Yes to all` via the command.
6. Once the installation script finishes successfully, check your Start menu and you should see Playnite FSE installed.
7. Open **Windows Settings > Gaming > Full Screen Experience**.
8. Select **Playnite FSE** from the list of available apps (it should be underneath Xbox) to set it as your default!

## 🛠️ How It Works (The Technical Details)

Historically, replacing the FSE dashboard required messy workarounds—like letting the official Xbox app load and then forcefully terminating its background process to wrestle window focus back to Playnite. Furthermore, no community app has previously been able to successfully populate inside the official Windows Settings dropdown.

This launcher changes that by integrating natively with the OS:

### 1. The Settings Dropdown Integration
To get the app to appear directly in the `Settings > Gaming > Full Screen Experience` menu alongside the official Xbox app, the launcher is packaged with a strictly formatted `AppxManifest`. By correctly implementing the `windows.gamingApp` extension, Windows natively recognizes the package as a selectable dashboard option.

### 2. Dashboard Privileges (The `.SCCD` File)
To ensure Windows grants the app the necessary foreground focus without suppressing it in the background, the package includes a Custom Capability Descriptor (`.SCCD`) file. This officially declares the `Microsoft.appCategory.gamingHome_8wekyb3d8bbwe` capability. By declaring this capability via the package manifest, the OS treats the launcher as a native FSE environment.

### 3. The Execution (`Launcher.exe`)
Once selected in the Windows Settings, the actual execution is incredibly lightweight. The package runs a minimal .NET 4.8 application that simply triggers `Playnite.FullscreenApp.exe` and gracefully exits. Because the initial package holds the correct FSE dashboard capabilities, Playnite inherits that foreground focus and launches flawlessly with zero background overhead.

---
### Support the Project
If this launcher made your handheld experience a bit smoother, feel free to buy me a coffee!

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/leeson65035)
