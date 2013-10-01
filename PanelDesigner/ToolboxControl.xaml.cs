using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// Interaction logic for ToolboxControl.xaml
    /// </summary>
    public partial class ToolboxControl : ContentControl
    {
        #region Static Members

        public static readonly DependencyProperty CategoriesProperty = DependencyProperty.Register("Categories", typeof(ObservableCollection<ToolboxCategory>), typeof(ToolboxControl), new PropertyMetadata(new ObservableCollection<ToolboxCategory>(), CategoriesPropertyCallback));

        private static void CategoriesPropertyCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = sender as ToolboxControl;
            if (obj != null)
                obj.OnCategoriesPropertyChanged(e);
        }

        #endregion

        public ToolboxControl()
        {
            InitializeComponent();
        }

        public ObservableCollection<ToolboxCategory> Categories
        {
            get { return (ObservableCollection<ToolboxCategory>)GetValue(CategoriesProperty); }
            set { SetValue(CategoriesProperty, value); }
        }

        protected virtual void OnCategoriesPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void treeViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void treeViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var item = (TreeViewItem)sender;
            var type = item.DataContext as Type;
            if (type == null)
                return;

            var data = new DataObject(typeof(Type), type);
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
        }
    }
}
