using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Controls.Shapes;
using Munyn.ViewModels;
using System;
using System.ComponentModel;

namespace Munyn.Views.Paths
{
    public partial class CompromisedPathView : UserControl
    {
        private DispatcherTimer _timer;
        private PathGeometry _pathGeometry;
        private double _currentDistance;
        private double _pathLength;

        public CompromisedPathView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Render, OnTimerTick);
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (DataContext is PathBaseViewModel vm)
            {
                vm.PropertyChanged += OnViewModelPropertyChanged;
                UpdatePathData(vm.PathData);
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PathBaseViewModel.PathData))
            {
                var vm = (PathBaseViewModel)sender;
                UpdatePathData(vm.PathData);
            }
        }

        private void UpdatePathData(string pathData)
        {
            if (string.IsNullOrWhiteSpace(pathData))
            {
                StopAnimation();
                return;
            }

            _pathGeometry = PathGeometry.Parse(pathData);
            _pathLength = _pathGeometry.ContourLength;
            _currentDistance = 0;

            if (_pathLength > 0)
            {
                StartAnimation();
            }
            else
            {
                StopAnimation();
            }
        }

        private void StartAnimation()
        {
            this.FindControl<Polygon>("AnimatedTriangle").IsVisible = true;
            _timer.Start();
        }

        private void StopAnimation()
        {
            _timer.Stop();
            this.FindControl<Polygon>("AnimatedTriangle").IsVisible = false;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _currentDistance += 1.5; // Speed of the animation
            if (_currentDistance > _pathLength)
            {
                _currentDistance = 0;
            }

            if (_pathGeometry.TryGetPointAndTangentAtDistance(_currentDistance, out var point, out var tangent))
            {
                var angle = Math.Atan2(tangent.Y, tangent.X) * (180 / Math.PI) + 90;
                var triangle = this.FindControl<Polygon>("AnimatedTriangle");

                var transformGroup = (TransformGroup)triangle.RenderTransform;
                var rotateTransform = (RotateTransform)transformGroup.Children[0];
                var translateTransform = (TranslateTransform)transformGroup.Children[1];

                rotateTransform.Angle = angle;
                translateTransform.X = point.X;
                translateTransform.Y = point.Y;
            }
        }

        protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            StopAnimation();
            if (DataContext is PathBaseViewModel vm)
            {
                vm.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }
    }
}
