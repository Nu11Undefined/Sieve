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
    /// Система ввода информации
    /// </summary>
    public class Controller
    {
        bool _isLeftMouseDown = false; // для заселения модели
        bool _isRightMouseDown = false; // для смещения обзора
        bool _isMiddleMouseDown = false; // для перемещения окна отображения
        Point MouseDownPoint { get; set; }
        Point CurrentMousePosition { get; set; }
        /// <param name="win">Окно управления</param>
        public Controller(Control win)
        {
            Form main = win.FindForm();
            win.MouseDown += (s, e) =>
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _isLeftMouseDown = true;
                        CurrentMousePosition = new Point(e.X, e.Y);
                        break;
                    case MouseButtons.Right:
                        _isRightMouseDown = true;
                        CurrentMousePosition = new Point(e.X, e.Y);
                        break;
                    case MouseButtons.Middle:
                        _isMiddleMouseDown = true;
                        CurrentMousePosition = new Point(e.X, e.Y);
                        break;
                }
            };
            win.MouseUp += (s, e) =>
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _isLeftMouseDown = false;
                        break;
                    case MouseButtons.Right:
                        _isRightMouseDown = false;
                        break;
                    case MouseButtons.Middle:
                        _isMiddleMouseDown = false;
                        break;
                }
            };
            win.MouseMove += (s, e) =>
            {
                CursorPositionChanged?.Invoke(e.X, e.Y);
                // определить обработку только одной кнопки мыши
                if (_isLeftMouseDown)
                {
                    ShiftByPixel?.Invoke(e.X - CurrentMousePosition.X, e.Y - CurrentMousePosition.Y);
                    CurrentMousePosition = new Point(e.X, e.Y);
                } else if (_isMiddleMouseDown)
                {
                    main.Location = new Point(main.Location.X + e.X - CurrentMousePosition.X, main.Location.Y + e.Y - CurrentMousePosition.Y);
                }
            };
            win.MouseWheel += (s, e) =>
            {
                if (e.Delta > 0) ZoomIn?.Invoke(e.X, e.Y);
                else ZoomOut?.Invoke(e.X, e.Y);
            };
            win.KeyUp += (s, e) =>
            {
                Console.WriteLine(e.KeyValue);
                // нажатия на цифру и
                // ЛКМ - уменьшение параметра
                // ПКМ - увеличение параметра
                if (e.KeyValue>48 && e.KeyValue<58)
                {
                    if (_isLeftMouseDown)
                    {
                        ParameterReduced?.Invoke(e.KeyValue - 48);
                    }
                    if (_isRightMouseDown)
                    {
                        ParameterIncreased?.Invoke(e.KeyValue - 48);
                    }
                }
                switch ((HotKeys)e.KeyCode)
                {
                    case HotKeys.SpeedDown:
                        SpeedDown?.Invoke();
                        break;
                    case HotKeys.SpeedUp:
                        SpeedUp?.Invoke();
                        break;
                    case HotKeys.StopPlay:
                        StopPlay?.Invoke();
                        break;
                    case HotKeys.Statistic:
                        StatisticModeChange?.Invoke();
                        break;
                    case HotKeys.ChangeWindowMode:
                        WindowModeChanged?.Invoke();
                        break;
                    case HotKeys.FpsUp:
                    case HotKeys.FpsUp2:
                        FpsUp?.Invoke();
                        break;
                    case HotKeys.FpsDown:
                    case HotKeys.FpsDown2:
                        FpsDown?.Invoke();
                        break;
                    case HotKeys.SpfUp:
                        StagesByFrameUp?.Invoke();
                        break;
                    case HotKeys.SpfDown:
                        StagesByFrameDown?.Invoke();
                        break;
                    case HotKeys.Exit:
                        Exit?.Invoke();
                        break;

                }
            };
        }
        enum HotKeys : int
        {
            StopPlay = Keys.Space,
            Statistic = Keys.Tab,
            ChangeWindowMode = Keys.F11,
            Exit = Keys.Escape,
            FpsUp = Keys.NumPad7,
            FpsUp2 = Keys.Home,
            FpsDown = Keys.NumPad1,
            FpsDown2 = Keys.End,
            SpfUp = Keys.NumPad9,
            SpfDown = Keys.NumPad3,
            SpeedUp = Keys.NumPad8,
            SpeedDown = Keys.NumPad2,
        }

        #region ModelEvents

        /// <summary>
        /// Общие события
        /// </summary>
        public delegate void ModelEventHandler();

        public event ModelEventHandler SpeedUp;
        public event ModelEventHandler SpeedDown;
        /// <summary>
        /// Запустить/остановить воспроизведение
        /// </summary>
        public event ModelEventHandler StopPlay;
        /// <summary>
        /// Увеличить скорость отображения кадров
        /// </summary>
        public event ModelEventHandler FpsUp;
        /// <summary>
        /// Уменьшить скорость отображения кадров
        /// </summary>
        public event ModelEventHandler FpsDown;
        /// <summary>
        /// Увеличить количество этапов за один фрейм
        /// </summary>
        public event ModelEventHandler StagesByFrameUp;
        /// <summary>
        /// Уменьшить количество этапов за один фрейм
        /// </summary>
        public event ModelEventHandler StagesByFrameDown;
        /// <summary>
        /// Показать/скрыть статистику
        /// </summary>
        public event ModelEventHandler StatisticModeChange;
        /// <summary>
        /// Изменение оконного режима полноэкранный-стандартный
        /// </summary>
        public event ModelEventHandler WindowModeChanged;
        /// <summary>
        /// Выйти из программы
        /// </summary>
        public event ModelEventHandler Exit;

        /// <summary>
        /// Изменение параметров модели
        /// </summary>
        public delegate void ChangeParametersEventHandler(int param);

        /// <summary>
        /// Увеличение параметра
        /// </summary>
        public event ChangeParametersEventHandler ParameterIncreased;
        /// <summary>
        /// Уменьшение параметра
        /// </summary>
        public event ChangeParametersEventHandler ParameterReduced;

        #endregion

        #region VisualEvents

        /// <summary>
        /// Передача информации, связанной с координатами
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public delegate void LocationEventHandler(int x, int y);
        /// <summary>
        /// Приближение
        /// </summary>
        public event LocationEventHandler ZoomIn;
        /// <summary>
        /// Отдаление
        /// </summary>
        public event LocationEventHandler ZoomOut;
        /// <summary>
        /// Смещение по пикселям
        /// </summary>
        public event LocationEventHandler ShiftByPixel;
        /// <summary>
        /// Смещение по ячейкам
        /// </summary>
        public event LocationEventHandler ShiftByCell;
        /// <summary>
        /// Происходит при изменении положения прямоугольника отображения
        /// </summary>
        public event LocationEventHandler LocationChanged;
        /// <summary>
        /// Происходит при изменении размера прямоугольника отображения
        /// </summary>
        public event LocationEventHandler SizeChanged;
        /// <summary>
        /// Сообщает о необходимости инвертировать ячейку в текущей позиции курсора
        /// </summary>
        public event LocationEventHandler InvertCellAtLocation;
        /// <summary>
        /// Сообщает о необходимости оживить ячейку в текущей позиции курсора
        /// </summary>
        public event LocationEventHandler ReviveCellAtLocation;
        /// <summary>
        /// Сообщает о перемещении курсора
        /// </summary>
        public event LocationEventHandler CursorPositionChanged;
        #endregion

    }
}