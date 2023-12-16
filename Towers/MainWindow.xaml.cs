using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Towers
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameState gameState = new GameState();

        int from = -1;
        int to = -1;


        bool goUp = true;
        bool goDown = false;
        bool goLeft = false;
        bool goRight = false;
        int directionsCount;


        Queue <Point> directions = new Queue<Point>();
        Point curDir = new Point();


        Rectangle cur = null;
        DispatcherTimer timer = new DispatcherTimer();




        public MainWindow()
        {
            InitializeComponent();
            labelStep.Content = gameState.ringCount.ToString();
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
            canvas.Children.Clear();
            rings.SelectedItem = null;

            Manual.Background = new SolidColorBrush(Color.FromRgb(29, 30, 37));
            Auto.Background = new SolidColorBrush(Color.FromRgb(29, 30, 37));

            gameState.gameMode = GameState.mode.Inactive;
            gameState.ringCount = 0;
            gameState.listColumn[0].Clear();
            gameState.listColumn[1].Clear();
            gameState.listColumn[2].Clear();

            goUp = true;
            goDown = false;
            goLeft = false;
            goRight = false;

            gameState.step = 0;
            from = -1;
            to = -1;
            directions.Clear();
            cur = null;
            timer.Stop();
            Speed.Value = 10;
            gameState.speed = 10;

            labelStep.Content = gameState.step.ToString();
        }

        private void Rings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem != null){
                gameState.ringCount = Int32.Parse(((ComboBoxItem)(((ComboBox)sender).SelectedItem)).Content.ToString());
            }
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


                    gameState.listColumn[0].Push(temp);
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
             

            if (gameState.gameMode == GameState.mode.Auto)
            {
                AutoMode(gameState.ringCount, 0, 2, 1);

                curDir = directions.Dequeue();
                directionsCount = directions.Count + 1;
                from = (int)curDir.X;
                to = (int)curDir.Y;
                cur = gameState.listColumn[from].Pop();

                gameState.step++;
                labelStep.Content = gameState.step;

                timer.Tick += TimerEvent;
                timer.Interval = TimeSpan.FromMilliseconds(gameState.speed);
                timer.Start();
            }
        }
        private void Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (true)
            {
                gameState.speed = Math.Round(Speed.Value + 0.49, 2);
            }
        }




        // АВТОМАТИЧЕСКИЙ
        private void AutoMode(int n, int fromAuto, int toAuto, int unused)
        {
            if (n <= 0)
            {
                return;
            }
            AutoMode(n - 1, fromAuto, unused, toAuto);
            directions.Enqueue(new Point(fromAuto, toAuto));
            AutoMode(n - 1, unused, toAuto, fromAuto);
            return;
        }

        private void TimerEvent(object sender, EventArgs e)
        {
            if (directionsCount != 0)
            {
                if (goUp)
                {
                    Canvas.SetBottom(cur, Canvas.GetBottom(cur) + 5);
                    if (Canvas.GetBottom(cur) == 250)
                    {
                        switch (from)
                        {
                            case 0:
                                goRight = true;
                                break;
                            case 1:
                                if (to == 0) goLeft = true;
                                else if (to == 2) goRight = true;
                                break;
                            case 2:
                                goLeft = true;
                                break;
                        }
                        goUp = false;
                    }
                    
                }
                else if (goDown)
                {
                    Canvas.SetBottom(cur, Canvas.GetBottom(cur) - 5);
                    if (Canvas.GetBottom(cur) == gameState.listColumn[to].Count * 25)
                    {
                        goDown = false;
                        directionsCount--;
                        gameState.listColumn[to].Push(cur);
                        if (directions.Count > 0)
                        {
                            curDir = directions.Dequeue();
                            from = (int)curDir.X;
                            to = (int)curDir.Y;
                            cur = gameState.listColumn[from].Pop();
                            goUp = true;
                            labelStep.Content = ++gameState.step;
                        }
                    }

                }
                else if (goLeft)
                {
                    Canvas.SetLeft(cur, Canvas.GetLeft(cur) - 5);
                    if (Canvas.GetLeft(cur) == 110 + (250 * to) - cur.Width / 2)
                    {
                        goLeft = false;
                        goDown = true;
                    }
                }
                else if (goRight)
                {
                    Canvas.SetLeft(cur, Canvas.GetLeft(cur) + 5);
                    if (Canvas.GetLeft(cur) == 110 + (250 * to) - cur.Width / 2)
                    {
                        goRight = false;
                        goDown = true;
                    }
                }
            }
            else
            {
                timer.Stop();
                messageBox m = new messageBox();
                m.Owner = this;
                m.mbLabel.Content = "Игра пройдена!\nКоличество шагов: " + gameState.step;
                m.Show();
            }
        }






        // РУЧНОЙ
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
            Ring_To(dropPosition);
            // canvas 720:284 (240)
            if (Correct_Drop(to))
            {
                if (from == to)
                {
                    Canvas.SetBottom(cur, (gameState.listColumn[to].Count() - 1) * 25);
                    //шаг не считаем
                }
                else
                {
                    Canvas.SetBottom(cur, gameState.listColumn[to].Count() * 25);
                    gameState.step++;
                }
                Canvas.SetLeft(cur, 110 + (250 * to) - cur.Width / 2);
                gameState.listColumn[from].Pop();
                gameState.listColumn[to].Push(cur);
            }
            else
            {
                Canvas.SetBottom(cur, (gameState.listColumn[from].Count() - 1) * 25);
                Canvas.SetLeft(cur, 110 * (from + 1) - cur.Width / 2);
            }

            labelStep.Content = gameState.step.ToString();
            from = -1;
            if (Done())
            {
                messageBox m = new messageBox();
                m.Owner = this;
                m.mbLabel.Content = "Игра пройдена!\nКоличество шагов: " + gameState.step;
                m.Show();
            }
        }
        private void ringDragOver(object sender, DragEventArgs e)
        {
            Point dropPosition = e.GetPosition(canvas);

            Canvas.SetLeft(cur, dropPosition.X - cur.Width/2);
            Canvas.SetBottom(cur, (285 - dropPosition.Y) - cur.Height/2); 

        }






        // ДОП
        private void Ring_From(Rectangle e)
        {
            for (int i = 0; i < 3; i++)
            {
                if (gameState.listColumn[i].Any() && e == gameState.listColumn[i].Peek())
                {
                    from = i;
                    break;
                }
            }
        }

        private void Ring_To(Point position)
        {
            if (position.X >= 0 && position.X <= 240 && Correct_Drop(0))
            {
                to = 0;
            }
            else if (position.X > 240 && position.X <= 480 && Correct_Drop(1))
            {
                to = 1;
            }
            else if (position.X > 480 && position.X <= 720 && Correct_Drop(2))
            {
                to = 2;
            }
        }

        private bool Correct_Drop(int n)
        {
            if (gameState.listColumn[n].Any())
            {
                return (cur.Width <= gameState.listColumn[n].Peek().Width);
            }
            return true;
        }

        private bool Correct_Top()
        {
            if (from >= 0)
            {
                return cur == gameState.listColumn[from].Peek();
            }
            return false;
        }

        private bool Done()
        {
            return gameState.listColumn[0].Count == 0 && (gameState.listColumn[1].Count == 0 && gameState.listColumn[2].Any() || gameState.listColumn[1].Any() && gameState.listColumn[2].Count == 0);
        }

    }
}
