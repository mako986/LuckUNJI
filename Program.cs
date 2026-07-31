#pragma warning disable CA1416

using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Terminal.Gui;

class Program
{
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_MAXIMIZE = 3;

    static void Main()
    {
        Application.Init();
        ShowWindow(GetConsoleWindow(), SW_MAXIMIZE);

        Application.Top.Width = Dim.Fill();
        Application.Top.Height = Dim.Fill();

        var msiBiosColorScheme = new ColorScheme
        {
            Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Black),
            Focus = Terminal.Gui.Attribute.Make(Color.White, Color.Red),
            HotNormal = Terminal.Gui.Attribute.Make(Color.BrightRed, Color.Black),
            HotFocus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.Red)
        };

        var win = new Window("=== LUCK UNJI EDITION UNLOCKED ===")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = msiBiosColorScheme
        };

        var lblHeader = new Label(2, 1, "SYSTEM STATUS: STABLE [OC GENIE: DISABLED] | CPU: KERNEL v5.2")
        {
            ColorScheme = msiBiosColorScheme
        };

        var lblTitle = new Label(2, 3, "SELECT HARDWARE MANAGEMENT AND RECOVERY OPTION:")
        {
            ColorScheme = msiBiosColorScheme
        };

        var btnTaskManager = new Button(2, 6, "[1] HARD MANAGER") { Width = 60, ColorScheme = msiBiosColorScheme };
        var btnRestoreFonts = new Button(2, 9, "[2] FIX FONTS") { Width = 60, ColorScheme = msiBiosColorScheme };
        var btnRestoreAssoc = new Button(2, 12, "[3] REPAIR .EXE HARD") { Width = 60, ColorScheme = msiBiosColorScheme };
        var btnRestoreTaskMgr = new Button(2, 15, "[4] UNLOCK TASK MANAGER") { Width = 60, ColorScheme = msiBiosColorScheme };
        var btnUnlockKeys = new Button(2, 18, "[5] UNLOCKED HOTKEYS & UNLOCKED POLICIES") { Width = 60, ColorScheme = msiBiosColorScheme };
        var btnExplorer = new Button(2, 21, "[6] FILE EXPLORER") { Width = 60, ColorScheme = msiBiosColorScheme };
        var btnExit = new Button(2, 24, "[7] SAVE CONFIGURATION & REBOOT (ESC)") { Width = 60, ColorScheme = msiBiosColorScheme };

        btnTaskManager.Clicked += () => OpenHardManagerSubMenu(msiBiosColorScheme);
        btnRestoreFonts.Clicked += () => RestoreFontsLogic();
        btnRestoreAssoc.Clicked += () => RestoreAssocLogic();
        btnRestoreTaskMgr.Clicked += () => RestoreTaskMgrLogic();
        btnUnlockKeys.Clicked += () => UnlockKeysAndPoliciesLogic();
        btnExplorer.Clicked += () => OpenFileExplorerWindow(msiBiosColorScheme);
        btnExit.Clicked += () => Application.RequestStop();

        win.Add(lblHeader, lblTitle, btnTaskManager, btnRestoreFonts, btnRestoreAssoc, btnRestoreTaskMgr, btnUnlockKeys, btnExplorer, btnExit);

        Application.Run(win);
        Application.Shutdown();
    }

    static void OpenHardManagerSubMenu(ColorScheme msiBiosColorScheme)
    {
        var win = new Window("=== HARD MANAGER: SELECT MODE ===")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = msiBiosColorScheme
        };

        var lbl = new Label(2, 2, "SELECT MONITORING TOOL:") { ColorScheme = msiBiosColorScheme };

        var btnProc = new Button(2, 5, "[1] Process Manager (Main List)") { Width = 55, ColorScheme = msiBiosColorScheme };
        var btnCmdLine = new Button(2, 8, "[2] View Arguments (CmdLine via WMI)") { Width = 55, ColorScheme = msiBiosColorScheme };
        var btnScan = new Button(2, 11, "[3] Suspicious Process Scanner (Temp/AppData)") { Width = 55, ColorScheme = msiBiosColorScheme };
        var btnWinlogon = new Button(2, 14, "[4] Check Winlogon and Startup") { Width = 55, ColorScheme = msiBiosColorScheme };
        var btnBack = new Button(2, 18, "[0] Back to Main Menu") { Width = 55, ColorScheme = msiBiosColorScheme };

        btnProc.Clicked += () => OpenTaskManagerWindow(msiBiosColorScheme);
        btnCmdLine.Clicked += () => OpenCmdLineWindow(msiBiosColorScheme);
        btnScan.Clicked += () => OpenSuspiciousScannerWindow(msiBiosColorScheme);
        btnWinlogon.Clicked += () => OpenWinlogonWindow(msiBiosColorScheme);
        btnBack.Clicked += () => Application.RequestStop();

        win.Add(lbl, btnProc, btnCmdLine, btnScan, btnWinlogon, btnBack);
        Application.Run(win);
    }

    static void OpenTaskManagerWindow(ColorScheme msiBiosColorScheme)
    {
        var win = new Window("=== HARDWARE MONITOR: HARD MANAGER ===")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = msiBiosColorScheme
        };

        var searchFrame = new FrameView(" PROCESS FILTER (TYPE '1' TO EXIT) ")
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 1,
            Height = 3,
            ColorScheme = msiBiosColorScheme
        };

        var txtSearch = new TextField("") { X = 0, Y = 0, Width = Dim.Fill(), ColorScheme = msiBiosColorScheme };
        searchFrame.Add(txtSearch);

        var listView = new ListView(new string[0])
        {
            X = 1,
            Y = 5,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 4,
            ColorScheme = msiBiosColorScheme
        };

        var btnBack = new Button("BACK TO MAIN MENU")
        {
            X = 1,
            Y = Pos.AnchorEnd(1),
            ColorScheme = msiBiosColorScheme
        };

        void RefreshList()
        {
            string query = txtSearch.Text?.ToString()?.Trim().ToLower() ?? "";
            if (query == "1") { Application.RequestStop(); return; }

            var lines = new System.Collections.Generic.List<string>();
            lines.Add($"{"Process Name",-25} | {"ID",-6} | {"Module/Status",-22} | {"Path"}");
            lines.Add(new string('-', 95));

            try
            {
                foreach (var p in Process.GetProcesses())
                {
                    string name = p.ProcessName;
                    string id = p.Id.ToString();
                    string path = "";
                    string analysisStatus = "[Normal]";

                    try
                    {
                        path = p.MainModule?.FileName ?? "";
                        if (string.IsNullOrEmpty(path)) analysisStatus = "[No Path]";
                        else if (path.Contains("System32", StringComparison.OrdinalIgnoreCase) || path.Contains("SysWOW64", StringComparison.OrdinalIgnoreCase)) analysisStatus = "[System Core]";
                        else if (path.Contains("AppData", StringComparison.OrdinalIgnoreCase) || path.Contains("Temp", StringComparison.OrdinalIgnoreCase)) analysisStatus = "[Unstable/Temp]";
                        else analysisStatus = "[User App]";
                    }
                    catch
                    {
                        path = "[Protected Kernel Space]";
                        analysisStatus = "[Ring 0 / PPL]";
                    }

                    if (string.IsNullOrEmpty(query) || name.ToLower().Contains(query))
                    {
                        if (name.Length > 23) name = name.Substring(0, 20) + "...";
                        lines.Add($"{name,-25} | {id,-6} | {analysisStatus,-22} | {path}");
                    }
                }
            }
            catch (Exception ex) { lines.Add($"[BUS ERROR]: {ex.Message}"); }

            listView.SetSource(lines);
        }

        txtSearch.TextChanged += (s) => RefreshList();
        btnBack.Clicked += () => Application.RequestStop();

        win.Add(searchFrame, listView, btnBack);
        RefreshList();
        Application.Run(win);
    }

    static void OpenCmdLineWindow(ColorScheme msiBiosColorScheme)
    {
        var win = new Window("=== HARD MANAGER: ARGUMENTS (CMDLINE) ===")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = msiBiosColorScheme
        };

        var listView = new ListView(new string[0])
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 3,
            ColorScheme = msiBiosColorScheme
        };

        var btnBack = new Button("BACK")
        {
            X = 1,
            Y = Pos.AnchorEnd(1),
            ColorScheme = msiBiosColorScheme
        };

        var lines = new System.Collections.Generic.List<string>
        {
            $"{"Process Name",-20} | {"PID",-6} | {"Command Line (CmdLine)"}",
            new string('-', 95)
        };

        try
        {
            using (var searcher = new ManagementObjectSearcher("SELECT ProcessId, Name, CommandLine FROM Win32_Process"))
            {
                foreach (var obj in searcher.Get())
                {
                    string name = obj["Name"]?.ToString() ?? "Unknown";
                    string pid = obj["ProcessId"]?.ToString() ?? "0";
                    string cmdLine = obj["CommandLine"]?.ToString() ?? "[Access Denied]";
                    if (name.Length > 18) name = name.Substring(0, 15) + "...";
                    lines.Add($"{name,-20} | {pid,-6} | {cmdLine}");
                }
            }
        }
        catch (Exception ex) { lines.Add($"WMI Error: {ex.Message}"); }

        listView.SetSource(lines);
        btnBack.Clicked += () => Application.RequestStop();
        win.Add(listView, btnBack);
        Application.Run(win);
    }

    static void OpenSuspiciousScannerWindow(ColorScheme msiBiosColorScheme)
    {
        var win = new Window("=== HARD MANAGER: SUSPICIOUS PROCESSES ===")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = msiBiosColorScheme
        };

        var listView = new ListView(new string[0])
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 3,
            ColorScheme = msiBiosColorScheme
        };

        var btnBack = new Button("BACK")
        {
            X = 1,
            Y = Pos.AnchorEnd(1),
            ColorScheme = msiBiosColorScheme
        };

        var lines = new System.Collections.Generic.List<string>
        {
            "SCANNING FOR ACTIVITY IN TEMP AND APPDATA:",
            new string('-', 70)
        };

        try
        {
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    string path = proc.MainModule?.FileName ?? "";
                    if (!string.IsNullOrEmpty(path))
                    {
                        string lower = path.ToLower();
                        if (lower.Contains("\\temp\\") || lower.Contains("\\appdata\\roaming\\"))
                        {
                            lines.Add($"[SUSPICIOUS] ID: {proc.Id} | {proc.ProcessName}");
                            lines.Add($"  Path: {path}\n");
                        }
                    }
                }
                catch { }
            }
            if (lines.Count <= 2) lines.Add("No threats found in Temp/AppData.");
        }
        catch (Exception ex) { lines.Add($"Error: {ex.Message}"); }

        listView.SetSource(lines);
        btnBack.Clicked += () => Application.RequestStop();
        win.Add(listView, btnBack);
        Application.Run(win);
    }

    static void OpenWinlogonWindow(ColorScheme msiBiosColorScheme)
    {
        var win = new Window("=== HARD MANAGER: WINLOGON & STARTUP ===")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = msiBiosColorScheme
        };

        var listView = new ListView(new string[0])
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 3,
            ColorScheme = msiBiosColorScheme
        };

        var btnBack = new Button("BACK")
        {
            X = 1,
            Y = Pos.AnchorEnd(1),
            ColorScheme = msiBiosColorScheme
        };

        var lines = new System.Collections.Generic.List<string>();

        try
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"))
            {
                if (key != null)
                {
                    lines.Add($"[Winlogon Shell]: {key.GetValue("Shell")}");
                    lines.Add($"[Winlogon Userinit]: {key.GetValue("Userinit")}\n");
                }
            }
        }
        catch (Exception ex) { lines.Add($"Winlogon Error: {ex.Message}"); }

        lines.Add("REGISTRY STARTUP KEYS (HKCU):");
        string[] regPaths = {
            @"Software\Microsoft\Windows\CurrentVersion\Run",
            @"Software\Microsoft\Windows\CurrentVersion\RunOnce"
        };

        foreach (var path in regPaths)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        foreach (string valName in key.GetValueNames())
                        {
                            lines.Add($" - {valName}: {key.GetValue(valName)}");
                        }
                    }
                }
            }
            catch { }
        }

        listView.SetSource(lines);
        btnBack.Clicked += () => Application.RequestStop();
        win.Add(listView, btnBack);
        Application.Run(win);
    }

    static void OpenFileExplorerWindow(ColorScheme msiBiosColorScheme)
    {
        string currentPath = DriveInfo.GetDrives()[0].RootDirectory.FullName;

        var win = new Window("=== HARD MANAGER: FILE EXPLORER ===")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = msiBiosColorScheme
        };

        var lblPath = new Label(2, 1, "PATH: ") { ColorScheme = msiBiosColorScheme };
        var lblCurrentPath = new Label(8, 1, currentPath) { ColorScheme = msiBiosColorScheme };

        var listView = new ListView(new string[0])
        {
            X = 1,
            Y = 3,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 4,
            ColorScheme = msiBiosColorScheme
        };

        // Раздельные кнопки: BACK (возврат в родительскую папку), DELETE, OPEN, EXIT (выход в главное меню)
        var btnBack = new Button("BACK")
        {
            X = 1,
            Y = Pos.AnchorEnd(1),
            ColorScheme = msiBiosColorScheme
        };
        var btnDelete = new Button("DELETE")
        {
            X = 10,
            Y = Pos.AnchorEnd(1),
            ColorScheme = msiBiosColorScheme
        };
        var btnOpen = new Button("OPEN")
        {
            X = 20,
            Y = Pos.AnchorEnd(1),
            ColorScheme = msiBiosColorScheme
        };
        var btnExit = new Button("EXIT")
        {
            X = 28,
            Y = Pos.AnchorEnd(1),
            ColorScheme = msiBiosColorScheme
        };

        var itemsList = new System.Collections.Generic.List<FileSystemItem>();

        void LoadDirectory(string path)
        {
            try
            {
                currentPath = path;
                lblCurrentPath.Text = currentPath;
                itemsList.Clear();
                var displayLines = new System.Collections.Generic.List<string>();

                if (Directory.GetParent(currentPath) != null)
                {
                    itemsList.Add(new FileSystemItem { Name = ".. [PARENT DIRECTORY]", FullPath = Directory.GetParent(currentPath)!.FullName, IsDirectory = true });
                    displayLines.Add(".. [PARENT DIRECTORY]");
                }

                foreach (var dir in Directory.GetDirectories(currentPath))
                {
                    var dirInfo = new DirectoryInfo(dir);
                    itemsList.Add(new FileSystemItem { Name = "[DIR] " + dirInfo.Name, FullPath = dirInfo.FullName, IsDirectory = true });
                    displayLines.Add("[DIR] " + dirInfo.Name);
                }

                foreach (var file in Directory.GetFiles(currentPath))
                {
                    var fileInfo = new FileInfo(file);
                    itemsList.Add(new FileSystemItem { Name = "      " + fileInfo.Name, FullPath = fileInfo.FullName, IsDirectory = false });
                    displayLines.Add("      " + fileInfo.Name);
                }

                listView.SetSource(displayLines);
            }
            catch (Exception ex)
            {
                MessageBox.Query(60, 7, "ACCESS ERROR", $"Cannot open directory:\n{ex.Message}", "OK");
            }
        }

        void HandleSelection()
        {
            int idx = listView.SelectedItem;
            if (idx >= 0 && idx < itemsList.Count)
            {
                var item = itemsList[idx];
                if (item.IsDirectory)
                {
                    LoadDirectory(item.FullPath);
                }
                else
                {
                    try
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c start \"\" \"{item.FullPath}\"",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };
                        Process.Start(psi);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Query(60, 7, "ERROR", $"Cannot launch file:\n{ex.Message}", "OK");
                    }
                }
            }
        }

        listView.OpenSelectedItem += (s) => HandleSelection();
        btnOpen.Clicked += () => HandleSelection();

        // Кнопка BACK теперь возвращает в родительскую папку (если она есть)
        btnBack.Clicked += () =>
        {
            var parent = Directory.GetParent(currentPath);
            if (parent != null)
            {
                LoadDirectory(parent.FullName);
            }
            else
            {
                MessageBox.Query(40, 6, "INFO", "This is the root directory.", "OK");
            }
        };

        btnDelete.Clicked += () =>
        {
            int idx = listView.SelectedItem;
            if (idx >= 0 && idx < itemsList.Count)
            {
                var item = itemsList[idx];
                if (item.FullPath == currentPath && item.IsDirectory) return;

                int choice = MessageBox.Query(50, 7, "CONFIRM DELETE", $"Are you sure you want to delete:\n{item.Name}?", "Yes", "No");
                if (choice == 0)
                {
                    try
                    {
                        if (item.IsDirectory) Directory.Delete(item.FullPath, true);
                        else File.Delete(item.FullPath);
                        LoadDirectory(currentPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Query(60, 7, "DELETE ERROR", $"Failed to delete item:\n{ex.Message}", "OK");
                    }
                }
            }
        };

        // Кнопка EXIT выходит в главное меню приложения
        btnExit.Clicked += () => Application.RequestStop();

        win.Add(lblPath, lblCurrentPath, listView, btnBack, btnDelete, btnOpen, btnExit);
        LoadDirectory(currentPath);
        Application.Run(win);
    }

    static void RestoreFontsLogic()
    {
        try
        {
            using (var baseKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\FontSubstitutes", writable: true))
            {
                if (baseKey != null)
                {
                    string[] important = { "MS Shell Dlg", "MS Shell Dlg 2", "UI Gothic" };
                    foreach (string valName in baseKey.GetValueNames())
                    {
                        bool isImportant = false;
                        foreach (var imp in important)
                        {
                            if (string.Equals(valName, imp, StringComparison.OrdinalIgnoreCase)) { isImportant = true; break; }
                        }
                        if (!isImportant) baseKey.DeleteValue(valName, false);
                    }
                }
            }
            MessageBox.Query(50, 7, "SUCCESS", "Font cache successfully reset.", "OK");
        }
        catch (Exception ex) { MessageBox.Query(60, 8, "ACCESS DENIED", $"Registry access error:\n{ex.Message}", "OK"); }
    }

    static void RestoreAssocLogic()
    {
        try
        {
            ExecuteCommand("assoc .exe=exefile");
            ExecuteCommand("ftype exefile=\"%1\" %*");
            MessageBox.Query(50, 7, "SUCCESS", ".EXE associations successfully restored.", "OK");
        }
        catch (Exception ex) { MessageBox.Query(50, 7, "ERROR", $"Recovery failed: {ex.Message}", "OK"); }
    }

    static void RestoreTaskMgrLogic()
    {
        try
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", writable: true))
            {
                if (key != null && key.GetValue("DisableTaskMgr") != null)
                {
                    key.DeleteValue("DisableTaskMgr");
                    MessageBox.Query(50, 7, "SUCCESS", "Task Manager restriction removed.", "OK");
                }
                else
                {
                    MessageBox.Query(50, 7, "INFO", "Task Manager is already unlocked.", "OK");
                }
            }
        }
        catch (Exception ex) { MessageBox.Query(60, 8, "ERROR", $"Access error:\n{ex.Message}", "OK"); }
    }

    static void UnlockKeysAndPoliciesLogic()
    {
        try
        {
            ExecuteCommand("reg delete \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\" /v NoWinKeys /f");
            ExecuteCommand("reg delete \"HKCU\\Control Panel\\Accessibility\\StickyKeys\" /f");
            ExecuteCommand("reg delete \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v DisableRegistryTools /f");
            MessageBox.Query(50, 7, "SUCCESS", "Hotkeys and policies reset to default.", "OK");
        }
        catch (Exception ex) { MessageBox.Query(50, 7, "ERROR", $"Failed to apply parameters: {ex.Message}", "OK"); }
    }

    static void ExecuteCommand(string command)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {command}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = Process.Start(psi);
        process?.WaitForExit();
    }
}

class FileSystemItem
{
    public string Name { get; set; } = "";
    public string FullPath { get; set; } = "";
    public bool IsDirectory { get; set; }
}