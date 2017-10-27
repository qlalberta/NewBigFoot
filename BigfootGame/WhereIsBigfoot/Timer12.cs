using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereIsBigfoot
{
    class Timer12
    {
        private int mm = 59;
        private int hh = 2;
        bool timeUp = false;

        public Timer12(int hh, int mm)
        {
            this.hh = hh;
            this.mm = mm;
        }

        public void CountDown()
        {
            if (this.mm == 0 && this.hh == 0)
            {
                this.timeUp = true;
            }
            else if (this.mm == 0)
            {
                this.mm = 59;
                this.hh--;

            }
            else
            {
                this.mm--;
            }
        }

        public string GetTime()
        {
            if (this.mm < 10)
            {
                return "0" + this.hh + ":0" + this.mm;
            }
            else
            {

                return "0" + this.hh + ":" + this.mm;
            }
        }

        public bool TimeUp
        {
            get { return this.timeUp; }
            set { this.timeUp = value; }
        }      
    }
}
