using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Решето_Эратосфена.Models;
using System.Drawing;

namespace Решето_Эратосфена
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<int>() { 4, 5, 6 };
            int w = 1920, h = 1080, fps = 30;
            double speedK = 1.1, speedKPerFrame = Math.Pow(speedK, 1.0 / fps);
            var model = new Sieve(0.2/fps);
            model.SpeedK = speedKPerFrame;
            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var player = new EvolutionRender(new Rectangle((screen.Width - w)/2, (screen.Height-h)/2, w, h), fps, RenderMode.GDI);
            var param = new DrawParams(player.Width, player.Height);
            var controller = new Controller(player.Window);
            controller.StopPlay += () => player.StopPlay();
            controller.ShiftByPixel += (dx, dy) =>
            {
                param.Shift(dx, dy);
                player.Render();
            };
            controller.ZoomIn += (x, y) =>
            {
                param.ZoomIn(x, y);
                player.Render();
            };
            controller.ZoomOut += (x, y) =>
            {
                param.ZoomOut(x, y);
                player.Render();
            };
            controller.Exit += () => player.Close();
            model.NewNumberCreated += () => param.AddColor();
            controller.SpeedDown += () => model.SpeedDown();
            controller.SpeedUp += () => model.SpeedUp();
            player.InitPlay(model, param);
            //Recorder.Record(model, param, fps, 1, fps * 55, (float)speedKPerFrame);
        }
    }
}
