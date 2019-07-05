using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Решето_Эратосфена
{
    public static class Ext
    {
        public static Color NextColor(this Random rand, int alpha = 255)
        {
            return Color.FromArgb(alpha, rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
        }
    }
}
