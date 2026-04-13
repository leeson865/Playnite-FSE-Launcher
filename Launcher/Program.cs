using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    internal static class Program
    {
        // Mutex prevents multiple launcher instances from fighting over the UI thread.
        private static Mutex mutex = new Mutex(true, "{PLAYNITE-FSE-BRIDGE-UNIQUE-ID}");

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Playnite FSE Bridge:
        /// Provides a native 'GamingHomeApp' shell for Windows 11 Handheld Experience.
        /// Fixes the 30-second Winlogon hang while preserving instant XInput focus for Playnite.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 1. Single Instance Guard
            // Prevents redundant window creation if the launcher is triggered while already running.
            if (!mutex.WaitOne(TimeSpan.Zero, true)) return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 2. State Check: Is Playnite already active?
            // This prevents the 'Ghost Window' from appearing when using Game Bar 'Home' or 'Library' buttons.
            var playniteProcess = Process.GetProcessesByName("Playnite.FullscreenApp").FirstOrDefault();

            if (playniteProcess == null)
            {
                // PHASE A: INITIAL BOOT HANDSHAKE
                // Winlogon.exe requires a top-level window to exist before it will drop the 'Welcome' spinner.
                using (Form handshakeForm = new Form())
                {
                    // Match Windows 11 Dark Mode neutral surface color (#202020)
                    handshakeForm.BackColor = Color.FromArgb(32, 32, 32);
                    handshakeForm.FormBorderStyle = FormBorderStyle.None;
                    handshakeForm.WindowState = FormWindowState.Maximized;
                    handshakeForm.ShowInTaskbar = false;

                    // 1% Opacity allows the window to be mathematically 'present' for the OS
                    // but visually transparent to the user, creating a seamless boot transition.
                    handshakeForm.Opacity = 0.01;

                    handshakeForm.Shown += async (sender, e) =>
                    {
                        // 1500ms is the 'Goldilocks' threshold. 
                        // It is long enough for the Windows Kernel/DWM to stabilize on cold boot,
                        // but short enough to keep the boot sequence feeling snappy.
                        await Task.Delay(1500);

                        // We explicitly close the form to annihilate the UI thread.
                        // This ensures the XInput queue is empty before Playnite initializes.
                        handshakeForm.Close();
                    };

                    Application.Run(handshakeForm);
                }

                // PHASE B: COLD LAUNCH
                // Launching Playnite strictly AFTER the ghost window is destroyed 
                // ensures Playnite is granted primary foreground focus and instant controller access.
                ProcessStartInfo playniteInfo = new ProcessStartInfo
                {
                    FileName = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Playnite\Playnite.FullscreenApp.exe"),
                    Arguments = "--hidesplashscreen",
                    UseShellExecute = true
                };

                try
                {
                    playniteProcess = Process.Start(playniteInfo);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Critical Error: Could not launch Playnite: {ex.Message}");
                    return;
                }
            }
            else
            {
                // PHASE C: SECONDARY ACTIVATION (Home/Library Buttons)
                // If Playnite is already running, we skip the handshake and simply 
                // ensure it is brought to the foreground.
                IntPtr handle = playniteProcess.MainWindowHandle;
                if (handle != IntPtr.Zero)
                {
                    SetForegroundWindow(handle);
                }
            }

            // PHASE D: SESSION PERSISTENCE
            // The launcher must remain alive in the background. If this process terminates,
            // Windows assumes the shell has crashed and may force-log the user out.
            if (playniteProcess != null)
            {
                playniteProcess.WaitForExit();
            }

            // Cleanup
            GC.KeepAlive(mutex);
        }
    }
}