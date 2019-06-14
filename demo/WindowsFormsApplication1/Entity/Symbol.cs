using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo1
{
    public class Instrument
    {
        private string symbol;
        private string name;
        private double vol; 
        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public double Vol
        {
            get { return vol; }
            set { vol = value; }
        }
    }
}
