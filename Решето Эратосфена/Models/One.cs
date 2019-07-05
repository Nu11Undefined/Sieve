using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Решето_Эратосфена.Models
{
    /// <summary>
    /// Круг единицы
    /// </summary>
    public class One: Circle
    {
        public One(Sieve parent):base(parent, 1)
        {

        }
        public override void NextStage()
        {
            if (IsRising)
            {
                RisingAngle += RiseSpeed;
                // если закончился рост
                if (RisingAngle > LimitRotation)
                {
                    IsRising = false;
                    RisingAngle = LimitRotation;
                }
                GrowthValue = (int)Math.Floor(RisingAngle / DashDeltaAngle);
            }
            else
            {
                Rotation += AngularVelocity;
                X += TranslationSpeedX;
                // если совершен очередной оборот, создать число
                if (Rotation > LimitRotation)
                {
                    Rotation -= LimitRotation;
                    Parent.AddNumber((int)Math.Floor(X));
                    //Console.WriteLine($"X = {X}");
                }
            }
        }
    }
}
