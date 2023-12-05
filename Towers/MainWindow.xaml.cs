using System;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Towers
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameState gameState = new GameState();
        int from = -1;
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

        private void Temp_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle cur = (Rectangle)sender;
            Ring_From(cur);

            labelRings.Content = from.ToString();


            if (e.LeftButton == MouseButtonState.Pressed && from != -1)
            {
                DragDrop.DoDragDrop((Rectangle)sender, (Rectangle)sender, DragDropEffects.Move);
            }
        }

        private void ManualMode()
        {

        }

        private void ringDrop(object sender, DragEventArgs e)
        {
            Point dropPosition = e.GetPosition(canvas);
            //Rectangle cur = new Rectangle();
            // canvas 720:284 (240)
            Rectangle cur = (Rectangle)sender;
            /*if (from == 0)
            {
                cur = gameState.first.Pop();
            }
            else if (from == 1)
            {
                cur = gameState.second.Pop();
            }
            else if (from == 2)
            {
                cur = gameState.third.Pop();
            }*/
            if (dropPosition.X >= 0 && dropPosition.X <= 240)
            {
                Canvas.SetBottom(cur, gameState.first.Count() * 25);
                Canvas.SetLeft(cur, 117 - cur.Width/2);
                gameState.first.Push(cur);
            }
            else if(dropPosition.X >240 && dropPosition.X <= 480)
            {
                Canvas.SetBottom(cur, gameState.second.Count() * 25);
                //110 - (170 / 2 - gameState.deltaWidth / 2 * i)
                Canvas.SetLeft(cur, 360 - cur.Width/2);
                gameState.second.Push(cur);
            }
            else if(dropPosition.X >480 && dropPosition.X <= 720)
            {
                Canvas.SetBottom(cur, gameState.third.Count() * 25);
                Canvas.SetLeft(cur, 610 - cur.Width/2);
                gameState.third.Push(cur);
            }
        }

        private void AutoMode()
        {

        }

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

        private void ringDragOver(object sender, DragEventArgs e)
        {
            Point dropPosition = e.GetPosition(canvas);
            //Canvas.SetLeft()
        }
    }
}
