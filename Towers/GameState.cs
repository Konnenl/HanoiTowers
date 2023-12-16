using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace Towers
{
    internal class GameState
    {
        public int step = 0;
        public mode gameMode = mode.Inactive;
        public int ringCount = 0;
        public double speed = 10;
        public string[] colors = new string[8] { "#117a65", "#138d75", "#16a085", "#45b39d", "#73c6b6", "#a2d9ce", "#d0ece7", "#daf0ec" };

        public List<Stack<Rectangle>> listColumn = new List<Stack<Rectangle>>();
        public int deltaWidth = 20;
        public enum mode
        {
            Inactive,
            Manual,
            Auto
        }

        public GameState()
        {
            listColumn.Add(new Stack<Rectangle>());
            listColumn.Add(new Stack<Rectangle>());
            listColumn.Add(new Stack<Rectangle>());
        }
    }
}
