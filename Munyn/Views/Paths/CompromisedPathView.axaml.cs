using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Munyn.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Munyn.Views.Paths
{
    public partial class TriangleViewModel : ObservableObject
    {
        [ObservableProperty]
        private double _x;

        [ObservableProperty]
        private double _y;

        [ObservableProperty]
        private double _angle;
    }

    public partial class CompromisedPathView : UserControl
    {
        private DispatcherTimer _timer;
        private PathGeometry _pathGeometry;
        private double _pathLength;
        private const double TriangleSpacing = 40.0;
        private PathBaseViewModel _viewModel;

        public ObservableCollection<TriangleViewModel> Triangles { get; } = new ObservableCollection<TriangleViewModel>();

        public CompromisedPathView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Render, OnTimerTick);
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            _viewModel = DataContext as PathBaseViewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                UpdatePathData(_viewModel.PathData);
            }
            else
            {
                StopAnimation();
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PathBaseViewModel.PathData))
            {
                UpdatePathData(_viewModel.PathData);
            }
        }

        private void UpdatePathData(string pathData)
        {
            if (string.IsNullOrWhiteSpace(pathData) || pathData == "M 0,0")
            {
                StopAnimation();
                return;
            }

            try
            {
                _pathGeometry = PathGeometry.Parse(pathData);
                _pathLength = _pathGeometry.GetOrComputeLength();
            }
            catch (FormatException)
            {
                StopAnimation();
                return;
            }

            if (_pathLength > 0)
            {
                PopulateTriangles();
                StartAnimation();
            }
            else
            {
                StopAnimation();
            }
        }

        private void PopulateTriangles()
        {
            Triangles.Clear();
            if (_pathLength <= 0) return;

            var triangleCount = (int)(_pathLength / TriangleSpacing);
            for (int i = 0; i < triangleCount; i++)
            {
                Triangles.Add(new TriangleViewModel());
            }
        }

        private void StartAnimation()
        {
            _timer.Start();
        }

        private void StopAnimation()
        {
            _timer.Stop();
            Triangles.Clear();
        }

        private double _animationOffset = 0;

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_pathLength == 0) return;

            _animationOffset += 0.5; // Speed of the animation
            if (_animationOffset > TriangleSpacing)
            {
                _animationOffset = 0;
            }

            for (int i = 0; i < Triangles.Count; i++)
            {
                var distance = (i * TriangleSpacing + _animationOffset) % _pathLength;
                if (_pathGeometry.TryGetPointAtLength(distance, out var point, out var tangent))
                {
                    var angle = Math.Atan2(tangent.Y, tangent.X) * (180 / Math.PI) + 90;
                    var triangle = Triangles[i];
                    triangle.Angle = angle;
                    triangle.X = point.X;
                    triangle.Y = point.Y;
                }
            }
        }

        protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            StopAnimation();
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }
    }
}
