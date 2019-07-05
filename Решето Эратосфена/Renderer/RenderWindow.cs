using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Решето_Эратосфена
{
    /// <summary>
    /// Окно для отображения
    /// </summary>
    public abstract class RenderWindow: Form
    {
        public virtual Control Window { get; set; }
        public RenderWindow(Rectangle bounds)
        {
            FormBorderStyle = FormBorderStyle.None;
            SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }
        /// <summary>
        /// Обновить поверхность отображения
        /// </summary>
        protected virtual void UpdateGraphics()
        {
        }
        public virtual void Render(IDrawn model, DrawParams param)
        {
        }
    }
}
