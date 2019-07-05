using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Решето_Эратосфена.Models
{
    public class DivisorColumn: IDrawn
    {
        List<int> Divisors { get; set; } = new List<int>();
        public DivisorColumn(int n)
        {
            AddDivisor(n); // само число
        }
        public void AddDivisor(int n)
        {
            Divisors.Add(n);
        }
        static StringFormat _format = new StringFormat()
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center,
        };
        static float _hK = 1F / 3;
        public void Draw(Graphics g, DrawParams p)
        {
            var rect = new RectangleF(Divisors[0] - 1F / 2, 0, 1F, _hK); 
            foreach(var div in Divisors)
            {
                g.FillRectangle(p.Brush[div], rect);
                g.DrawRectangle(p.DivPen, rect.X, rect.Y, rect.Width, rect.Height);
                g.DrawString(div.ToString(), p.DivFont, Brushes.Black, rect, _format);
                rect.Y += _hK;
            }
        }
    }
}
