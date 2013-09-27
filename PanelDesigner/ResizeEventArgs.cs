using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDesigner
{
    public class ResizeEventArgs : EventArgs
    {
        public ResizeEventArgs(FrameworkElementObservableCollection elements, double left, double top, double right, double bottom)
            : this(elements, left, top, right, bottom, false)
        {

        }

        public ResizeEventArgs(FrameworkElementObservableCollection elements, double left, double top, double right, double bottom, bool snapToElements)
        {
            Elements = elements;
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            SnapToElements = snapToElements;
        }

        public double Bottom { get; set; }
        public FrameworkElementObservableCollection Elements { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public bool SnapToElements { get; set; }
        public double Top { get; set; }
    }
}
