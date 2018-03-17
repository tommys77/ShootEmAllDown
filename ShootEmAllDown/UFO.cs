using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmAllDown
{
    public class UFO : Enemy
    {
        public UFO(int lvl, string origin) : base(lvl, origin)
        {
            Energy = Level * 2.0;
            BattleCry = "Zlorfgof! I really miss " + Origin + "...";
        }

        public override double Energy { get => base.Energy; set => base.Energy = value; }


    }
}
