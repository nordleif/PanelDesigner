using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PanelDesigner
{
    /// <summary>
    /// Interaction logic for OutlineViewControl.xaml
    /// </summary>
    public partial class OutlineViewControl : ContentControl
    {
        #region Static Members

        public static readonly DependencyProperty PanelDesignerProperty = DependencyProperty.Register("PanelDesigner", typeof(PanelDesigner), typeof(OutlineViewControl), new PropertyMetadata(PanelDesignerPropertyCallback));

        private static void PanelDesignerPropertyCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = sender as OutlineViewControl;
            if (obj != null)
                obj.OnPanelDesignerChanged(e);
        }

        #endregion
     
        public OutlineViewControl()
        {
            InitializeComponent();
        }

        public PanelDesigner PanelDesigner
        {
            get { return (PanelDesigner)GetValue(PanelDesignerProperty); }
            set { SetValue(PanelDesignerProperty, value); }
        }

        protected virtual void OnPanelDesignerChanged(DependencyPropertyChangedEventArgs e)
        {
            var panelDesigner = e.OldValue as PanelDesigner;
            if (panelDesigner != null)
            {
                panelDesigner.ContentChanged -= panelDesigner_ContentChanged;
                panelDesigner.DesignModeChanged -= panelDesigner_DesignModeChanged;
                panelDesigner.SelectedElementChanged -= panelDesigner_SelectedElementChanged;
                PanelDesigner.SelectedPanelChanged -= PanelDesigner_SelectedPanelChanged;
            }
            
            panelDesigner = e.NewValue as PanelDesigner;
            if (panelDesigner == null && panelDesigner.Content == null)
            {
                treeView.IsEnabled = false;
                treeView.ItemsSource = null;
                return;
            }

            panelDesigner.ContentChanged += panelDesigner_ContentChanged;
            panelDesigner.DesignModeChanged += panelDesigner_DesignModeChanged;
            panelDesigner.SelectedElementChanged += panelDesigner_SelectedElementChanged;
            PanelDesigner.SelectedPanelChanged += PanelDesigner_SelectedPanelChanged;

            treeView.IsEnabled = panelDesigner.DesignMode; 
            if (panelDesigner.Content != null)
                treeView.ItemsSource = new ObservableCollection<OutlineViewItem> { new OutlineViewItem(panelDesigner.Content as FrameworkElement) };
        }

        private void CommandSelectPanel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == PanelDesigner.SelectPanelCommand)
            {
                if (PanelDesigner.DesignMode && PanelDesigner.SelectedElement is Panel && PanelDesigner.SelectedElement != PanelDesigner.SelectedPanel)
                    e.CanExecute = true;
            }
        }

        private void commandSelectPanel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == PanelDesigner.SelectPanelCommand)
                PanelDesigner.SelectedPanel = PanelDesigner.SelectedElement as Panel;
        }

        private OutlineViewItem FindOutlineViewItem(UIElement element)
        {
            var items = treeView.ItemsSource as ObservableCollection<OutlineViewItem>;
            if (items == null)
                return null;

            return FindOutlineViewItem(element, items);
        }


        private OutlineViewItem FindOutlineViewItem(UIElement element, ObservableCollection<OutlineViewItem> items)
        {
            if (items == null)
                return null;

            foreach (var item in items)
            {
                if (item.Element == element)
                    return item;

                var result = FindOutlineViewItem(element, item.Children);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void panelDesigner_ContentChanged(object sender, ContentChangedEventArgs e)
        {
            treeView.ItemsSource = new ObservableCollection<OutlineViewItem> { new OutlineViewItem(e.NewContent as FrameworkElement) };
        }

        private void panelDesigner_DesignModeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            treeView.IsEnabled = PanelDesigner.DesignMode;
        }

        private void panelDesigner_SelectedElementChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldElement = e.OldValue as UIElement;
            if (oldElement != null)
            {
                var item = FindOutlineViewItem(oldElement);
                if (item != null)
                    item.IsSelectedElement = false;
            }

            var newElement = e.NewValue as UIElement;
            if (newElement != null)
            {
                var item = FindOutlineViewItem(newElement);
                if (item != null)
                    item.IsSelectedElement = true;
            }
        }

        private void PanelDesigner_SelectedPanelChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldElement = e.OldValue as UIElement;
            if (oldElement != null)
            {
                var item = FindOutlineViewItem(oldElement);
                if (item != null)
                    item.IsSelectedPanel = false;
            }

            var newElement = e.NewValue as UIElement;
            if (newElement != null)
            {
                var item = FindOutlineViewItem(newElement);
                if (item != null)
                    item.IsSelectedPanel = true;
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = treeView.SelectedItem as OutlineViewItem;
            if (item != null)
                PanelDesigner.SelectedElement = item.Element;
        }
    }
}