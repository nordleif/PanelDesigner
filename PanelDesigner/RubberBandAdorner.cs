using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PanelDesigner
{
    internal class RubberbandAdorner : Adorner
    {
        private Canvas m_adornerCanvas;
        private Point? m_endPoint;
        private PanelDesigner m_panelDesigner;
        private Rectangle m_rubberBand;
        private Point? m_startPoint;
        private VisualCollection m_visualChildren;

        public RubberbandAdorner(PanelDesigner panelDesigner, UIElement adornedElement, Point? startPoint)
            : base(adornedElement)
        {
            m_panelDesigner = panelDesigner;
            m_startPoint = startPoint;

            m_visualChildren = new VisualCollection(this);

            m_adornerCanvas = new Canvas();
            m_adornerCanvas.Background = Brushes.Transparent;
            //m_adornerCanvas.Background = Brushes.Yellow;
            m_visualChildren.Add(m_adornerCanvas);

            m_rubberBand = new Rectangle();
            m_rubberBand.Stroke = Brushes.Black;
            m_rubberBand.StrokeThickness = 1;
            m_rubberBand.StrokeDashArray = new DoubleCollection(new double[] { 2 });
            m_adornerCanvas.Children.Add(m_rubberBand);
        }

        protected override int VisualChildrenCount
        {
            get { return m_visualChildren.Count; }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            m_adornerCanvas.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return m_visualChildren[index];
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            e.Handled = true;

            if (!IsMouseCaptured)
                CaptureMouse();

            m_endPoint = e.GetPosition(this);
            UpdateRubberBand();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();

            var adornerLayer = Parent as AdornerLayer;
            if (adornerLayer != null)
                adornerLayer.Remove(this);

            SelectElementsWithinRubberBand();
        }

        private void UpdateRubberBand()
        {
            var left = Math.Min(m_startPoint.Value.X, m_endPoint.Value.X);
            var top = Math.Min(m_startPoint.Value.Y, m_endPoint.Value.Y);
            var width = Math.Abs(m_startPoint.Value.X - m_endPoint.Value.X);
            var height = Math.Abs(m_startPoint.Value.Y - m_endPoint.Value.Y);

            m_rubberBand.Width = width;
            m_rubberBand.Height = height;

            Canvas.SetLeft(m_rubberBand, left);
            Canvas.SetTop(m_rubberBand, top);
        }

        private void SelectElementsWithinRubberBand()
        {
            m_panelDesigner.SelectedElements.Clear();

            if (m_endPoint == null)
                return;

            var rubberBand = new Rect(m_startPoint.Value, m_endPoint.Value);
            foreach (FrameworkElement element in m_panelDesigner.SelectedPanel.Children)
            {
                if (element == m_panelDesigner.SelectedPanel)
                    continue;

                var elementRect = VisualTreeHelper.GetDescendantBounds(element);
                var elementBounds = element.TransformToAncestor(m_panelDesigner.SelectedPanel).TransformBounds(elementRect);

                if (rubberBand.IntersectsWith(elementBounds))
                {
                    if (!m_panelDesigner.SelectedElements.Contains(element))
                        m_panelDesigner.SelectedElements.Add(element);
                }
                else
                {
                    if (m_panelDesigner.SelectedElements.Contains(element))
                        m_panelDesigner.SelectedElements.Remove(element);
                }
            }
        }
    }
}