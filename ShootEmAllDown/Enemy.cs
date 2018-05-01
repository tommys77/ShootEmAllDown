using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShootEmAllDown
{
    public abstract class Enemy
    {
        public Guid Id { get; }
        public virtual Rectangle Rectangle { get; set; }
        public virtual int Level { get; set; }
        public virtual double Energy { get; set; }
        public virtual string BattleCry { get; set; }
        public virtual string Origin { get; set; }

        //To keep track of position and direction of the enemy
        public bool MovingUp { get; set; }
        public bool MovingLeft { get; set; }

        protected Enemy(int lvl, string origin)
        {
            Id = Guid.NewGuid();
            Level = lvl;
            Origin = origin;
            Energy = Level * 1.0;
            BattleCry = "I will destroy you!";

            Rectangle = new Rectangle
            {
                Width = 40,
                Height = 35,
                Uid = Id.ToString(),
            };
            Rectangle.Stretch = Stretch.Fill;
        }
    }
}
