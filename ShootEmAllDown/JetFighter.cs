using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShootEmAllDown
{
    public class JetFighter : Enemy
    {
        private BitmapImage leftBitmap { get; set; } = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Resources/jetfighter_left.png"));
        private BitmapImage rightBitmap { get; set; } = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Resources/jetfighter_right.png"));
        public ImageBrush ImgBrush { get; set; } = new ImageBrush();

        public JetFighter(int lvl, int speed, string origin) : base(lvl, speed, origin)
        {
            BattleCry = "For " + base.Origin + "!!";

            ImgBrush = RightBrush;

            if (MovingLeft)
            {
                ImgBrush = LeftBrush;
            }

            Rectangle.Fill = ImgBrush;
        }

        public ImageBrush LeftBrush
        {
            get
            {
                ImgBrush.ImageSource = leftBitmap;
                return ImgBrush;
            }
        }

        public ImageBrush RightBrush
        {
            get
            {
                ImgBrush.ImageSource = rightBitmap;
                return ImgBrush;
            }
        }

        public override string BattleCry { get => base.BattleCry; set => base.BattleCry = value; }
        public override string Origin { get => base.Origin; set => base.Origin = value; }
    }
}
