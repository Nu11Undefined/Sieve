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
    public class GdiRenderer: RenderWindow
    {
        BufferedGraphicsContext _ctx;
        BufferedGraphics _buf;
        Graphics _g;
        public override Control Window { get => this; }
        public GdiRenderer(Rectangle bounds): base(bounds)
        {
            _ctx = BufferedGraphicsManager.Current;
            _g = CreateGraphics();
            UpdateGraphics();
        }
        /// <summary>
        /// Обновить поверхность отображения
        /// </summary>
        protected override void UpdateGraphics()
        {
            if (_buf != null)
            {
                _buf.Dispose();
            }
            _buf = _ctx.Allocate(_g, ClientRectangle);
            _buf.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _buf.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        }
        public override void Render(IDrawn model, DrawParams param)
        {
            _buf.Graphics.Clear(Color.Black);
            model.Draw(_buf.Graphics, param);
            _buf.Render();
        }
    }
}
