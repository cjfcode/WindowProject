﻿using System;
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
            var window = Window.GetWindow(AssociatedObject);

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
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                // WM_NCCALCSIZE
                case 0x83:
                    var rcClientArea = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                    
                    rcClientArea.Bottom += (int)(WindowChromeHelper.WindowResizeBorderThickness.Bottom / 2);
                    
                    Marshal.StructureToPtr(rcClientArea, lParam, false);
                    
                    handled = true;
                    
                    var retVal = IntPtr.Zero;
                    
                    if (wParam == new IntPtr(1))
                    {
                        retVal = new IntPtr((int)NativeMethods.WVR.REDRAW);
                    }
                    
                    return retVal;
            }

            return IntPtr.Zero;
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