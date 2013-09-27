using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace PanelDesigner
{
    internal class ResizeThumb : Thumb
    {
        static ResizeThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeThumb), new FrameworkPropertyMetadata(typeof(ResizeThumb)));
        }
    }
}