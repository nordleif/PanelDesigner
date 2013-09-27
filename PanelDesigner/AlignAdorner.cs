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
    internal class AlignAdorner : Adorner
    {
        private Canvas m_adornerCanvas;
        private Line m_bottomLine;
        private Line m_leftLine;
        private PanelDesigner m_panelDesigner;
        private Line m_rightLine;
        private Line m_topLine;
        private VisualCollection m_visualChildren;

        public AlignAdorner(PanelDesigner panelDesigner)
            : base(panelDesigner)
        {
            m_panelDesigner = panelDesigner;
            m_visualChildren = new VisualCollection(this);

            m_adornerCanvas = new Canvas();
            m_adornerCanvas.Background = null;
            m_visualChildren.Add(m_adornerCanvas);

            m_leftLine = new Line();
            m_leftLine.Stroke = Brushes.Green;
            m_leftLine.StrokeDashArray = new DoubleCollection(new double[] { 2 });
            m_leftLine.StrokeThickness = 1;
            m_leftLine.Visibility = Visibility.Collapsed;
            m_adornerCanvas.Children.Add(m_leftLine);

            m_topLine = new Line();
            m_topLine.Stroke = Brushes.Green;
            m_topLine.StrokeDashArray = new DoubleCollection(new double[] { 2 });
            m_topLine.StrokeThickness = 1;
            m_topLine.Visibility = Visibility.Collapsed;
            m_adornerCanvas.Children.Add(m_topLine);

            m_rightLine = new Line();
            m_rightLine.Stroke = Brushes.Green;
            m_rightLine.StrokeDashArray = new DoubleCollection(new double[] { 2 });
            m_rightLine.StrokeThickness = 1;
            m_rightLine.Visibility = Visibility.Collapsed;
            m_adornerCanvas.Children.Add(m_rightLine);

            m_bottomLine = new Line();
            m_bottomLine.Stroke = Brushes.Green;
            m_bottomLine.StrokeDashArray = new DoubleCollection(new double[] { 2 });
            m_bottomLine.StrokeThickness = 1;
            m_bottomLine.Visibility = Visibility.Collapsed; 
            m_adornerCanvas.Children.Add(m_bottomLine);
        }

        public void RenderLines()
        {
            bool isLeftLineVisible = false;
            bool isTopLineVisible = false;
            bool isRightLineVisible = false;
            bool isBottomLineVisible = false;

            if (m_panelDesigner.SelectedElement == m_panelDesigner.SelectedPanel)
                return;

            var selectedElements = m_panelDesigner.SelectedElements;
            var selectedElement = m_panelDesigner.SelectedElement;
            var selectedElementRect = VisualTreeHelper.GetDescendantBounds(selectedElement);
            var selectedElementBounds = selectedElement.TransformToAncestor(m_panelDesigner).TransformBounds(selectedElementRect);
            var selectedElementLeft = Math.Abs(selectedElementBounds.Left);
            var selectedElementRight = Math.Abs(selectedElementBounds.Right);
            var selectedElementTop = Math.Abs(selectedElementBounds.Top);
            var selectedElementBottom = Math.Abs(selectedElementBounds.Bottom);

            m_leftLine.X1 = selectedElementLeft;
            m_leftLine.X2 = selectedElementLeft;
            m_leftLine.Y1 = selectedElementTop;
            m_leftLine.Y2 = selectedElementBottom;
            m_topLine.X1 = selectedElementLeft;
            m_topLine.X2 = selectedElementRight;
            m_topLine.Y1 = selectedElementTop;
            m_topLine.Y2 = selectedElementTop;
            m_rightLine.X1 = selectedElementRight;
            m_rightLine.X2 = selectedElementRight;
            m_rightLine.Y1 = selectedElementTop;
            m_rightLine.Y2 = selectedElementBottom;
            m_bottomLine.X1 = selectedElementLeft;
            m_bottomLine.X2 = selectedElementRight;
            m_bottomLine.Y1 = selectedElementBottom;
            m_bottomLine.Y2 = selectedElementBottom;

            foreach (FrameworkElement element in m_panelDesigner.GetDescendants())
            {
                if (m_panelDesigner.SelectedElements.Contains(element))
                    continue;

                var elementRect = VisualTreeHelper.GetDescendantBounds(element);
                var elementBounds = element.TransformToAncestor(m_panelDesigner).TransformBounds(elementRect);
                var elementLeft = Math.Abs(elementBounds.Left);
                var elementRight = Math.Abs(elementBounds.Right);
                var elementTop = Math.Abs(elementBounds.Top);
                var elementBottom = Math.Abs(elementBounds.Bottom);

                if (elementLeft == selectedElementLeft || elementRight == selectedElementLeft)
                {
                    Debug.WriteLine(string.Format("ElementLeft: {0}", element.Name));

                    isLeftLineVisible = true;
                    if (elementTop < selectedElementTop) m_leftLine.Y1 = elementTop;
                    if (elementBottom > selectedElementBottom) m_leftLine.Y2 = elementBottom;
                }

                if (elementTop == selectedElementTop || elementBottom == selectedElementTop)
                {
                    Debug.WriteLine(string.Format("ElementTop: {0}", element.Name));

                    isTopLineVisible = true;
                    if (elementLeft < selectedElementLeft) m_topLine.X1 = elementLeft;
                    if (elementRight > selectedElementRight) m_topLine.X2 = elementRight;
                }

                if (elementLeft == selectedElementRight || elementRight == selectedElementRight)
                {
                    Debug.WriteLine(string.Format("ElementRight: {0}", element.Name));

                    isRightLineVisible = true;
                    if (elementTop < selectedElementTop) m_rightLine.Y1 = elementTop;
                    if (elementBottom > selectedElementBottom) m_rightLine.Y2 = elementBottom;
                }

                if (elementTop == selectedElementBottom || elementBottom == selectedElementBottom)
                {
                    Debug.WriteLine(string.Format("ElementBottom: {0}", element.Name));

                    isBottomLineVisible = true;
                    if (elementLeft < selectedElementLeft) m_bottomLine.X1 = elementLeft;
                    if (elementRight > selectedElementRight) m_bottomLine.X2 = elementRight;
                }
            }

            m_leftLine.Visibility = isLeftLineVisible ? Visibility.Visible : Visibility.Collapsed;
            m_topLine.Visibility = isTopLineVisible ? Visibility.Visible : Visibility.Collapsed;
            m_rightLine.Visibility = isRightLineVisible ? Visibility.Visible : Visibility.Collapsed;
            m_bottomLine.Visibility = isBottomLineVisible ? Visibility.Visible : Visibility.Collapsed;
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
    }
}