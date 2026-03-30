using System;
using System.Diagnostics;

namespace Launcher
{
    class Program
    {
        static void Main()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Playnite\Playnite.FullscreenApp.exe"),
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch { /* Silent fail */ }
        }
    }
}