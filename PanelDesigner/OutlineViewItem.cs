using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PanelDesigner
{
    internal class OutlineViewItem : INotifyPropertyChanged
    {
        private ObservableCollection<OutlineViewItem> m_children;
        private FrameworkElement m_element;
        private bool m_isSelectedElement;
        private bool m_isSelectedPanel;

        public OutlineViewItem(FrameworkElement element)
        {
            Element = element;
        }

        public ObservableCollection<OutlineViewItem> Children
        {
            get { return m_children; }
            protected set
            {
                if (m_children != value)
                {
                    m_children = value;
                    OnPropertyChanged("Children");
                }
            }
        }

        public FrameworkElement Element
        {
            get { return m_element; }
            protected set
            {
                if (m_element != value)
                {
                    if (m_element != null)
                    {
                        m_element.LayoutUpdated -= m_element_LayoutUpdated;
                        m_children.Clear();
                    }
                    m_element = value;
                    m_element_LayoutUpdated(null, new EventArgs());
                    m_element.LayoutUpdated += m_element_LayoutUpdated;
                    OnPropertyChanged("Element");
                }
            }
        }

        public bool IsSelectedElement
        {
            get { return m_isSelectedElement; }
            set
            {
                if (m_isSelectedElement != value)
                {
                    m_isSelectedElement = value;
                    OnPropertyChanged("IsSelectedElement");
                }
            }
        }

        public bool IsSelectedPanel
        {
            get { return m_isSelectedPanel; }
            set
            {
                if (m_isSelectedPanel != value)
                {
                    m_isSelectedPanel = value;
                    OnPropertyChanged("IsSelectedPanel");
                }
            }
        }

        private void m_element_LayoutUpdated(object sender, EventArgs e)
        {
            var panel = m_element as Panel;
            if (panel == null)
                return;

            if (Children == null)
                Children = new ObservableCollection<OutlineViewItem>();

            if (Children.Count == panel.Children.Count)
                return;

            if (panel.Children.Count < Children.Count)
            {
                // Element removed
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    var item = Children[i];
                    if (!panel.Children.Contains(item.Element))
                        Children.Remove(item);
                }
            }

            if (panel.Children.Count > Children.Count)
            {
                // Element added
                foreach (FrameworkElement element in panel.Children)
                {
                    var item = Children.SingleOrDefault(i => i.Element == element);
                    if (item == null)
                        Children.Add(new OutlineViewItem(element));
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}