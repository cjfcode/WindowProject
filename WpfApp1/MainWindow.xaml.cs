using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // setting a delay, even of 5 milliseconds, seems to prevent the flickering on startup caused
            // by setting the windowChrome GlassFrameThickness and NonClientFrameEdges to non-zero values
            // or None, respectively, and then changing them in the code.
            Task.Delay(5).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    // Setting GlassFrameThickness gives us the default shadow and
                    // makes NonClientFrameEdges.Bottom less noticeable, while
                    // NonClientFrameEdges.Bottom fixes a lot of issues with WindowChrome
                    // including:
                    //            (1) 8 pixels of empty margin when peeking a maximized window
                    //            (2) No aero glass transparent rectangle effect on aero peek
                    //            (3) Jittery window resizing from corners other than bottom right
                    //                (probably because it forces the window to change position)
                    //            (4) Blurry focus effect when selecting the minimized window from task view
                    //
                    mainWindowChrome.GlassFrameThickness = new Thickness(0, 0, 0, 1);
                    mainWindowChrome.NonClientFrameEdges = System.Windows.Shell.NonClientFrameEdges.Bottom;
                });
            });

            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hWnd).AddHook(new HwndSourceHook(NativeMethods.WndProc));
        }
    }
}