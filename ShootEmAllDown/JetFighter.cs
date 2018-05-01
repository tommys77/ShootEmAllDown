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
        public JetFighter(int lvl, string origin) : base(lvl, origin)
        {
            BattleCry = "For " + base.Origin + "!!";
            var imgBrush = new ImageBrush();

            imgBrush.ImageSource = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Resources/jetfighter.png"));
            Rectangle.Fill = imgBrush;
        }

        public override string BattleCry { get => base.BattleCry ; set => base.BattleCry = value; }
        public override string Origin { get => base.Origin; set => base.Origin = value; }
    }
}
