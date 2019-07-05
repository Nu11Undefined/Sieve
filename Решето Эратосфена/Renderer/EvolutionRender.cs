using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Решето_Эратосфена
{
    /// <summary>
    /// Отображение развивающихся систем
    /// </summary>
    public class EvolutionRender
    {
        /// <summary>
        /// Окно отображения
        /// </summary>
        RenderWindow Renderer { get; set; }
        public Control Window { get => Renderer.Window; }
        Timers.Timer _timer;
        /// <summary>
        /// Максимальное количество кадров в секунду
        /// </summary>
        public int FPS_MAX { get; set; } = 60;
        /// <summary>
        /// Минимальное количество кадров в секунду
        /// </summary>
        public int FPS_MIN { get; set; } = 1;
        /// <summary>
        /// Максимальное количество этапов развития за кадр
        /// </summary>
        public int SPF_MAX { get; set; } = int.MaxValue;
        /// <summary>
        /// Минимальное количество этапов развития за кадр
        /// </summary>
        public int SPF_MIN { get; set; } = 1;
        private int _fps = 30;
        /// <summary>
        /// Кадров в секунду
        /// </summary>
        public int FPS
        {
            get => _fps;
            set
            {
                if (value != _fps && value >= FPS_MIN && value <= FPS_MAX)
                {
                    _fps = value;
                }
            }
        }
        private int _spf = 2;
        /// <summary>
        /// Количество этапов развития, пройденных моделью за один кадр
        /// </summary>
        public int SPF
        {
            get => _spf;
            set
            {
                if (value != _spf && value >= SPF_MIN && value <= SPF_MAX)
                {
                    _spf = value;
                }
            }
        }
        public int Width => Window.Width;
        public int Height => Window.Height;
        public EvolutionRender(Rectangle bounds, int fps, RenderMode mode)
        {
            if (mode == RenderMode.GDI) Renderer = new GdiRenderer(bounds);
            FPS = fps;
        }
        Action _render;
        public void InitPlay(IAnimation model, DrawParams param)
        {
            _timer = new Timers.Timer() { Period = 1000 / FPS };
            _timer.Tick += (s, e) => Renderer.BeginInvoke(new MethodInvoker(() =>
            {
                for (int i = 0; i < SPF; i++) model.NextStage();
                Renderer.Render(model, param);
            }));
            _render = () => Renderer.Render(model, param);
            // первичное отображение
            Renderer.Shown += (s, e) =>
            {
                Render();
                _timer.Start();
            };
            Application.Run(Renderer);
        }
        public void Close()
        {
            _timer.Stop();
            Renderer.Close();
        }
        /// <summary>
        /// Принудительно отрисовать
        /// </summary>
        public void Render()
        {
            _render?.Invoke();
        }
        public void StopPlay()
        {
            if (_timer.IsRunning) _timer.Stop();
            else _timer.Start();
        }
    }
    public enum RenderMode : int
    {
        GDI,
        OpenGL,
        WPF,
        DirectX
    }
}
