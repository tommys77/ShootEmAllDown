using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmAllDown
{
    public abstract class Enemy : IEnemy
    {
        public Guid Id { get; }
        public virtual int Level { get; set; }
        public virtual double Energy { get; set; }
        public virtual string BattleCry { get; set; }
        public virtual string Origin { get; set; }

        protected Enemy(int lvl, string origin)
        {
            Id = Guid.NewGuid();
            Level = lvl;
            Origin = origin;
            Energy = Level * 1.5;
            BattleCry = "I will destroy you!";
        }

        public void Move()
        {
            throw new NotImplementedException();
        }

        public void Attack()
        {
            throw new NotImplementedException();
        }

        public void Defend()
        {
            throw new NotImplementedException();
        }
    }
}
