using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PanelDesigner
{
    public class ToolboxCategory
    {
        private ObservableCollection<Type> m_types = new ObservableCollection<Type>();

        public ToolboxCategory()
        {

        }

        public ToolboxCategory(string categoryName)
        {
            CategoryName = categoryName;
        }

        public string CategoryName { get; set; }

        public ObservableCollection<Type> Types
        {
            get { return m_types; }
        }
    }
}
