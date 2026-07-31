# Luck Unji Edition Unlocked

**Download:** You need to download two files, **LuckUnji.msi** and **cab1.cab**, and run the installer.

**Error:** If **cab1.cab** is missing, the installer will crash with an error stating that the file cannot be found.

---

A powerful, high-performance system recovery, hardware management, and diagnostic tool built in C# using the `Terminal.Gui` library for terminal-based graphical interfaces (TUI). Designed to bypass restrictions, repair critical Windows components, and monitor system resources in real-time.

---

## Features

* **Advanced Process Manager (Hard Manager):**
  * Real-time list of running processes with categorization (System Core, Unstable/Temp, User Apps, Protected Kernel Space).
  * Live argument and command-line inspector (via WMI).
  * Automated suspicious process scanner targeting `Temp` and `AppData` directories.
  * Winlogon shell/userinit verification and startup registry keys analyzer.
* **Built-in File Explorer:**
  * Fully interactive TUI file explorer with directory navigation.
  * Direct file launching using the system shell.
  * Secure file and directory deletion with confirmation prompts.
* **System & Registry Recovery:**
  * **Fix Fonts:** Cleans up broken or corrupted font substitution entries in the registry.
  * **Repair .EXE Hard:** Restores default `.exe` file associations and shell file types (`exefile`).
  * **Unlock Task Manager:** Removes registry policies restricting Task Manager access.
  * **Unlock Policies & Hotkeys:** Resets disabled registry tools (`DisableRegistryTools`), sticky keys restrictions, and system hotkeys (`NoWinKeys`).

---

## Requirements

* **OS:** Windows 10 / 11 (Admin privileges required for full registry and process management capabilities).
* **Runtime:** .NET / .NET Core compatible environment (configured for modern C# SDK targets).
* **Dependencies:** `Terminal.Gui` NuGet package.

---

## Installation & Building

1. Clone or download the repository.
2. Ensure you have the `Terminal.Gui` library added to your project dependencies.
3. Build and run the project via terminal or your preferred C# IDE (e.g., Visual Studio or via `dotnet build`).
