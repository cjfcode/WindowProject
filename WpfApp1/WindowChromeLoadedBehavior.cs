using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using Microsoft.Xaml.Behaviors;

namespace WpfApp1
{
    public class WindowChromeLoadedBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Task.Delay(5).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    var window = Window.GetWindow(AssociatedObject);

                    if (window == null) return;

                    var oldWindowChrome = WindowChrome.GetWindowChrome(window);

                    if (oldWindowChrome == null) return;

                    var newWindowChrome = new WindowChrome
                    {
                        CaptionHeight = oldWindowChrome.CaptionHeight,
                        CornerRadius = oldWindowChrome.CornerRadius,
                        GlassFrameThickness = new Thickness(0, 0, 0, 1),
                        NonClientFrameEdges = NonClientFrameEdges.Bottom,
                        ResizeBorderThickness = oldWindowChrome.ResizeBorderThickness,
                        UseAeroCaptionButtons = oldWindowChrome.UseAeroCaptionButtons
                    };

                    WindowChrome.SetWindowChrome(window, newWindowChrome);
                });
            });
        }
    }
}