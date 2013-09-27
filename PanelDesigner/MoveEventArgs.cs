using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDesigner
{
    public class MoveEventArgs : EventArgs
    {
        public MoveEventArgs(FrameworkElementObservableCollection elements, double horizontalChange, double verticalChange)
            : this(elements, horizontalChange, verticalChange, false)
        {

        }

        public MoveEventArgs(FrameworkElementObservableCollection elements, double horizontalChange, double verticalChange, bool snapToElements)
        {
            Elements = elements;
            HorizontalChange = horizontalChange;
            VerticalChange = verticalChange;
            SnapToElements = snapToElements;
        }

        public FrameworkElementObservableCollection Elements { get; set; }
        public double HorizontalChange { get; set; }
        public bool SnapToElements { get; set; }
        public double VerticalChange { get; set;}
    }
}
