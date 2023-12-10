using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

#region [For Acrylic Brush Effect]
using Microsoft.UI.Composition.SystemBackdrops;
using WinRT; // required to support Window.As<ICompositionSupportsSystemBackdrop>()
#endregion

using Windows.UI.StartScreen; // for tile calls (see note)

using VisualSortingItems.SortingAlgorithm;

namespace VisualSortingItems;

public sealed partial class MainWindow : Window
{
    #region [Properties]
    List<ISortStrategy> _algorithmCollection;
    public List<ISortStrategy> AlgorithmCollection { get => _algorithmCollection;  }

    public Observables Obs = new Observables();
    List<Windows.UI.Color> _colors = new();
    Windows.UI.Color _clr = Windows.UI.Color.FromArgb(255, 100, 100, 100);
    int _colorFloor = 50;
    int _selectedPalette = 0;
    List<byte[]> _palette = new();
    public List<int> Palettes { get; } = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

    int _shape = 0;
    const int Size = 76;
    const int PaddingElement = 2;
    const int ElementSize = 8;
    const int Speed = 100; // milliseconds

    IList<int> _collection;
    IList<int> _exceptCollection;
    ISortStrategy _sorter;
    CancellationTokenSource _cancellationTokenSource;
    #endregion


    public MainWindow()
    {
        #region [Configure the color palettes]
        _palette.Add(new byte[] { 150, 150, 255 });
        _palette.Add(new byte[] { 150, 255, 150 });
        _palette.Add(new byte[] { 255, 150, 150 });
        _palette.Add(new byte[] { 255, 255, 150 });
        _palette.Add(new byte[] { 150, 255, 255 });
        _palette.Add(new byte[] { 255, 150, 255 });
        _palette.Add(new byte[] { 255, 255, 255 });
        _palette.Add(new byte[] { 150, 150, 150 });
        _palette.Add(new byte[] { 220, (byte)_colorFloor, (byte)_colorFloor });
        _palette.Add(new byte[] { (byte)_colorFloor, 220, (byte)_colorFloor });
        _palette.Add(new byte[] { (byte)_colorFloor, (byte)_colorFloor, 220 });
        _palette.Add(new byte[] { 230, 200, (byte)_colorFloor });
        _palette.Add(new byte[] { (byte)_colorFloor, 220, 255 });
        //_colors = GetWinUIColorList();
        #endregion

        InitializeCollections();
        InitializeComponent();

        this.SetSize(920, 860);
        if (!this.CenterWindow())
            Debug.WriteLine($"Failed to center window.", nameof(MainWindow));

        // Apply custom title bar
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(titleBar);

        btnStop.IsEnabled = false;
    }

    public MainWindow(Dictionary<string, string> args) : this()
    {
        Debug.WriteLine($"{args.Count} arguments were passed.", nameof(MainWindow));
    }

    void InitializeCollections()
    {
        _algorithmCollection = new List<ISortStrategy>
        {
            new BubbleSort(),
            new QuickSort(),
            new ShellSort(),
            new HeapSort(),
            new RadixSort(),
            new BucketSort(),
            new SelectionSort(),
            new MergeSort(),
            new InsertionSort(),
        };

        _collection = new List<int>();
        _exceptCollection = new List<int>();

        for (int i = 0; i < Size; i++)
            _exceptCollection.Add(i);
    }

    void OnStartClick(object sender, RoutedEventArgs e)
    {
        if (AlgorithmComboBox.SelectedItem == null)
            return;

        _shape = Random.Shared.Next(0, 9);

        InitializeCollectionWithRandomNumbers(_collection, Size);

        _sorter = (ISortStrategy)AlgorithmComboBox.SelectedItem;
        _sorter.ReportProgress += UpdateProgress;

        btnStart.IsEnabled = AlgorithmComboBox.IsEnabled = false;
        btnStop.IsEnabled = true;

        StartSorting();
    }
    
