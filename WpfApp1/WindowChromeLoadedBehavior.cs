using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shell;
using Microsoft.Xaml.Behaviors;

namespace WpfApp1
{
    public class WindowChromeLoadedBehavior : Behavior<FrameworkElement>
    {
        private Window window;
        private bool loaded;

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
            window = Window.GetWindow(AssociatedObject);

            if (window == null) return;

            Task.Delay(5).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
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

            var hWnd = new WindowInteropHelper(window).Handle;
            HwndSource.FromHwnd(hWnd)?.AddHook(WndProc);

            // fixes the flickering during initial window transition
            // and also oddly fixes the flickering when activating
            // the maximized window from a minimized state on preview
            // without the need to set bottom margin to 10
            // (no idea why this would affect this!)
            Task.Delay(300).ContinueWith(_ =>
            {
                loaded = true;
            });
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_NCPAINT:
                    RemoveFrame();
                    handled = false;
                    break;

                case NativeMethods.WM_NCCALCSIZE:

                    handled = true;

                    if(loaded)
                    {
                        var rcClientArea = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                        rcClientArea.Bottom += (int)(WindowChromeHelper.WindowResizeBorderThickness.Bottom / 2);
                        Marshal.StructureToPtr(rcClientArea, lParam, false);
                    }

                    return wParam == new IntPtr(1) ? new IntPtr((int)NativeMethods.WVR.REDRAW) : IntPtr.Zero;
            }

            return IntPtr.Zero;
        }

        private void RemoveFrame()
        {
            if (Environment.OSVersion.Version.Major >= 6 && NativeMethods.IsDwmAvailable())
            {
                if (NativeMethods.DwmIsCompositionEnabled() && SystemParameters.DropShadow)
                {
                    // to get the aero shadow back, margins have to be set on at least one side
                    // don't use negative values for the margins because it causes flickering when restoring or maximizing the window
                    // setting the margin on the left or top sides seem best, since the window never appears to flicker there on resizing
                    // but the right and bottom sometimes do, otherwise there will occasionally be white flicker when resizing the window.
                    NativeMethods.MARGINS margins;

                    margins.bottomHeight = 0;
                    margins.leftWidth = 1;
                    margins.rightWidth = 0;
                    margins.topHeight = 0;

                    var helper = new WindowInteropHelper(window);

                    NativeMethods.DwmExtendFrameIntoClientArea(helper.Handle, ref margins);
                }
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public static RECT Empty;

            public int Width => Math.Abs(Right - Left);

            public int Height => (Bottom - Top);

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(RECT rcSrc)
            {
                Left = rcSrc.Left;
                Top = rcSrc.Top;
                Right = rcSrc.Right;
                Bottom = rcSrc.Bottom;
            }

            public RECT(Rectangle rectangle) : this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
            {
            }

            public bool IsEmpty
            {
                get
                {
                    if (Left < Right)
                    {
                        return (Top >= Bottom);
                    }
                    return true;
                }
            }

            public override string ToString()
            {
                if (this == Empty)
                {
                    return "RECT {Empty}";
                }
                return string.Concat("RECT { left : ", Left, " / top : ", Top, " / right : ", Right, " / bottom : ", Bottom, " }");
            }

            public override bool Equals(object obj)
            {
                return ((obj is Rect) && (this == ((RECT)obj)));
            }

            public override int GetHashCode()
            {
                return ((Left.GetHashCode() + Top.GetHashCode()) + Right.GetHashCode()) + Bottom.GetHashCode();
            }

            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return ((((rect1.Left == rect2.Left) && (rect1.Top == rect2.Top)) && (rect1.Right == rect2.Right)) && (rect1.Bottom == rect2.Bottom));
            }

            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }

            static RECT()
            {
                Empty = new RECT();
            }
        }

    }
}