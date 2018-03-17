using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmAllDown
{
    public class JetFighter : Enemy
    {
        public JetFighter(int lvl, string origin) : base(lvl, origin)
        {
            BattleCry = "For " + base.Origin + "!!";
        }

        public override string BattleCry { get => base.BattleCry ; set => base.BattleCry = value; }
        public override string Origin { get => base.Origin; set => base.Origin = value; }
    }
}
