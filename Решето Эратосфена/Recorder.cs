using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Accord.Video.FFMPEG;
using System.IO;

namespace Решето_Эратосфена
{
    public static class Recorder
    {
        /// <summary>
        /// Записать развитие модели на видео
        /// </summary>
        /// <param name="model">Модель развития</param>
        /// <param name="param">Параметры отображения</param>
        /// <param name="fps">Количество кадров в секунду</param>
        /// <param name="spf">Количество этапов развития за кадр</param>
        /// <param name="frameCount">Количество кадров</param>
        /// <param name="speedK">Ускорение развития системы за одну секунду</param>
        public static void Record(this IAnimation model, DrawParams param, int fps, int spf, int frameCount, float speedK)
        {
            var writer = new VideoFileWriter();
            writer.Open(
                $"out{Directory.GetFiles(Environment.CurrentDirectory).Count(x => x.Contains("out"))}.mp4", 
                (int)param.Rect.Width, (int)param.Rect.Height, fps, VideoCodec.MPEG4, (int)(10 * param.Rect.Width * param.Rect.Height));
            ProgressWindow pr = new ProgressWindow(frameCount);
            float deltaShift = -0.3F;
            double zoomK = speedK;
            float zoomKPerFrame = (float)Math.Pow(zoomK, zoomK / fps);
            param.ZoomK = zoomKPerFrame;
            using (var bmp = new Bitmap(writer.Width, writer.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    for (int i = 0; i < frameCount; i++)
                    {
                        g.Clear(Color.Black);
                        model.Draw(g, param);
                        for (int j = 0; j < spf; j++) model.NextStage();
                        //param.Shift(deltaShift, 0);
                        //deltaShift *= speedK;
                        param.ZoomOut(0, param.Rect.Height *4/ 5);
                        writer.WriteVideoFrame(bmp);
                        pr.Next();
                    }
                }
            }
            pr.Dispose();
            writer.Close();
        }
    }
    public class ProgressWindow: Form
    {
        int _value = 0;
        int _maxValue;

        BufferedGraphicsContext _ctx;
        BufferedGraphics _buf;
        Graphics _g;
        public ProgressWindow(int maxValue)
        {
            FormBorderStyle = FormBorderStyle.None;
            SetBounds(100, 100, 300, 100);
            _g = CreateGraphics();
            _ctx = BufferedGraphicsManager.Current;
            _g = CreateGraphics();
            _buf = _ctx.Allocate(_g, ClientRectangle);
            _buf.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _buf.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            _maxValue = maxValue;
            _font = new Font("Segoe UI", Height / 3);
            _format = new StringFormat();
            _format.Alignment = StringAlignment.Center;
            _format.LineAlignment = StringAlignment.Center;
            Show();
        }
        Font _font;
        StringFormat _format;
        Brush _brush = new SolidBrush(Color.Green);
        public void Next()
        {
            _value++;
            _buf.Graphics.Clear(Color.White);
            _buf.Graphics.FillRectangle(_brush, 0, 0, Width * _value / _maxValue, Height);
            _buf.Graphics.DrawString($"{_value}/{_maxValue}", _font, Brushes.Black, ClientRectangle, _format);
            _buf.Render();
        }
        public new void Dispose()
        {
            Close();
            Dispose(true);
        }
    }
}
