using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Решето_Эратосфена.Models
{
    /// <summary>
    /// Модель числа как окружности, катящейся по числовой прямой
    /// </summary>
    public class Circle: IAnimation
    {
        /// <summary>
        /// Текущая координата X центра окружности
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Значение числа - меняется в процессе "роста", определяет количество штрихов
        /// </summary>
        public int GrowthValue { get; set; }
        /// <summary>
        /// Разница в угловых координатах штрихов
        /// </summary>
        public double DashDeltaAngle { get; set; }
        /// <summary>
        /// Значение числа - состояние в конце роста
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Находится ли число в процессе "роста"
        /// </summary>
        public bool IsRising { get; set; } = true;
        /// <summary>
        /// Рост, выраженный в угле
        /// </summary>
        protected double RisingAngle { get; set; }
        /// <summary>
        /// Скорость роста как угловая скорость
        /// </summary>
        protected double RiseSpeed { get; set; }
        /// <summary>
        /// Скорость перемещения вдоль оси X
        /// </summary>
        public static double TranslationSpeedX { get; set; }
        /// <summary>
        /// Отношение длины штриха к радиусу
        /// </summary>
        public static double HatchLengthK { get; set; } = 0.3;
        /// <summary>
        /// Длина штриха
        /// </summary>
        public double HatchLength { get; set; }
        /// <summary>
        /// Радиус
        /// </summary>
        public double R { get; set; }
        /// <summary>
        /// Угловая скорость
        /// </summary>
        public double AngularVelocity { get; set; }
        /// <summary>
        /// Угол поворота
        /// </summary>
        public double Rotation { get; set; }
        protected Sieve Parent; // контейнер
        public Circle(Sieve parent, int number)
        {
            Parent = parent;
            Number = number;
            X = number;
            R = number / Math.PI / 2;
            HatchLength = R * HatchLengthK;
            UpdateTranslationSpeed();
            DashDeltaAngle = LimitRotation / Number;
        }
        public void UpdateTranslationSpeed()
        {
            RiseSpeed = 4*TranslationSpeedX / Number * LimitRotation;
            AngularVelocity = TranslationSpeedX / R;
        }
        protected const double StartAngle = Math.PI / 2; // Начальный угол поворота
        protected const double LimitRotation = 2 * Math.PI + 0.00001; // полный оборот
        public virtual void NextStage()
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
                GrowthValue = (int)Math.Floor(RisingAngle/DashDeltaAngle);
            }
            else
            {
                Rotation += AngularVelocity;
                X += TranslationSpeedX;
                // если совершен очередной оборот, отметить число как составное
                if (Rotation > LimitRotation)
                {
                    Rotation -= LimitRotation;
                    Parent.SetOddNumber((int)Math.Round(X), Number);
                    //Console.WriteLine($"X = {X}");
                }
            }
        }
        public static float ToDegree(double rad)
        {
            return (float)(rad * 180.0 / Math.PI);
        }
        static StringFormat _format = new StringFormat()
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center,
        };
        public void Draw(Graphics g, DrawParams p)
        {
            var fRot = ToDegree(Rotation);
            g.TranslateTransform((float)(X), (float)(-R) - p.PenWidth/p.SizeK, MatrixOrder.Prepend);
            var circleRect = new RectangleF((float)-R, (float)-R, (float)(2 * R), (float)(2 * R));
            // отрисовка круга
            using (var pen = new Pen(Color.Green, p.PenWidth / p.SizeK))
            {
                g.DrawArc(pen, circleRect, ToDegree(StartAngle), ToDegree(RisingAngle));
            }
            double a0 = StartAngle + Rotation, x, y;
            // отрисовка основного штриха
            using (var pen = new Pen(Color.Red, p.PenWidth / p.SizeK))
            {
                x = R * Math.Cos(a0);
                y = R * Math.Sin(a0);
                g.DrawLine(pen, (float)x, (float)y, (float)(x * (1 - HatchLength/R)), (float)(y*(1 - HatchLength/R)));
            }
            using (var pen = new Pen(Color.Green, p.PenWidth / p.SizeK))
            {
                int count = GrowthValue == Number ? Number - 1 : GrowthValue;
                // Отрисовка доп штрихов
                for (int i = 0; i < count; i++)
                {
                    a0 += DashDeltaAngle;
                    x = R * Math.Cos(a0);
                    y = R * Math.Sin(a0);
                    g.DrawLine(pen, (float)x, (float)y, (float)(x * (1 - HatchLength / R/2)), (float)(y * (1 - HatchLength / R/2)));
                }
            }
            // отображение числа
            using (var font = new Font("Consolas", (float)R / Number.ToString().Length, GraphicsUnit.Pixel))
            {
                g.DrawString(GrowthValue.ToString(), font, p.Brush[Number], circleRect, _format);
            }
            g.TranslateTransform((float)(-X), (float)(R) + p.PenWidth / p.SizeK, MatrixOrder.Prepend);
        }
    }
}
