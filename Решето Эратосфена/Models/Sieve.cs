using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Решето_Эратосфена.Models
{
    /// <summary>
    /// Модель решета Эратосфена
    /// </summary>
    public class Sieve: IAnimation
    {
        List<Circle> _circles = new List<Circle>();
        // буфер для хранения чисел, которые необходимо добавить в список после прохождения крайнего слева круга, true - простое
        List<bool> _buffer = new List<bool>() { false, false }; // 0, 1
        List<DivisorColumn> _columns = new List<DivisorColumn>();
        int _bufferIndex = 2; // число, которое находится в ожидании прохождения крайнего мимо него
        public Sieve(double speed)
        {
            Circle.TranslationSpeedX = speed;
            _circles.Add(new One(this));
        }
        public event Action NewNumberCreated;
        public void AddNumber(int n)
        {
            _buffer.Add(true);
            _columns.Add(new DivisorColumn(n));
            NewNumberCreated?.Invoke();
        }
        /// <summary>
        /// Отметить число как составное
        /// </summary>
        /// <param name="n"></param>
        public void SetOddNumber(int n, int divisor)
        {
            //Console.WriteLine($"Number {n} set as odd, divisor = {divisor}");
            _buffer[n] = false;
            _columns[n-2].AddDivisor(divisor);
        }
        public void NextStage()
        {
            foreach (var circle in _circles)
            {
                circle.NextStage();
            }
            if (_buffer.Count < 3) return;
            // если через число прошли все круги
            if (_bufferIndex < _circles.Last().X)
            {
                // если на число ни разу не приходился полный поворот круга
                if (_buffer[_bufferIndex])
                {
                    _circles.Add(new Circle(this, _bufferIndex));
                }
                // перейти к следующему числу
                _bufferIndex++;
            }
            //SpeedUp();
        }
        public double SpeedK = 2; // коэффициент изменения скорости развития модели
        public void SpeedUp()
        {
            Circle.TranslationSpeedX *= SpeedK;
            ApplySpeedChanges();
        }
        public void SpeedDown()
        {
            Circle.TranslationSpeedX /= SpeedK;
            ApplySpeedChanges();
        }
        void ApplySpeedChanges()
        {
            foreach(var circle in _circles)
            {
                circle.UpdateTranslationSpeed();
            }
        }
        public void Draw(Graphics g, DrawParams p)
        {
            g.ScaleTransform(p.SizeK, p.SizeK, MatrixOrder.Prepend);
            g.TranslateTransform(p.ZeroXY.X, p.ZeroXY.Y, MatrixOrder.Append);
            p.Update();
            foreach (var column in _columns)
            {
                column.Draw(g, p);
            }
            foreach (var circle in _circles)
            {
                circle.Draw(g, p);
            }
            g.ResetTransform();
        }
    }
}
