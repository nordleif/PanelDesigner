using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace PanelDesigner
{
    public class FrameworkElementObservableCollection : ObservableCollection<FrameworkElement>
    {
        protected override void ClearItems()
        {
            // HACK: There are no items in e.OldItems on Clearitems.
            for (int i = base.Count; i > 0; i--)
            {
                var item = base.Items[i - 1];
                base.Remove(item);
            }
            base.ClearItems();
        }
    }
}
