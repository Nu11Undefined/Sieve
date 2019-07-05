using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

namespace Решето_Эратосфена
{
    /// <summary>
    /// Отображение
    /// </summary>
    public interface IDrawn
    {
        void Draw(Graphics g, DrawParams p);
    }
    /// <summary>
    /// Развитие
    /// </summary>
    public interface IEvolution
    {
        void NextStage();
    }
    /// <summary>
    /// Отображение развивающихся систем
    /// </summary>
    public interface IAnimation : IDrawn, IEvolution { }

    /// <summary>
    /// Параметры отображения
    /// </summary>
    public class DrawParams
    {
        static Random _rand = new Random(1);
        /// <summary>
        /// Точка отображения начала координат
        /// </summary>
        public PointF ZeroXY { get; set; }
        /// <summary>
        /// Масштаб отображения
        /// </summary>
        public float SizeK { get; set; } = 100;
        public float ZoomK = 1.5F;
        public float PenWidth = 2F;
        public RectangleF Rect { get; set; }
        public List<Brush> Brush = new List<Brush>() { Brushes.Black, Brushes.White };
        public DrawParams(int w, int h)
        {
            ZeroXY = new PointF(0, h * 4/5);
            Rect = new RectangleF(0, 0, w, h);
        }
        public Pen DivPen;
        public Font DivFont;
        public void Update()
        {
            DivPen = new Pen(Color.Gray, 1 / SizeK);
            DivFont = new Font("Consolas", 0.2F, GraphicsUnit.Pixel);
        }
        public void AddColor()
        {
            Brush.Add(new SolidBrush(_rand.NextColor()));
        }
        public void Shift(float dx, float dy)
        {
            ZeroXY = new PointF(ZeroXY.X + dx, ZeroXY.Y + dy);
        }
        void MakeZoom(float x, float y, float k)
        {
            float dx2 = k *(x - ZeroXY.X), dy2 = k * (y - ZeroXY.Y);
            ZeroXY = new PointF(x - dx2, y - dy2);
            SizeK *= k;
        }
        public void ZoomIn(float x, float y)
        {
            MakeZoom(x, y, ZoomK);
        }
        public void ZoomOut(float x, float y)
        {
            MakeZoom(x, y, 1 / ZoomK);
        }
    }
}
