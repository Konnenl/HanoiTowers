using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Towers
{
    internal class GameState
    {
        public int step = 0;
        public mode gameMode = mode.Inactive;
        public int ringCount = 0;
        public int speed = 10;
        public string[] colors = new string[8] { "#117a65", "#138d75", "#16a085", "#45b39d", "#73c6b6", "#a2d9ce", "#d0ece7", "#daf0ec" };


        public Stack<Rectangle> first = new Stack<Rectangle>();
        public Stack<Rectangle> second = new Stack<Rectangle>();
        public Stack<Rectangle> third = new Stack<Rectangle>();


        public string[] Array = {};
        // public список цветов
        public int deltaWidth = 15;
        public enum mode
        {
            Inactive,
            Manual,
            Auto
        }
    }
}
