using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShootEmAllDown
{
    public class UFO : Enemy
    {
        public UFO(int lvl, string origin) : base(lvl, origin)
        {
            Energy = Level * 1.5;
            BattleCry = "Zlorfgof! I really miss " + Origin + "...";

            var imgBrush = new ImageBrush();

            imgBrush.ImageSource = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Resources/ufo.png"));
            
            Rectangle.Fill = imgBrush;
            
        }

        public override double Energy { get => base.Energy; set => base.Energy = value; }


    }
}