    void OnStopClick(object sender, RoutedEventArgs e)
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _collection.Clear();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }
    }

    void OnPaletteSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cb = sender as ComboBox;
        _selectedPalette = (int)cb.SelectedValue;
    }

    void InitializeCollectionWithRandomNumbers(IList<int> collection, int size)
    {
        collection.Clear();
        Random random = new((int)DateTime.Now.Ticks);
        for (int i = 0; i < size; i++)
        {
            int newNumber = random.Next(size);
            if (collection.Contains(newNumber))
            {
                IList<int> tmp = _exceptCollection.Except(collection).ToList();
                newNumber = tmp[random.Next(tmp.Count - 1)];
            }
            collection.Add(newNumber);
        }
    }

    /// <summary>
    /// Is only called once the sort has done a full iteration.
    /// Originates from <see cref="SortAlgorithmBase.ReportProgress"/> <see cref="Action"/> event.
    /// This could be smoother if a refresh-rate draw timer was employed.
    /// </summary>
    void UpdateProgress(IList<int> list)
    {
        VisualizeCollection(list, _shape);
        Thread.Sleep(Speed);
    }

    async void StartSorting()
    {
        infoBar.IsOpen = true;
        infoBar.Message = AppResourceManager.GetInstance.GetString("InforBar.Message.ActionInProgress");
        infoBar.Severity = InfoBarSeverity.Warning;
        progress.IsActive = Obs.IsBusy = true;
        StoryboardSpin.Begin();
        DateTime dtStart = DateTime.Now;

        _cancellationTokenSource = new();
        try
        {
            await Task.Run(() =>
            {
                _sorter.SortCancellationToken = _cancellationTokenSource.Token;
                _sorter.Sort(_collection);
                _sorter.ReportProgress -= UpdateProgress;

                this.DispatcherQueue.TryEnqueue(() =>
                {

                    TimeSpan time = DateTime.Now.Subtract(dtStart);
                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        infoBar.Message = $"{AppResourceManager.GetInstance.GetString("InforBar.Message.ActionCancelled")} ({time.TotalSeconds.ToString("F")} seconds)";
                        infoBar.Severity = InfoBarSeverity.Error;
                    }
                    else
                    {
                        infoBar.Message = $"{AppResourceManager.GetInstance.GetString("InforBar.Message.ActionCompleted")} ({time.TotalSeconds.ToString("F")} seconds)";
                        infoBar.Severity = InfoBarSeverity.Success;
                    }
                    progress.IsActive = Obs.IsBusy = false;
                    btnStart.IsEnabled = AlgorithmComboBox.IsEnabled = true;
                    btnStop.IsEnabled = false;
                    StoryboardSpin.Stop();
                });

            });
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            _sorter.ReportProgress -= UpdateProgress;
            infoBar.Message = AppResourceManager.GetInstance.GetString("InforBar.Message.ActionCancelled"); ;
            infoBar.Severity = InfoBarSeverity.Error;
            progress.IsActive = false;
            btnStart.IsEnabled = AlgorithmComboBox.IsEnabled = true;
            btnStop.IsEnabled = false;
        }
    }

    /// <summary>
    /// The more complex geometric shapes tend to stutter the framerate.
    /// https://learn.microsoft.com/en-us/windows/apps/design/controls/shapes
    /// </summary>
    void VisualizeCollection(IList<int> list, int shapeType)
    {
        if (list != null && list.Count > 0)
        {
            canvas.DispatcherQueue.TryEnqueue(() =>
            {
                canvas.Children.Clear();

                for (int i = 0; i < list.Count; i++)
                {
                    if (i % 10 == 0) // Every 10 elements will have a specific color.
                        _clr = Windows.UI.Color.FromArgb(255, (byte)Random.Shared.Next(_colorFloor, _palette[_selectedPalette][0]), (byte)Random.Shared.Next(_colorFloor, _palette[_selectedPalette][1]), (byte)Random.Shared.Next(_colorFloor, _palette[_selectedPalette][2]));

                    int element = list[i];
                    switch (shapeType)
                    {
                        // We'll tip the odds in favor of ellipse and rectangle by increasing the cases.
                        case 0:
                        case 1:
                        case 2:
                            Ellipse ellipse = new Ellipse();
                            ellipse.Fill = new SolidColorBrush(_clr);
                            ellipse.Width = ellipse.Height = ElementSize + (ElementSize * 2);
                            //ellipse.StrokeThickness = 1.0d;
                            //ellipse.Stroke = new SolidColorBrush(Colors.Gray);
                            canvas.Children.Add(ellipse);
                            Canvas.SetLeft(ellipse, list.IndexOf(element) * ElementSize + PaddingElement + (i * 2));
                            Canvas.SetTop(ellipse, element * ElementSize + PaddingElement + (i == 0 ? i : i/2));
                            break;
                        case 3:
                        case 4:
                        case 5:
                            Rectangle rectangle = new();
                            rectangle.Height = ElementSize + (ElementSize / 2);
                            rectangle.Width =  ElementSize + (ElementSize * 2);
                            rectangle.RadiusX = 0.8d;
                            rectangle.RadiusY = 0.8d;
                            rectangle.Fill = new SolidColorBrush(_clr);
                            canvas.Children.Add(rectangle);
                            Canvas.SetLeft(rectangle, list.IndexOf(element) * ElementSize + PaddingElement + i);
                            Canvas.SetTop(rectangle, element * ElementSize + PaddingElement + (i == 0 ? i : i / 2));
                            break;
                        case 6:
                            Polygon poly = new Polygon();
                            poly.Fill = new SolidColorBrush(_clr);
                            //poly.StrokeThickness = 1.0d;
                            //poly.Stroke = new SolidColorBrush(Colors.Gray);
                            var points1 = new PointCollection();
                            points1.Add(new Windows.Foundation.Point(0, 20));
                            points1.Add(new Windows.Foundation.Point(40, 60));
                            points1.Add(new Windows.Foundation.Point(60, 0));
                            poly.Points = points1;
                            canvas.Children.Add(poly);
                            Canvas.SetLeft(poly, list.IndexOf(element) * ElementSize + PaddingElement + i);
                            Canvas.SetTop(poly, element * ElementSize + PaddingElement);
                            break;
                        case 7:
                            Polyline polyln = new Polyline();
                            polyln.StrokeThickness = 4d;
                            polyln.Stroke = new SolidColorBrush(_clr);
                            var points2 = new PointCollection();
                            points2.Add(new Windows.Foundation.Point(5, 100));
                            points2.Add(new Windows.Foundation.Point(30, 70));
                            points2.Add(new Windows.Foundation.Point(65, 70));
                            points2.Add(new Windows.Foundation.Point(90, 100));
                            polyln.Points = points2;
                            canvas.Children.Add(polyln);
                            Canvas.SetLeft(polyln, list.IndexOf(element) * ElementSize + PaddingElement + i);
                            Canvas.SetTop(polyln, element * ElementSize + PaddingElement);
                            break;
                        case 8:
                            TextBlock text = new();
                            text.FontFamily = new FontFamily("Consolas");
                            text.Width = text.Height = ElementSize * 4;
                            text.Foreground = new SolidColorBrush(_clr);
                            text.Text = $"{element}";
                            canvas.Children.Add(text);
                            Canvas.SetLeft(text, list.IndexOf(element) * ElementSize + PaddingElement + i);
                            Canvas.SetTop(text, element * ElementSize + PaddingElement + i);
                            break;
                    }
                }
            });
        }
    }

    /// <summary>
    /// Returns a random selection from <see cref="Microsoft.UI.Colors"/>.
    /// We are interested in the runtime <see cref="System.Reflection.PropertyInfo"/>
    /// from the <see cref="Microsoft.UI.Colors"/> sealed class. We will only add a
    /// property to our collection if it is of type <see cref="Windows.UI.Color"/>.
    /// </summary>
    /// <returns><see cref="List{T}"/></returns>
    List<Windows.UI.Color> GetWinUIColorList()
    {
        List<Windows.UI.Color> colors = new();

        foreach (var color in typeof(Microsoft.UI.Colors).GetRuntimeProperties())
        {
            // We must check the property type before assuming the explicit cast with GetValue.
            if (color != null && color.PropertyType == typeof(Windows.UI.Color))
            {
                try
                {
                    var clr = (Windows.UI.Color)color.GetValue(null);
                    if (clr != null)
                    {
                        if (clr.A == 0 || (clr.R == 0 && clr.G == 0 && clr.B == 0))
                            Debug.WriteLine($"Skipping this color (transparent)");
                        else if (clr.A != 0 && (clr.R <= 40 && clr.G <= 40 && clr.B <= 40))
                            Debug.WriteLine($"Skipping this color (too dark)");
                        else
                            colors.Add(clr);
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine($"Failed to get the value for '{color.Name}'");
                }
            }
            else
            {
                Debug.WriteLine($"I was looking for type {nameof(Windows.UI.Color)} but found {color?.PropertyType} instead.");
            }
        }

        return colors;
    }

    /// <summary>
    /// NOTE: Tile adding/updating is only possible when running as a packaged app.
    /// Requires Fall Creators Update: You must target SDK 16299 and be running build 16299 or later to pin 
    /// secondary tiles from Desktop Bridge apps.
    /// Requires April 2018 version 17134.81 or later: You must be running build 17134.81 or later to send tile 
    /// or badge notifications to secondary tiles from Desktop Bridge apps. Before this .81 servicing update, a
    /// 0x80070490 "Element not found" exception would occur when sending tile or badge notifications to secondary 
    /// tiles from Desktop Bridge apps.
    /// https://learn.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/secondary-tiles-desktop-pinning?tabs=csharpnet6
    /// </summary>
    async Task PinTile()
    {
        // Prepare package images for all four tile sizes in our tile to be pinned as well as for the square30x30 logo used in the Apps view.  
        /*
        Uri square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
        Uri wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png");
        Uri square310x310Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
        Uri square30x30Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
        */

        // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
        // These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
        /*
        string activationArguments = "VisualSortingItems WasPinnedAt=" + DateTime.Now.ToLocalTime().ToString();
        */

        // Create a Secondary tile with all the required arguments.
        // Note the last argument specifies what size the Secondary tile should show up as by default in the Pin to start fly out.
        // It can be set to TileSize.Square150x150, TileSize.Wide310x150, or TileSize.Default.  
        // If set to TileSize.Wide310x150, then the asset for the wide size must be supplied as well.
        // TileSize.Default will default to the wide size if a wide size is provided, and to the medium size otherwise. 
        /*
        SecondaryTile tile = new SecondaryTile("VisualSortingItems", "WinUI3 Visual Sorting", activationArguments, square150x150Logo, TileSize.Square150x150);
        */

        // Initialize the tile with required arguments
        var tile = new Windows.UI.StartScreen.SecondaryTile(
            "VisualSortingItems",
            "WinUI3 Visual Sorting",
            "myActivationArgs",
            new Uri("ms-appx:///Assets/StoreLogo.png"),
            TileSize.Default);

        // The display of the secondary tile name can be controlled for each tile size. The default is false.
        tile.VisualElements.ShowNameOnSquare150x150Logo = true;
        // Specify a foreground text value. The tile background color is inherited from the parent unless a separate value is specified.
        tile.VisualElements.ForegroundText = ForegroundText.Light;

        try
        {
            // 1st technique (newer WinUI3 style)
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(tile, hWnd);

            // 2nd technique (older DesktopBridge style)
            /*
            This causes the error "Invalid cast from 'Windows.UI.StartScreen.SecondaryTile' to 'VisualSortingItems.MainWindow+IInitializeWithWindow'."
            IInitializeWithWindow initWindow = (IInitializeWithWindow)(object)tile;
            initWindow.Initialize(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);
            */

            // Pin the tile
            bool isPinned = await tile.RequestCreateAsync(); // Tile Error: Element not found. (0x80070490)

            if (isPinned)
                Debug.WriteLine($"Secondary tile successfully pinned.");
            else
                Debug.WriteLine($"Secondary tile not pinned.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] PinTile: {ex.Message}", $"{nameof(MainWindow)}");
        }
    }

    // This interface definition is necessary because this is a non-universal
    // app and we have transfer the hwnd for the window to the WinRT object.
    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow
    {
        void Initialize(IntPtr hwnd);
    }
}
