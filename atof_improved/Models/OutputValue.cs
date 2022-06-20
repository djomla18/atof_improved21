using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atof_improved
{
    class OutputValue
    {
        public string Mesec { get; set; }
        public int UkupnoMerenja { get; set; }
        public double Suma { get; set; }

        public OutputValue()
        {

        }
        public OutputValue(string Mesec, int UkupnoMerenja, double Suma)
        {
            this.Mesec = Mesec;
            this.UkupnoMerenja = UkupnoMerenja;
            this.Suma = Suma;
        }
    }
}
