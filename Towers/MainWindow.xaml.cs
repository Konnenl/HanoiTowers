using System;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

// TODO 
//      счёт шагов
//      изменение скорости
//      убрать счет шагов на месте

namespace Towers
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameState gameState = new GameState();
        int from = -1;
        Rectangle cur = null;
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            labelRings.Content = gameState.ringCount.ToString();
        }

        private void Button_Click_Manual(object sender, RoutedEventArgs e)
        {
            Manual.Background = new SolidColorBrush(Color.FromRgb(51, 52, 65));
            if (gameState.gameMode == GameState.mode.Auto)
            {
                Auto.Background = new SolidColorBrush(Color.FromRgb(29, 30, 37));
            }
            gameState.gameMode = GameState.mode.Manual;
        }

        private void Button_Click_Auto(object sender, RoutedEventArgs e)
        {
            Auto.Background = new SolidColorBrush(Color.FromRgb(51, 52, 65));
            if (gameState.gameMode == GameState.mode.Manual)
            {
                Manual.Background = new SolidColorBrush(Color.FromRgb(29, 30, 37));
            }
            gameState.gameMode = GameState.mode.Auto;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Rings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gameState.ringCount = Int32.Parse(((ComboBoxItem)(((System.Windows.Controls.ComboBox)sender).SelectedItem)).Content.ToString());
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (gameState.gameMode != GameState.mode.Inactive && gameState.ringCount != 0)
            {
                canvas.Children.Clear();
                for (int i = 0; i < gameState.ringCount; i++)
                {
                    Rectangle temp = new Rectangle();
                    temp.Width = 170 - i * gameState.deltaWidth;
                    temp.Height = 25;
                    temp.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(gameState.colors[i]));
                    if (gameState.gameMode == GameState.mode.Manual)
                    {
                        temp.MouseMove += Temp_MouseMove;
                    }
                    temp.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

                    gameState.first.Push(temp);
                    canvas.Children.Add(temp);
                    Canvas.SetBottom(temp, i * 25);
                    Canvas.SetLeft(temp, 110 - (170/2 - gameState.deltaWidth/2 * i));
                }

            }
            else
            {
                messageBox m = new messageBox();
                m.Owner = this;
                if (gameState.gameMode == GameState.mode.Inactive && gameState.ringCount != 0) {
                    m.mbLabel.Content = "Выберите режим игры";
                }
                else if (gameState.gameMode != GameState.mode.Inactive && gameState.ringCount == 0)
                {
                    m.mbLabel.Content = "Укажите количество колец";
                }
                else
                {
                    m.mbLabel.Content = "Выберите режим игры\nи укажите количество колец";
                }
                m.Show();
            }
             
            if (gameState.gameMode == GameState.mode.Manual)
            {
                ManualMode();
            }
            else
            {
                AutoMode();
            }
        }

        private void ManualMode()
        {

        }
        private void AutoMode()
        {

        }


        // ПЕРЕДВИЖЕНИЕ В РУЧНОЙ РЕЖИМЕ
        private void Temp_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            cur = (Rectangle)sender;
            Ring_From(cur);
           
            if (e.LeftButton == MouseButtonState.Pressed && from != -1 && Correct_Top())
            {
                DragDrop.DoDragDrop((Rectangle)sender, (Rectangle)sender, DragDropEffects.Move);
            }
        }

        private void ringDrop(object sender, DragEventArgs e)
        {
            Point dropPosition = e.GetPosition(canvas);
            // canvas 720:284 (240)

            //TODO анимация возвращение если нельзя перенести
            if (dropPosition.X >= 0 && dropPosition.X <= 240 && Correct_Drop(0))
            {
                if (from == 0) Canvas.SetBottom(cur, (gameState.first.Count() - 1) * 25);
                else Canvas.SetBottom(cur, gameState.first.Count() * 25);
                Canvas.SetLeft(cur, 110 - cur.Width/2);
                Delete();
                gameState.first.Push(cur);
                gameState.step++;
            }
            else if(dropPosition.X >240 && dropPosition.X <= 480 && Correct_Drop(1))
            {
                if (from == 1) Canvas.SetBottom(cur, (gameState.second.Count() - 1) * 25);
                else Canvas.SetBottom(cur, gameState.second.Count() * 25);
                Canvas.SetLeft(cur, 360 - cur.Width/2);
                Delete();
                gameState.second.Push(cur);
                gameState.step++;
            }
            else if(dropPosition.X >480 && dropPosition.X <= 720 && Correct_Drop(2))
            {
                if (from == 2) Canvas.SetBottom(cur, (gameState.third.Count() - 1) * 25);
                else Canvas.SetBottom(cur, gameState.third.Count() * 25);
                Canvas.SetLeft(cur, 610 - cur.Width/2);
                Delete();
                gameState.third.Push(cur);
                gameState.step++;
            }
            else
            {
                if (from == 0)
                {
                    Canvas.SetBottom(cur, (gameState.first.Count() - 1) * 25);
                    Canvas.SetLeft(cur, 110 - cur.Width / 2);
                }
                else if (from == 1)
                {
                    Canvas.SetBottom(cur, (gameState.second.Count() - 1) * 25);
                    Canvas.SetLeft(cur, 360 - cur.Width / 2);
                }
                else if (from == 2)
                {
                    Canvas.SetBottom(cur, (gameState.third.Count() - 1) * 25);
                    Canvas.SetLeft(cur, 610 - cur.Width / 2);
                }
            }
            labelRings.Content = gameState.step.ToString();
            from = -1;
            if (Done())
            {
                messageBox m = new messageBox();
                m.Owner = this;
                m.mbLabel.Content = "Игра пройдена!";
                m.Show();
                // сделать сброс или что-то другое
            }
        }
        private void ringDragOver(object sender, DragEventArgs e)
        {
            Point dropPosition = e.GetPosition(canvas);

            Canvas.SetLeft(cur, dropPosition.X - cur.Width/2);
            Canvas.SetBottom(cur, (285 - dropPosition.Y) - cur.Height/2); 

        }



        // ДОП ФУНКЦИИ
        private void Ring_From(Rectangle e)
        {
            if (gameState.first.Any() && e == gameState.first.Peek())
            {
                from = 0;
            }
            else if (gameState.second.Any() && e == gameState.second.Peek())
            {
                from = 1;
            }
            else if (gameState.third.Any() && e == gameState.third.Peek())
            {
                from = 2;
            }
        }

        private void Delete()
        {
            Rectangle udaliPotom = null;
            if (from == 0) udaliPotom = gameState.first.Pop();
            else if (from == 1) udaliPotom = gameState.second.Pop();
            else if (from == 2) gameState.third.Pop();
        }

        private bool Correct_Drop(int n)
        {
            if (n == 0 && gameState.first.Any())
            {
                return (cur.Width <= gameState.first.Peek().Width);
            }
            else if (n == 1 && gameState.second.Any())
            {
                return (cur.Width <= gameState.second.Peek().Width);
            }
            else if (n == 2 && gameState.third.Any())
            {
                return (cur.Width <= gameState.third.Peek().Width);
            }
            return true;
        }

        private bool Correct_Top()
        {
            if (from == 0) return cur == gameState.first.Peek();
            else if (from == 1) return cur == gameState.second.Peek();
            else if (from == 2) return cur == gameState.third.Peek();
            return false;
        }

        private bool Done()
        {
            return gameState.first.Count == 0 && (gameState.second.Count == 0 && gameState.second.Any() || gameState.second.Any() && gameState.third.Count == 0);
        }
    }
}
