using System;
using System.Windows;

namespace WpfApp1
{
    public class MainWindowViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the <see cref="Thickness"/> used when inflating the margin on maximize.
        /// </summary>
        public Thickness MaximizeFix { get; private set; } = new Thickness
        (
            // thickness of left border
            SystemParameters.WindowNonClientFrameThickness.Left
            + SystemParameters.WindowResizeBorderThickness.Left,
            
            // thickness of top border
            SystemParameters.WindowNonClientFrameThickness.Top
            + SystemParameters.WindowResizeBorderThickness.Top
            - SystemParameters.CaptionHeight
            - SystemParameters.BorderWidth,
            
            // thickness of right border
            SystemParameters.WindowNonClientFrameThickness.Right
            + SystemParameters.WindowResizeBorderThickness.Right,
            
            // thickness of bottom border
            // setting this to zero prevents white line on bottom from showing when maximizing with maximize fix
            0
            //SystemParameters.WindowNonClientFrameThickness.Bottom + SystemParameters.WindowResizeBorderThickness.Bottom
        );

        public RelayCommand MinimizeMainWindow { get => new RelayCommand(MinimizeWindow); }

        public RelayCommand MaximizeMainWindow { get => new RelayCommand(MaximizeWindow); }

        public RelayCommand CloseMainWindow { get => new RelayCommand(CloseWindow); }

        public RelayCommand NoBorderCommand { get => new RelayCommand(NoBorder); }
        public RelayCommand BorderCommand { get => new RelayCommand(Border); }

        private void NoBorder()
        {
            Application.Current.Resources.MergedDictionaries.Remove(Application.Current.Resources.MergedDictionaries[3]);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/NoBorder.xaml", UriKind.Relative) });
        }

        private void Border()
        {
            Application.Current.Resources.MergedDictionaries.Remove(Application.Current.Resources.MergedDictionaries[3]);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/Border.xaml", UriKind.Relative) });
        }

        private void MinimizeWindow()
        {
            SystemCommands.MinimizeWindow(Application.Current.MainWindow);
        }

        private void MaximizeWindow()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Normal)
            {
                SystemCommands.MaximizeWindow(Application.Current.MainWindow);
            }
            else if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                SystemCommands.RestoreWindow(Application.Current.MainWindow);
            }
        }

        private void CloseWindow()
        {
            SystemCommands.CloseWindow(Application.Current.MainWindow);
        }
    }
}