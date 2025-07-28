using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Munyn.Views.Misc
{
    public class Wisp
    {
        public Ellipse Shape { get; }
        public Vector Position { get; set; }
        public Vector Velocity { get; set; }
        public double Opacity { get; set; }

        public Wisp(Vector position, Vector velocity)
        {
            Position = position;
            Velocity = velocity;
            Opacity = 0;

            Shape = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(Colors.White),
            };
        }
    }
}
