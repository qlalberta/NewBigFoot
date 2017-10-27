using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereIsBigfoot
{
    class GameState
    {
        public string Name;
        public string Location;
        public List<string> Items;
        public List<string> History;
        public int level;

        public GameState()
        {
            Items = new List<string>();
            History = new List<string>();
        }

    }
}
