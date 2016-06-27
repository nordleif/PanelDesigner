using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    public class PanelDesigner : ContentControl
    {
        #region Static Members

        public static readonly DependencyProperty DesignModeProperty = DependencyProperty.Register("DesignMode", typeof(bool), typeof(PanelDesigner), new PropertyMetadata(DesignModePropertyCallback));
        public static readonly DependencyProperty SelectedElementProperty = DependencyProperty.Register("SelectedElement", typeof(FrameworkElement), typeof(PanelDesigner), new PropertyMetadata(SelectedElementPropertyCallback));
        public static readonly DependencyProperty SelectedElementsProperty = DependencyProperty.Register("SelectedElements", typeof(FrameworkElementObservableCollection), typeof(PanelDesigner), new PropertyMetadata(new FrameworkElementObservableCollection(), SelectedElementsPropertyCallback));
        public static readonly DependencyProperty SelectedPanelProperty = DependencyProperty.Register("SelectedPanel", typeof(Panel), typeof(PanelDesigner), new PropertyMetadata(SelectedPanelPropertyCallback));
        public static readonly DependencyProperty SnapToElementsProperty = DependencyProperty.Register("SnapToElements", typeof(bool), typeof(PanelDesigner), new PropertyMetadata(true));

        public static readonly RoutedCommand BringToFrontCommand = new RoutedCommand();
        public static readonly RoutedCommand DeleteCommand = new RoutedCommand();
        public static readonly RoutedCommand SelectPanelCommand = new RoutedCommand();
        public static readonly RoutedCommand SendToBackCommand = new RoutedCommand();

        private static void DesignModePropertyCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = sender as PanelDesigner;
            if (obj != null)
                obj.OnDesignModeChanged(e);
        }

        private static void SelectedElementPropertyCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = sender as PanelDesigner;
            if (obj != null)
                obj.OnSelectedElementChanged(e);
        }

        private static void SelectedElementsPropertyCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = sender as PanelDesigner;
            if (obj != null)
                obj.OnSelectedElementsChanged(e);
        }

        private static void SelectedPanelPropertyCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = sender as PanelDesigner;
            if (obj != null)
                obj.OnSelectedPanelChanged(e);
        }

        #endregion

        private List<Adorner> m_adorners = new List<Adorner>();
        private AlignAdorner m_alignAdorner;
        private bool m_ignoreOnSelectedElementChanged;
        private bool m_isDragging;
        private Point m_mouseDownOffsetPoint;
        private Point m_mouseDownStartPoint;

        public PanelDesigner()
        {
            CommandManager.AddCanExecuteHandler(this, CommandCanExecuteHandler);
            CommandManager.AddExecutedHandler(this, CommandExecuteHandler);
            SelectedElements.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedElements_CollectionChanged);
        }

        public bool DesignMode
        {
            get { return (bool)GetValue(DesignModeProperty); }
            set { SetValue(DesignModeProperty, value); }
        }

        public FrameworkElement SelectedElement
        {
            get { return (FrameworkElement)GetValue(SelectedElementProperty); }
            set { SetValue(SelectedElementProperty, value); }
        }

        public FrameworkElementObservableCollection SelectedElements
        {
            get { return (FrameworkElementObservableCollection)GetValue(SelectedElementsProperty); }
        }

        public Panel SelectedPanel
        {
            get { return (Panel)GetValue(SelectedPanelProperty); }
            set { SetValue(SelectedPanelProperty, value); }
        }

        public bool SnapToElements
        {
            get { return (bool)GetValue(SnapToElementsProperty); }
            set { SetValue(SnapToElementsProperty, value); }
        }

        public void AddService(object obj)
        {
            OnAddService(obj);
        }

        public void Arrange(ArrangeAction arrangeAction)
        {
            OnArrange(arrangeAction);
        }

        public void BringToBack()
        {
            OnBringToBack();
        }

        public void ChangeVisibility(Visibility visibility)
        {
            OnChangeVisibility(visibility);
        }

        public void BringToFront()
        {
            OnBringToFront();
        }

        public void Delete()
        {
            OnDelete();
        }

        public T GetService<T>() where T : class
        {
            return OnGetService<T>();
        }

        public void Move(double horizontalChange, double verticalChange)
        {
            OnMove(new MoveEventArgs(SelectedElements, horizontalChange, verticalChange, Mouse.LeftButton == MouseButtonState.Pressed));
        }

        public void Resize(double left, double top, double right, double bottom)
        {
            OnResize(new ResizeEventArgs(SelectedElements, left, top, right, bottom, Mouse.LeftButton == MouseButtonState.Pressed));
        }

        public event EventHandler<ContentChangedEventArgs> ContentChanged;

        public event EventHandler<DependencyPropertyChangedEventArgs> DesignModeChanged;

        public event EventHandler<DependencyPropertyChangedEventArgs> SelectedElementChanged;

        public event EventHandler SelectedElementsChanged;

        public event EventHandler<DependencyPropertyChangedEventArgs> SelectedPanelChanged;

        protected virtual void OnAddService(object obj)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnArrange(ArrangeAction arrangeAction)
        {
            if (!DesignMode)
                throw new DesignModeException();

            var firstElement = SelectedElements.FirstOrDefault();
            if (firstElement == null)
                return;

            var left = Canvas.GetLeft(firstElement);
            var right = Canvas.GetRight(firstElement);
            var top = Canvas.GetTop(firstElement);
            var bottom = Canvas.GetBottom(firstElement);
            var width = firstElement.Width;
            var height = firstElement.Height;

            foreach (var element in SelectedElements)
            {
                if (element == null)
                    continue;

                switch (arrangeAction)
                {
                    case ArrangeAction.AlignLefts:
                        Canvas.SetLeft(element, left);
                        break;
                    case ArrangeAction.AlignCenters:
                        // TODO
                        break;
                    case ArrangeAction.AlignRights:
                        // TODO
                        Canvas.SetRight(element, right);
                        break;
                    case ArrangeAction.AlignTops:
                        Canvas.SetTop(element, top);
                        break;
                    case ArrangeAction.AlignMiddles:
                        // TODO
                        break;
                    case ArrangeAction.AlignBottoms:
                        // TODO
                        Canvas.SetBottom(element, bottom);
                        break;
                    case ArrangeAction.MakeSameWidth:
                        element.Width = width;
                        break;
                    case ArrangeAction.MakeSameHeight:
                        element.Height = height;
                        break;
                    case ArrangeAction.MakeSameSize:
                        element.Width = width;
                        element.Height = height;
                        break;
                }
            }
        }

        protected virtual void OnBringToBack()
        {
            if (!DesignMode)
                throw new DesignModeException();

            if (SelectedPanel == null)
                throw new NullReferenceException("SelectedPanel is null.");
            
            int minZIndex = int.MaxValue;
            foreach (FrameworkElement child in SelectedPanel.Children)
            {
                var zIndex = Panel.GetZIndex(child);
                if (zIndex < minZIndex)
                    minZIndex = zIndex;
            }

            if (minZIndex == 0)
            {
                foreach (FrameworkElement child in SelectedPanel.Children)
                {
                    var zIndex = Panel.GetZIndex(child);
                    Panel.SetZIndex(child, zIndex + 1);
                }
            }

            foreach (var element in SelectedElements)
            {
                if (element == null)
                    continue;

                Panel.SetZIndex(element, 0);
            }
        }

        protected virtual void OnBringToFront()
        {
            if (!DesignMode)
                throw new DesignModeException();

            if (SelectedPanel == null)
                throw new NullReferenceException("SelectedPanel is null.");

            int maxZIndex = 0;
            foreach (FrameworkElement child in SelectedPanel.Children)
            {
                var zIndex = Panel.GetZIndex(child);
                if (zIndex > maxZIndex)
                    maxZIndex = zIndex;
            }

            foreach (var element in SelectedElements)
            {
                if (element == null)
                    continue;

                Panel.SetZIndex(element, maxZIndex + 1);
            }
        }

        protected virtual void OnChangeVisibility(Visibility visibility)
        {
            if (!DesignMode)
                throw new DesignModeException();

            foreach (var element in SelectedElements)
            {
                if (element == null)
                    continue;

                element.Visibility = visibility;
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            var element = newContent as FrameworkElement;
            if (element != null)
            {
                if (element.TryGetValue("Background", null) == null)
                    element.TrySetValue("Background", Brushes.Transparent);
            }
            
            SelectedElements.Clear();
            SelectedElement = null;
            SelectedPanel = element as Panel;

            if (ContentChanged != null)
                ContentChanged(this, new ContentChangedEventArgs(oldContent, newContent));
        }

        protected virtual void OnDelete()
        {
            if (!DesignMode)
                throw new DesignModeException();

            for (int i = SelectedElements.Count; i > 0; i--)
            {
                var element = SelectedElements[i - 1];

                var parent = VisualTreeHelper.GetParent(element) as Panel;
                if (parent != null)
                    parent.Children.Remove(element);
            }
        }

        protected virtual void OnDesignModeChanged(DependencyPropertyChangedEventArgs e)
        {            
            if (DesignMode)
            {
                AllowDrop = true;
                if (SelectedPanel == null)
                    SelectedPanel = Content as Panel;
                foreach (var adorner in m_adorners)
                    adorner.Visibility = Visibility.Visible;
                Mouse.OverrideCursor = IsMouseOver ? Mouse.OverrideCursor = Cursors.Arrow : Mouse.OverrideCursor = null;
            }
            else
            {
                AllowDrop = false;
                foreach (var adorner in m_adorners)
                    adorner.Visibility = Visibility.Collapsed;
                Mouse.OverrideCursor = null;
            }
            
            if (DesignModeChanged != null)
                DesignModeChanged(this, e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            if (!DesignMode)
                throw new DesignModeException();

            var type = e.Data.GetData(typeof(Type)) as Type;
            if (type == null)
                return;

            var instance = Activator.CreateInstance(type);
            var element = instance as FrameworkElement;
            if (element == null)
                return;
            
            element.TrySetValue("Background", Brushes.Transparent);
            
            var panel = SelectedPanel != null ? SelectedPanel : Content as Panel;

            if (panel is Canvas)
            {
                var position = e.GetPosition(panel);
                Canvas.SetLeft(element, position.X);
                Canvas.SetTop(element, position.Y);

                element.TrySetValue("Text", type.Name);
                element.TrySetValue("Width", 75);
            }

            if (panel != null)
            {
                panel.Children.Add(element);
                SelectedElement = element;
            }
            else
            {
                Content = element;
                SelectedPanel = element as Panel;
            }
        }

        protected virtual T OnGetService<T>() where T : class
        {
            throw new NotImplementedException();
        }

        protected virtual void OnMove(MoveEventArgs e)
        {
            if (!DesignMode)
                throw new DesignModeException();

            if (e.Elements == null || e.Elements.Count == 0)
                return;

            var selectedElementRect = VisualTreeHelper.GetDescendantBounds(e.Elements.First());
            var selectedElementBounds = e.Elements.First().TransformToAncestor(this).TransformBounds(selectedElementRect);
            var selectedElementLeft = selectedElementBounds.Left + e.HorizontalChange;
            var selectedElementRight = selectedElementBounds.Right + e.HorizontalChange;
            var selectedElementTop = selectedElementBounds.Top + e.VerticalChange;
            var selectedElementBottom = selectedElementBounds.Bottom + e.VerticalChange;

            if (e.SnapToElements && SnapToElements)
            {
                var result = CalculateSnapToElements(selectedElementLeft, selectedElementTop, selectedElementRight, selectedElementBottom, 2);

                var horizontalChange = result.Left;
                if (horizontalChange == 0 || result.Right != 0 && Math.Abs(result.Right) < Math.Abs(horizontalChange))
                    horizontalChange = result.Right;
                e.HorizontalChange += horizontalChange;

                var verticalChange = result.Top;
                if (verticalChange == 0 || result.Bottom != 0 && Math.Abs(result.Bottom) < Math.Abs(verticalChange))
                    verticalChange = result.Bottom;
                e.VerticalChange += verticalChange;
            }

            foreach (var element in e.Elements)
            {
                var left = Canvas.GetLeft(element);
                var top = Canvas.GetTop(element);
                Canvas.SetLeft(element, left + e.HorizontalChange);
                Canvas.SetTop(element, top + e.VerticalChange);
            }

            if (e.SnapToElements && m_alignAdorner != null)
                m_alignAdorner.RenderLines();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (!DesignMode)
                return;

            e.Handled = true;

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (!DesignMode)
                return;

            e.Handled = true;

            Mouse.OverrideCursor = null;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!DesignMode)
                return;

            e.Handled = true;

            switch(e.Key)
            {
                case Key.Left:
                    Move(-1, 0);
                    break;
                case Key.Up:
                    Move(0, -1);
                    break;
                case Key.Right:
                    Move(1, 0);
                    break;
                case Key.Down:
                    Move(0, 1);
                    break;
                case Key.Delete:
                    Delete();
                    break;
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (!DesignMode)
                return;

            e.Handled = true;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!DesignMode)
                return;

            e.Handled = true;

            var element = GetHitElement(e.GetPosition(this));
            if (element == null || !element.IsDescendantOf(SelectedPanel))
                return;

            SelectedElement = element;
            SelectedElement.Focus();

            if (SelectedElement != null)
            {
                m_isDragging = false;
                m_mouseDownOffsetPoint = e.GetPosition(SelectedElement);
                m_mouseDownStartPoint = e.GetPosition(SelectedPanel);
            }

            if (m_alignAdorner == null)
            {
                m_alignAdorner = new AlignAdorner(this);
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                adornerLayer.Add(m_alignAdorner);
            }
            m_alignAdorner.RenderLines();
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!DesignMode)
                return;

            e.Handled = true;

            if (m_alignAdorner == null)
                return;
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            adornerLayer.Remove(m_alignAdorner);
            m_alignAdorner = null;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (!DesignMode)
                return;

            e.Handled = true;
            
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var newPosition = Mouse.GetPosition(SelectedPanel);
            if (!m_isDragging)
            {
                if (Math.Abs(newPosition.X - m_mouseDownStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(newPosition.Y - m_mouseDownStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                    m_isDragging = true;
            }

            if (!m_isDragging)
                return;

            if (SelectedElement == SelectedPanel)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                adornerLayer.Add(new RubberbandAdorner(this, SelectedPanel, m_mouseDownStartPoint));
            }
            else
            {
                var horizontalChange = -(m_mouseDownStartPoint.X - newPosition.X);
                var verticalChange = -(m_mouseDownStartPoint.Y - newPosition.Y);
                Move(horizontalChange, verticalChange);
            }

            m_mouseDownStartPoint = newPosition;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (!DesignMode)
                return;

            e.Handled = true;
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (!DesignMode)
                return;

            e.Handled = true;
        }

        protected virtual void OnResize(ResizeEventArgs e)
        {
            if (!DesignMode)
                throw new DesignModeException();

            if (e.Elements == null || e.Elements.Count == 0)
                return;

            if (e.SnapToElements && SnapToElements)
            {
                var elementWidth = e.Elements.First().ActualWidth;
                var elementHeight = e.Elements.First().ActualHeight;

                var elementLeft = Math.Abs(Canvas.GetLeft(e.Elements.First()) + e.Left);
                var elementRight = Math.Abs(elementLeft + elementWidth - e.Left + e.Right);
                var elementTop = Math.Abs(Canvas.GetTop(e.Elements.First()) + e.Top);
                var elementBottom = Math.Abs(elementTop + elementHeight - e.Top + e.Bottom);

                var result = CalculateSnapToElements(elementLeft, elementTop, elementRight, elementBottom, 3);

                if (e.Left != 0)
                    e.Left += result.Left;
                if (e.Top != 0)
                    e.Top += result.Top;
                if (e.Right != 0)
                    e.Right += result.Right;
                if (e.Bottom != 0)
                    e.Bottom += result.Bottom;
            }

            foreach(var element in e.Elements)
            {
                var elementLeft = Canvas.GetLeft(element);
                var elementTop = Canvas.GetTop(element);
                var elementWidth = element.ActualWidth;
                var elementHeight = element.ActualHeight;

                if (e.Left != 0 && e.Left < elementWidth)
                {
                    Canvas.SetLeft(element, elementLeft + e.Left);
                    element.Width =  elementWidth - e.Left < 0 ? 0 : elementWidth - e.Left;
                }

                if (e.Top != 0 && e.Top < elementHeight)
                {
                    Canvas.SetTop(element, elementTop + e.Top);
                    element.Height = elementHeight - e.Top < 0 ? 0 : elementHeight - e.Top;
                }

                if (e.Right != 0)
                {
                    if (elementWidth + e.Right < 0)
                        e.Right = -elementWidth;
                    element.Width = elementWidth + e.Right;
                }

                if (e.Bottom != 0)
                {
                    if (elementHeight + e.Bottom < 0)
                        e.Bottom = -elementHeight;
                    element.Height = elementHeight + e.Bottom;
                }
            }

            if (e.SnapToElements)
            {
                if (m_alignAdorner == null)
                {
                    m_alignAdorner = new AlignAdorner(this);
                    var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    adornerLayer.Add(m_alignAdorner);
                }
                m_alignAdorner.RenderLines();
            }
        }

        protected virtual void OnSelectedElementChanged(DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (m_ignoreOnSelectedElementChanged)
                    return;
                m_ignoreOnSelectedElementChanged = true;

                if (e.NewValue != null)
                {
                    var element = e.NewValue as FrameworkElement;

                    if (element == null)
                    {
                        SelectedElements.Clear();
                    }
                    else
                    {
                        if (!SelectedElements.Contains(element))
                        {
                            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                                SelectedElements.Clear();

                            if (SelectedElement == SelectedPanel)
                                SelectedElements.Clear();

                            SelectedElements.Add(element);
                        }

                        var index = SelectedElements.IndexOf(element);
                        if (index > 0)
                            SelectedElements.Move(index, 0);

                        if (element != SelectedPanel)
                            SelectedPanel = element.Parent as Panel;
                    }
                }
            }
            finally
            {
                m_ignoreOnSelectedElementChanged = false;
            }

            if (SelectedElementChanged != null)
                SelectedElementChanged(this, e);
        }

        protected virtual void OnSelectedElementsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!DesignMode)
                throw new DesignModeException();

            if (e.OldValue is FrameworkElementObservableCollection)
            {
                var selectedElements = (FrameworkElementObservableCollection)e.OldValue;
                selectedElements.CollectionChanged -= new NotifyCollectionChangedEventHandler(SelectedElements_CollectionChanged);
            }

            if (e.NewValue is FrameworkElementObservableCollection)
            {
                var selectedElements = (FrameworkElementObservableCollection)e.NewValue;
                selectedElements.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedElements_CollectionChanged);
            }

            if (SelectedElementsChanged != null)
                SelectedElementsChanged(this, new EventArgs());
        }

        protected virtual void OnSelectedPanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!DesignMode)
                return;

            SelectedElements.Clear();

            if (SelectedPanelChanged != null)
                SelectedPanelChanged(this, e);
        }
        
        private CalculateSnapToElementsResult CalculateSnapToElements(double left, double top, double right, double bottom, double snapToElementsWithin)
        {
            var result = new CalculateSnapToElementsResult();
            foreach (FrameworkElement element in this.GetDescendants())
            {
                if (element == SelectedElement)
                    continue;

                var elementRect = VisualTreeHelper.GetDescendantBounds(element);
                var elementBounds = element.TransformToAncestor(this).TransformBounds(elementRect);
                var elementLeft = Math.Abs(elementBounds.Left);
                var elementRight = Math.Abs(elementBounds.Right);
                var elementTop = Math.Abs(elementBounds.Top);
                var elementBottom = Math.Abs(elementBounds.Bottom);

                // Left
                if (elementLeft > left - snapToElementsWithin && elementLeft < left + snapToElementsWithin)
                {
                    result.Left = elementLeft - left;
                }
                else if (elementRight > left - snapToElementsWithin && elementRight < left + snapToElementsWithin)
                {
                    result.Left = elementRight - left;
                }

                // Right
                if (elementLeft > right - snapToElementsWithin && elementLeft < right + snapToElementsWithin)
                {
                    result.Right = elementLeft - right;
                }
                else if (elementRight > right - snapToElementsWithin && elementRight < right + snapToElementsWithin)
                {
                    result.Right = elementRight - right;
                }

                // Top
                if (elementTop > top - snapToElementsWithin && elementTop < top + snapToElementsWithin)
                {
                    result.Top = elementTop - top;
                }
                else if (elementBottom > top - snapToElementsWithin && elementBottom < top + snapToElementsWithin)
                {
                    result.Top = elementBottom - top;
                }

                // Bottom
                if (elementTop > bottom - snapToElementsWithin && elementTop < bottom + snapToElementsWithin)
                {
                    result.Bottom = elementTop - bottom;
                }
                else if (elementBottom > bottom - snapToElementsWithin && elementBottom < bottom + snapToElementsWithin)
                {
                    result.Bottom = elementBottom - bottom;
                }
            }

            return result;
        }

        private void CommandCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == PanelDesigner.SelectPanelCommand)
            {
                if (DesignMode && SelectedElement is Panel && SelectedElement != SelectedPanel)
                    e.CanExecute = true;
            }
        }

        private void CommandExecuteHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == PanelDesigner.SelectPanelCommand)
                SelectedPanel = SelectedElement as Panel;
        }

        private FrameworkElement GetHitElement(Point position)
        {
            var result = VisualTreeHelper.HitTest(this, position);
            if (result == null)
                return null;

            var element = result.VisualHit as DependencyObject;
            if (element == null)
                return null;

            if (element == SelectedPanel)
                return SelectedPanel;

            do
            {
                var parent = VisualTreeHelper.GetParent(element);
                if (parent == SelectedPanel)
                    break;
                element = parent;
            }
            while (element != null);

            return element as FrameworkElement;
        }

        private void ResizeAdorner_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (m_alignAdorner == null)
            {
                m_alignAdorner = new AlignAdorner(this);
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                adornerLayer.Add(m_alignAdorner);
            }
            m_alignAdorner.RenderLines();
        }

        private void ResizeAdorner_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnPreviewMouseLeftButtonUp(e);
        }

        private void SelectedElements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
                return;

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var element = (FrameworkElement)item;
                    var resizeAdorner = element.GetAdorner<ResizeAdorner>();
                    if (resizeAdorner != null)
                    {
                        resizeAdorner.PreviewMouseLeftButtonUp -= ResizeAdorner_PreviewMouseLeftButtonUp;
                        resizeAdorner.PreviewMouseLeftButtonDown -= ResizeAdorner_PreviewMouseLeftButtonDown;
                        m_adorners.Remove(resizeAdorner);
                        element.RemoveAdorner(resizeAdorner);
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item == null)
                        continue;

                    var element = (FrameworkElement)item;
                    if (element != SelectedPanel)
                    {
                        var resizeAdorner = element.GetAdorner<ResizeAdorner>();
                        if (resizeAdorner == null)
                        {
                            resizeAdorner = new ResizeAdorner(this, element);
                            resizeAdorner.PreviewMouseLeftButtonUp += ResizeAdorner_PreviewMouseLeftButtonUp;
                            resizeAdorner.PreviewMouseLeftButtonDown += ResizeAdorner_PreviewMouseLeftButtonDown;
                            resizeAdorner.Visibility = DesignMode ? Visibility.Visible : Visibility.Collapsed;
                            m_adorners.Add(resizeAdorner);
                            element.AddAdorner(resizeAdorner);
                        }
                    }
                }
            }

            CommandManager.InvalidateRequerySuggested();
        }
    }
}