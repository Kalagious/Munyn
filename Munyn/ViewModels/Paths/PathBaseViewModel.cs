using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia; // For Point, Rect
using System;
using System.Globalization; // For InvariantCulture

namespace Munyn.ViewModels
{
    public partial class PathBaseViewModel : NodeBaseViewModel
    {
        private NodeBaseViewModel _startNode;
        private NodeBaseViewModel _endNode;

        // PathData property for the curved line
        [ObservableProperty]
        private string _pathData = "M 0,0 C 0,0 0,0 0,0"; // Default empty path

        // Properties for XAML binding to draw the line (now primarily for calculating PathData)
        public double X1 => StartNode?.X ?? 0;
        public double Y1 => StartNode?.Y ?? 0;
        public double X2 => EndNode?.X ?? 0;
        public double Y2 => EndNode?.Y ?? 0;


        public NodeBaseViewModel StartNode
        {
            get => _startNode;
            set
            {
                if (SetProperty(ref _startNode, value))
                {
                    if (_startNode != null)
                        _startNode.PropertyChanged += OnNodePropertyChanged;
                    RecalculatePathData(); // Recalculate when node changes
                }
            }
        }

        public NodeBaseViewModel EndNode
        {
            get => _endNode;
            set
            {
                if (SetProperty(ref _endNode, value))
                {
                    if (_endNode != null)
                        _endNode.PropertyChanged += OnNodePropertyChanged;
                    RecalculatePathData(); // Recalculate when node changes
                }
            }
        }



        public PathBaseViewModel(NodeBaseViewModel startNode, NodeBaseViewModel endNode)
        {
            if (startNode == null || endNode == null)
            {
                throw new ArgumentNullException("Start and End nodes cannot be null for a connection.");
            }
            _startNode = startNode;
            _endNode = endNode;

            _startNode.PropertyChanged += OnNodePropertyChanged;
            _endNode.PropertyChanged += OnNodePropertyChanged;

            RecalculatePathData(); // Initial calculation
        }


        private void OnNodePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NodeBaseViewModel.X) || e.PropertyName == nameof(NodeBaseViewModel.Y))
            {
                RecalculatePathData(); // Recalculate when node position changes
            }
        }

        // --- NEW: Method to calculate the curved path data ---
        public void RecalculatePathData()
        {
            if (StartNode == null || EndNode == null)
            {
                PathData = "M 0,0"; // Empty path if nodes are null
                return;
            }

            // Node coordinates (adjusting to approximate center or connection point of node view)
            // Assuming node views are around 140x90 based on HostNodeView.axaml
            double x1 = StartNode.X + 70; // Center X of start node
            double y1 = StartNode.Y + 45; // Center Y of start node
            double x2 = EndNode.X + 70;   // Center X of end node
            double y2 = EndNode.Y + 45;   // Center Y of end node

            double dx = x2 - x1;
            double dy = y2 - y1;

            double normalLength = Math.Sqrt(dx * dx + dy * dy);

            // Handle division by zero / very short lines
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
            if (x1 < x2) // Moving generally right: first bend down, second bend up
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