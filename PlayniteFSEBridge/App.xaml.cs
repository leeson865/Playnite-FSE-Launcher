using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

namespace PlayniteFSEBridge
{
    /// <summary>
    /// The "Front Door" Bridge for Playnite Fullscreen Experience.
    /// This UWP app acts as the authorized shell to wake up the Win32 Launcher.
    /// </summary>
    public sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Boilerplate UWP Window Setup
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Navigate to the blank main page
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                // 1. Show the Bridge Window (Splash Screen)
                Window.Current.Activate();

                // 2. FSE HANDOFF BUFFER
                // We wait 2 seconds to let the OS fully promote this app to the foreground.
                // If we fire the bridge too fast, Windows security blocks the "Full Trust" request.
                await Task.Delay(2000);

                try
                {
                    // 3. TRIGGER THE WIN32 LAUNCHER
                    // This tells the Packaging Project to run Launcher.exe
                    await Windows.ApplicationModel.FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();

                    // 4. HANDOVER BUFFER
                    // Give the EXE 1 second to start Playnite before we close the UWP "Curtain"
                    await Task.Delay(1000);

                    // 5. EXIT
                    // Close the bridge so Playnite is the only thing on screen
                    Application.Current.Exit();
                }
                catch (Exception ex)
                {
                    // If something fails, we exit anyway so the user isn't stuck
                    System.Diagnostics.Debug.WriteLine($"Bridge Error: {ex.Message}");
                    Application.Current.Exit();
                }
            }
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // No state saving needed for a bridge app
            deferral.Complete();
        }
    }
}