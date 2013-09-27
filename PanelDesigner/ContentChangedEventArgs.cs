using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PanelDesigner
{
    public class ContentChangedEventArgs : EventArgs
    {
        public ContentChangedEventArgs(object oldContent, object newContent)
        {
            OldContent = oldContent;
            NewContent = newContent;
        }

        public object OldContent { get; protected set; }
        public object NewContent { get; protected set; }
    }
}