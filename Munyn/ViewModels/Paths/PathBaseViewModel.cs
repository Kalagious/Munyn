using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia; // For Point, Rect
using System;
using System.Globalization; // For InvariantCulture

namespace Munyn.ViewModels
{
    public partial class PathBaseViewModel : ViewModelBase
    {
        public Point StartPoint;
        public Point EndPoint;
        public NodeBaseViewModel StartNode;
        public NodeBaseViewModel EndNode;
        public MainViewModel _mainVm;
        // PathData property for the curved line
        [ObservableProperty]
        private string _pathData = "M 0,0 C 0,0 0,0 0,0"; // Default empty path

        // Properties for XAML binding to draw the line (now primarily for calculating PathData)




        public PathBaseViewModel(NodeBaseViewModel startNode, Point endPoint, MainViewModel mainViewModel)
        {
            _mainVm = mainViewModel;
            if (startNode == null || endPoint == null)
            {   
                throw new ArgumentNullException("Start and End nodes cannot be null for a connection.");
            }
            StartNode = startNode;
            StartPoint = new Point(StartNode.X, StartNode.Y);
            EndPoint = endPoint;



            RecalculatePathData(); // Initial calculation
        }




        // --- NEW: Method to calculate the curved path data ---
        public void RecalculatePathData()
        {
            if (_mainVm == null || StartNode == null) return;

            var startNodeView = _mainVm.FindNodeViewByViewModel(StartNode);
            if (startNodeView == null) return;

            StartPoint = new Point(StartNode.X + startNodeView.Bounds.Width / 2, StartNode.Y+7);

            if (EndNode != null)
            {
                var endNodeView = _mainVm.FindNodeViewByViewModel(EndNode);
                if (endNodeView == null) return;
                EndPoint = new Point(EndNode.X + endNodeView.Bounds.Width / 2, EndNode.Y-7);
            }

            if (StartPoint == null || EndPoint == null)
            {
                PathData = "M 0,0";
                return;
            }


            bool startNodeTop = false;

            double y1 = StartPoint.Y;
            double y2 = EndPoint.Y;
            double x1 = StartPoint.X;
            double x2 = EndPoint.X;   // Center X of end node

            
            if (EndNode != null)
            {
                y2 += _mainVm.FindNodeViewByViewModel(EndNode).Bounds.Height;
                startNodeTop = true;
            }
            


            double dx = x2 - x1;
            double dy = y2 - y1;

            double normalLength = Math.Sqrt(dx * dx + dy * dy);

            if (normalLength < 1.0)
            {
                PathData = string.Format(
                    CultureInfo.InvariantCulture,
                    "M {0},{1} L {2},{3}", // Draw a tiny straight line
                    x1, y1, x2, y2
                );
                return; // Exit the method
            }

            // --- NEW: Calculate Control Points for S-Curve with Vertical Ends ---
            double cp1x, cp1y, cp2x, cp2y;

            // Enforce vertical tangency at both ends:
            // Control points' X-coordinates are the same as their respective start/end nodes.
            cp1x = x1;
            cp2x = x2;

            // Determine the Y-coordinates for the S-bend.
            // This offset creates the "S" shape by pulling the curve inwards or outwards vertically.
            // The magnitude of the S-bend can be influenced by the horizontal distance (dx)
            // or a fixed value. We'll use a minimum fixed value or a proportion of dx.
            double sBendOffset = Math.Max(70.0, Math.Abs(dx) * 0.4); // Adjust 70.0 for min bend, 0.4 for dx proportion

            // Apply the offset to the Y-coordinates to create the S-curve.
            // The direction of the bend depends on whether x1 is less than x2 (moving right) or greater (moving left).
            if (!startNodeTop) // Moving generally right: first bend down, second bend up
            {
                cp1y = y1 + sBendOffset;
                cp2y = y2 - sBendOffset;
            }
            else // Moving generally left: first bend up, second bend down
            {
                cp1y = y1 - sBendOffset;
                cp2y = y2 + sBendOffset;
            }

            // --- END NEW S-Curve Calculation ---

            // Format the path data string
            PathData = string.Format(
                CultureInfo.InvariantCulture,
                "M {0},{1} C {2},{3} {4},{5} {6},{7}",
                x1, y1, cp1x, cp1y, cp2x, cp2y, x2, y2
            );
        }

        // IMPORTANT: Implement IDisposable if you create a lot of connections dynamically
        // and need to clean up event subscriptions to prevent memory leaks.
    }
}