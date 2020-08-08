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
    }
}