using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PanelDesigner
{
    public class ElementDroppedEventArgs : EventArgs
    {
        public ElementDroppedEventArgs(FrameworkElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            Element = element;
        }

        public FrameworkElement Element { get; protected set; }
    }
}
