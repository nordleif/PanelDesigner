using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PanelDesigner;

namespace PanelDesignerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void menuItemDesignMode_Click(object sender, RoutedEventArgs e)
        {
            panelDesigner.DesignMode = !panelDesigner.DesignMode;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            /*
            var assembly = System.Reflection.Assembly.Load("PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            using (var stream = assembly.GetManifestResourceStream("System.Windows.Controls.Button.bmp"))
            using (var reader = new System.IO.BinaryReader(stream))
            {
                byte[] image = reader.ReadBytes((int)stream.Length);
                System.IO.File.WriteAllBytes(@"d:\temp\button.bmp", image);
            }
            */

            //var assembly = System.Reflection.Assembly.Load("System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            //using (var stream = assembly.GetManifestResourceStream("System.Windows.Forms.Button.bmp"))
            //using (var reader = new System.IO.BinaryReader(stream))
            //{
            //    byte[] image = reader.ReadBytes((int)stream.Length);
            //    System.IO.File.WriteAllBytes(@"d:\temp\button.bmp", image);
            //}

            var generalCategory = new ToolboxCategory("Controls");
            generalCategory.Types.Add(typeof(Button));
            generalCategory.Types.Add(typeof(Label));
            generalCategory.Types.Add(typeof(TextBox));
            generalCategory.Types.Add(typeof(CheckBox));
            generalCategory.Types.Add(typeof(TestControl));

            var panelCategory = new ToolboxCategory("Panels");
            panelCategory.Types.Add(typeof(Canvas));
            panelCategory.Types.Add(typeof(Grid));


            toolbox.Categories.Add(generalCategory);
            toolbox.Categories.Add(panelCategory);

            outlineView.PanelDesigner = panelDesigner;

            panelDesigner.SelectedElementChanged += panelDesigner_SelectedElementChanged;
        }

        private void panelDesigner_SelectedElementChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //propertyGrid.SelectedObject = panelDesigner.SelectedElement;
        }

        private void menuItemDelete_Click_1(object sender, RoutedEventArgs e)
        {
            if (!panelDesigner.DesignMode)
                return;

            if (panelDesigner.SelectedElements == null || panelDesigner.SelectedElements.Count == 0)
                return;

            panelDesigner.Delete();
        }

        private void menuItemTest_Click(object sender, RoutedEventArgs e)
        {
            panelDesigner.SelectedPanel = canvas;

        }

        private void menuItemSelectPanel_Click_1(object sender, RoutedEventArgs e)
        {
            var panel = panelDesigner.SelectedElement as Panel;
            if (panel != null)
                panelDesigner.SelectedPanel = panel;
        }

    }
}
