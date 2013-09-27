using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PanelDesigner
{
    internal class ResizeAdorner : Adorner
    {
        private ResizeThumb m_bottom;
        private ResizeThumb m_bottomLeft;
        private ResizeThumb m_bottomRight;
        private ResizeThumb m_left;
        private PanelDesigner m_panelDesigner;
        private Rectangle m_rectangle;
        private ResizeThumb m_right;
        private ResizeThumb m_top;
        private ResizeThumb m_topLeft;
        private ResizeThumb m_topRight;
        private VisualCollection m_visualChildren;

        public ResizeAdorner(PanelDesigner panelDesigner, UIElement adornedElement)
            : base(adornedElement)
        {
            m_panelDesigner = panelDesigner;
            m_panelDesigner.SelectedElementChanged += m_panelDesigner_SelectedElementChanged;
            m_visualChildren = new VisualCollection(this);

            m_rectangle = new Rectangle();
            m_rectangle.Stroke = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;
            m_rectangle.StrokeThickness = 1;
            m_visualChildren.Add(m_rectangle);

            BuildResizeAdorner(ref m_bottom, Cursors.SizeNS);
            BuildResizeAdorner(ref m_bottomLeft, Cursors.SizeNESW);
            BuildResizeAdorner(ref m_bottomRight, Cursors.SizeNWSE);
            BuildResizeAdorner(ref m_left, Cursors.SizeWE);
            BuildResizeAdorner(ref m_right, Cursors.SizeWE);
            BuildResizeAdorner(ref m_top, Cursors.SizeNS);
            BuildResizeAdorner(ref m_topLeft, Cursors.SizeNWSE);
            BuildResizeAdorner(ref m_topRight, Cursors.SizeNESW);

            m_bottom.DragDelta += new DragDeltaEventHandler(HandleBottom);
            m_bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            m_bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            m_left.DragDelta += new DragDeltaEventHandler(HandleLeft);
            m_right.DragDelta += new DragDeltaEventHandler(HandleRight);
            m_top.DragDelta += new DragDeltaEventHandler(HandleTop);
            m_topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            m_topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
        }

        protected override int VisualChildrenCount
        {
            get { return m_visualChildren.Count; }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var width = DesiredSize.Width;
            var height = DesiredSize.Height;

            m_bottom.Arrange(new Rect(0, (height / 2) + (m_bottomLeft.Height / 2), width, height));
            m_bottomLeft.Arrange(new Rect(-(width / 2) - (m_bottomLeft.Width / 2), (height / 2) + (m_bottomLeft.Height / 2), width, height));
            m_bottomRight.Arrange(new Rect((width / 2) + (m_bottomRight.Width / 2), (height / 2) + (m_bottomRight.Height / 2), width, height));
            m_left.Arrange(new Rect(-(width / 2) - (m_topLeft.Width / 2), 0, width, height));
            m_rectangle.Arrange(new Rect(-1, -1, width + 2, height + 2));
            m_right.Arrange(new Rect((width / 2) + (m_topRight.Width / 2), 0, width, height));
            m_top.Arrange(new Rect(0, -(height / 2) - (m_topLeft.Height / 2), width, height));
            m_topLeft.Arrange(new Rect(-(width / 2) - (m_topLeft.Width / 2), -(height / 2) - (m_topLeft.Height / 2), width, height));
            m_topRight.Arrange(new Rect((width / 2) + (m_topRight.Width / 2), -(height / 2) - (m_topRight.Height / 2), width, height));

            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return m_visualChildren[index];
        }

        private void BuildResizeAdorner(ref ResizeThumb cornerThumb, Cursor cursor)
        {
            if (cornerThumb != null)
                return;

            cornerThumb = new ResizeThumb();
            cornerThumb.BorderThickness = new Thickness(1);
            cornerThumb.BorderBrush = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;
            cornerThumb.Background = Brushes.White; 
            cornerThumb.Height = 6;
            cornerThumb.Width = 6;

            cornerThumb.Cursor = cursor;

            m_visualChildren.Add(cornerThumb);
        }

        private void HandleBottom(object sender, DragDeltaEventArgs e)
        {
            m_panelDesigner.Resize(0, 0, 0, e.VerticalChange);
        }

        private void HandleBottomLeft(object sender, DragDeltaEventArgs e)
        {
            m_panelDesigner.Resize(e.HorizontalChange, 0, 0, e.VerticalChange);
        }

        private void HandleBottomRight(object sender, DragDeltaEventArgs e)
        {
            m_panelDesigner.Resize(0, 0, e.HorizontalChange, e.VerticalChange);
        }

        private void HandleLeft(object sender, DragDeltaEventArgs e)
        {
            m_panelDesigner.Resize(e.HorizontalChange, 0, 0, 0);
        }

        private void HandleRight(object sender, DragDeltaEventArgs e)
        {
            m_panelDesigner.Resize(0, 0, e.HorizontalChange, 0);
        }

        private void HandleTop(object sender, DragDeltaEventArgs e)
        {
            m_panelDesigner.Resize(0, e.VerticalChange, 0, 0);
        }

        private void HandleTopLeft(object sender, DragDeltaEventArgs e)
        {
            m_panelDesigner.Resize(e.HorizontalChange, e.VerticalChange, 0, 0);
        }

        private void HandleTopRight(object sender, DragDeltaEventArgs e)
        {
            m_panelDesigner.Resize(0, e.VerticalChange, e.HorizontalChange, 0);
        }

        private void m_panelDesigner_SelectedElementChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_bottom.BorderBrush = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;
            m_bottomLeft.BorderBrush = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;
            m_bottomRight.BorderBrush = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;
            m_left.BorderBrush = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;
            m_rectangle.Stroke = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;
            m_right.BorderBrush = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;
            m_top.BorderBrush = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;
            m_topLeft.BorderBrush = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;
            m_topRight.BorderBrush = m_panelDesigner.SelectedElement == AdornedElement ? Brushes.CornflowerBlue : Brushes.LightBlue;  
        }
    }
}
