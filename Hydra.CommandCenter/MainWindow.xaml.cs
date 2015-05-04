using System.ComponentModel;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Hydra.CommandCenter
{
    public static class Navigation
    {
        private static MetroWindow _mainWindow;
        public static void RegisterWindow( MetroWindow mainWindow )
        {
            _mainWindow = mainWindow;
        }

        public static async Task ShowMessageAsync( string title, string message )
        {
            await _mainWindow.ShowMessageAsync( title, message );
        }

        public static async Task<ProgressDialogController> ShowProgressAsync( string title, string message )
        {
            return await _mainWindow.ShowProgressAsync( title, message );
        }

        public static void ShowOverlay()
        {
            _mainWindow.ShowOverlay();
        }

        public static void HideOverlay()
        {
            _mainWindow.HideOverlay();
        }

        public static async Task<string> ShowInputAsync( string title, string message, string defaultText = null )
        {
            var settings = new MetroDialogSettings()
            {
                DefaultText = defaultText
            };
            return await _mainWindow.ShowInputAsync( title, message, settings );
        }
    }

    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
            Navigation.RegisterWindow( this );
        }

        private void OnWindowClosing( object sender, CancelEventArgs e )
        {
            ViewModel.OnClosing();
        }
    }
}
