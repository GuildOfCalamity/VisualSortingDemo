using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace VisualSortingItems
{
    public partial class App : Application
    {
        private Window _window;
        public App()
        {
            // Configure exception handlers.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomainFirstChanceException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Determining passed arguments
            string[] argsAlt = Environment.GetCommandLineArgs();
            if (argsAlt.Length > 0)
            {
                Dictionary<string, string> argDict = new();

                var array = IgnoreFirstTakeRest(argsAlt);
                for (int i = 0; i < array.Length; i += 2)
                {
                    try
                    {
                        argDict[array[i]] = array[i + 1];
                    }
                    catch (Exception ex) // probably out of bounds or key duplicate
                    {
                        Debug.WriteLine($"Argument parsing: {ex.Message}", $"{nameof(App)}");
                    }
                }

                // Call secondary constructor.
                _window = new MainWindow(argDict);
            }
            else
            {
                // Call primary constructor.
                _window = new MainWindow();
            }

            // Show our main window.
            _window.Activate();
        }

        #region [Domain Events]
        void CurrentDomainOnProcessExit(object sender, EventArgs e)
        {
            if (sender is AppDomain ad)
            {
                Debug.WriteLine($"[OnProcessExit]", $"{nameof(App)}");
                Debug.WriteLine($"DomainID: {ad.Id}", $"{nameof(App)}");
                Debug.WriteLine($"FriendlyName: {ad.FriendlyName}", $"{nameof(App)}");
                Debug.WriteLine($"BaseDirectory: {ad.BaseDirectory}", $"{nameof(App)}");
            }
        }

        void CurrentDomainFirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Debug.WriteLine($"First chance exception: {e.Exception}", $"{nameof(App)}");
        }

        void CurrentDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            Exception? ex = e.ExceptionObject as Exception;
            Debug.WriteLine($"Thread exception of type {ex?.GetType()}: {ex}", $"{nameof(App)}");
        }

        void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Debug.WriteLine($"Unobserved task exception: {e.Exception}", $"{nameof(App)}");

            e.SetObserved(); // suppress and handle manually
        }
        #endregion

        /// <summary>
        /// Helper for parsing command line arguments.
        /// </summary>
        /// <param name="inputArray"></param>
        /// <returns>string array of args excluding the 1st arg</returns>
        string[] IgnoreFirstTakeRest(string[] inputArray)
        {
            if (inputArray.Length > 1)
                return inputArray.Skip(1).ToArray();
            else
                return new string[0];
        }
    }
}
