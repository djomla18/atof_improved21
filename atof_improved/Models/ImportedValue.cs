using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atof_improved
{ 
    class ImportedValue
    {
        public DateTime Datum { get; set; }
        public string Vrednost { get; set; }
        public string Komentar { get; set; }
        public ImportedValue()
        {

        }
        public ImportedValue(DateTime Datum, string Vrednost, string Komentar)
        {
            this.Datum = Datum;
            this.Vrednost = Vrednost;
            this.Komentar = Komentar;
        }
    }
}
