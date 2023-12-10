using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.UI.Xaml;

using Windows.Graphics;

namespace VisualSortingItems
{
    public static class WindowExtensions
    {
        public static event Action<string> OnWindowClosing = (msg) => { };
        /// <summary>
        /// For signaling processes that are still running.
        /// </summary>
        public static bool IsClosing { get; private set; } = false;

        public static void SetSize(this Window window, int width, int height)
        {
            // To set the size you need to wrap the XAML window in a AppWindow
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            if (hWnd == IntPtr.Zero) { return; }
            var windowsId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowsId);

            // With the AppWindow we can configure other events:
            appWindow.Closing += OnWindowClosingEvent;

            // Notice that AppWindow uses raw pixels because it's a low level API.
            // WinUI uses effective pixels, so the size of the elements scale within the scale set in the Windows Settings (100%, 120%, 200%, etc)
            // It's expected that your APIs use the same unit as WinUI (effective pixels), 
            // so you should convert the size from effective pixels to raw pixels to use the Microsoft.UI.Windowing.AppWindow APIs.
            var rawPixels = ConvertEffectivePixelsIntoRawPixels(hWnd, new SizeInt32(width, height));

            SetIcon("Assets/Cube-Purple.ico", appWindow);
            appWindow.Resize(rawPixels);
        }

        static SizeInt32 ConvertEffectivePixelsIntoRawPixels(IntPtr hWnd, SizeInt32 effectivePixels)
        {
            SizeInt32 rawPixels = new();

            if (hWnd == IntPtr.Zero) { return rawPixels; }

            // We can get the scale factor from the Win32 API's GetDpiForWindow divided by 96. 
            double dpi = GetDpiForWindow(hWnd);
            float scaleFactor = (float)dpi / 96;
            rawPixels.Width = (int)(effectivePixels.Width * scaleFactor);
            rawPixels.Height = (int)(effectivePixels.Height * scaleFactor);

            return rawPixels;
        }

        /// <summary>
        /// Centers a <see cref="Microsoft.UI.Xaml.Window"/> based on the <see cref="Microsoft.UI.Windowing.DisplayArea"/>.
        /// </summary>
        public static bool CenterWindow(this Window window)
        {
            try
            {
                System.IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                if (hWnd  == IntPtr.Zero) { return false; }
                Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                if (Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId) is Microsoft.UI.Windowing.AppWindow appWindow &&
                    Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest) is Microsoft.UI.Windowing.DisplayArea displayArea)
                {
                    Windows.Graphics.PointInt32 CenteredPosition = appWindow.Position;
                    CenteredPosition.X = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
                    CenteredPosition.Y = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;
                    appWindow.Move(CenteredPosition);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Local event to fire the <see cref="OnWindowClosing"/>.
        /// </summary>
        static void OnWindowClosingEvent(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            IsClosing = true;
            Debug.WriteLine($"[OnWindowClosing] Cancel={args.Cancel}");
            OnWindowClosing?.Invoke(DateTime.Now.ToString("hh:mm:ss.fff tt"));
        }

        /// <summary>
        /// Use <see cref="Microsoft.UI.Windowing.AppWindow"/> to set the taskbar icon for WinUI application.
        /// This method has been tested with an unpackaged and packaged app.
        /// Setting the icon in the project file using the ApplicationIcon tag.
        /// </summary>
        /// <param name="iconName">the local path, including any subfolder, e.g. "Assets/Logo.ico"</param>
        /// <param name="appWindow"><see cref="Microsoft.UI.Windowing.AppWindow"/></param>
        static void SetIcon(string iconName, Microsoft.UI.Windowing.AppWindow appWindow)
        {
            if (appWindow == null)
                return;

            try
            {
                appWindow.SetIcon(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), iconName));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
            }
        }

        public static string GetPackageName()
        {
            StringBuilder sb = new StringBuilder(1024);
            int length = 0;
            int result = GetCurrentPackageFullName(ref length, ref sb);

            if (result != 15700) // no package found
            {
                
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        [DllImport("User32.dll")]
        public static extern int GetDpiForWindow(IntPtr hwnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetCurrentPackageFullName(ref int packageFullNameLength, ref StringBuilder packageFullName);
    }
}
