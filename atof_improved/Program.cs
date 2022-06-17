using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace atof_improved
{
    class Program
    {
        // public static char[] chars;
        public static string[] headers;
        static List<importedValue> importedValues = new List<importedValue>();
        static List<string[]> listOfStringLines = new List<string[]>();
        // public static string[] sepratedValues;

        static void Main(string[] args)
        {
            //ReadCsv(); // citanje podataka iz CSV fajla
            //SeprateValues();

            char[] arrayOfChars = new char[] { '1', '2', '3'}; // testni niz

            if (CheckIfEverythingIsOk(arrayOfChars)) // validacija da li je uneti niz karaktera u dobrom formatu
            {
                Console.WriteLine(atof_improved(arrayOfChars));
            }
            else
            {
                throw new Exception("Number cannot be converted");
            }
        }

        private static double atof_improved(char[] str)
        {
            double result = 0;
            int numberSign = 1; // 1 - positive, -1 - negative
            int i = 0;
            int dIndex = 0;
            double e = 0; // incijalizujemo vrednost broja e iz naseg stringa, koja je za sada 0

            if (str[0].Equals('-'))
            {
                numberSign = -1;
                i++;
            }

            for(; i < str.Length;)
            {
                if(str[i] == '.')
                {
                    // pamtimo indeks na kojem se nalazi tacka i kada naidjemo na nju samo i povecavamo za jedan i nastavljamo dalje
                    dIndex = i; 
                    ++i;
                    continue;
                }
                if (str[i] != 'e')
                {
                    result = result * 10 + str[i] - '0';
                    ++i;
                }
                else
                {
                    if(i == str.Length-1 || i + 1 == str.Length - 1)
                    {
                        throw new Exception("Error! Number cannot be converted");
                    }
                    
                    string numberE = new string(str, i, str.Length - i); // odvajanje dela niza pocevisi od pozicije
                                                                         // karaktera e pa do kraja stringa
                    
                    e += NumEToNum(numberE); // konvertovanje broja e u double vrednost 
                    if (dIndex != 0)
                    {
                        // Uzimajuci u obzir da smo karakter '.' preskocili i nastavili sa konverzijom kao da tog
                        // karaktera nije ni bilo, ovde racunamo koliko decimalnih mesa je bilo od karaktera
                        // '.' do broja e. Na ovaj nacin cemo nakon konverzije dobijeni rezultat da podelimo
                        // sa 10^brojDecimala kako bi konverzija bila tacna.
                        int lengthDecimals = i - (dIndex + 1); // broj decimala
                        int valueDecimals = GetNumberOfDecimals(lengthDecimals); // 10^brojDecimala
                        result = result * e / valueDecimals; // rezultat
                    }
                    else
                    {
                        result = result * e;
                    }
                }

                // Pod pretpostavkom da svaki broj nakon znaka -/+ predstavlja eksponent za "broj" e
                // mozemo reci da smo dosli do kraja niza karaktera kada se izracuna vrednost broja e.
                // U suportnom ce se petlja prekinuti kada dodje do poslednjeg karaktera.
                if (e != 0)
                {
                    break;
                }
               
            }

            if(result > double.MaxValue || result < double.MinValue)
            {
                throw new Exception("Error! Number cannot be converted");
            }

            return result * numberSign;
        }
        private static int GetNumberOfDecimals(int lengthDecimals)
        {
            int result = 1;

            for(int i = 0; i < lengthDecimals; i++)
            {
                result *= 10;
            }

            return result;
        }
        private static double NumEToNum(string numberE)
        {
            var digits = from c in numberE // izvlacimo u poseban niz brojeve koje se nalaze nakon znaka -/+
                         where Char.IsDigit(c)
                         select c;

            double exponent = atof_improved(digits.ToArray()); // broj ili brojeve koje smo izvukli, a tipa su char
                                                               // konvertujemo u jedan broj tipa double
            double result = 0;
            int temp = 1;
            for (int i = 0; i < exponent; i++)
            {
                temp *= 10;
            }

            if (numberE[1] == '-')
            {
                result += (double)1 / temp;
            }
            else if(numberE[1] == '+')
            {
                result += 1 * temp;
            }
            else
            {
                throw new Exception("Number cannot be converted");
            }

            if(result > Double.MaxValue || result < Double.MinValue)
            {
                throw new Exception("Number cannot be converted");
            }

            return (double)result;
        }
        private static bool CheckIfEverythingIsOk(char[] str)
        {
            foreach(char ch in str)
            {
                if (!IsNumber(ch)) // Da li je karakter broj?
                {
                    if(ch != '+' && ch != '-' && ch != 'e' && ch != '.') // Ukoliko nije broj, da li odgovara nekom od
                                                                         // zadatih karaktera?
                    {
                        return false;
                    }
                } 
            }
            return true;
        }
        private static bool IsNumber(char ch)
        {
            return (ch >= 48 && ch <= 57);
        }
        private static void ReadCsv()
        {
            TextFieldParser csvParser = new TextFieldParser(@"C:\Users\Mladen PC\Desktop\input.csv");

            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;


            headers = (csvParser.ReadLine()).Split(',', 3);

            for (int i = 0; i < headers.Length; i++)
            {
                headers[i] = headers[i].Trim('"');
            }
            while (!csvParser.EndOfData)
            {
                listOfStringLines.Add(csvParser.ReadFields());
            }
        }
        private static void SeprateValues()
        {
            for (int i = 0; i < listOfStringLines.Count; i++)
            {
                string[] line = listOfStringLines[i];

                for (int j = 0; j < line.Length; j++)
                {
                    //sepratedValues = line[j].S
                }

                
            }
        }

    }

    class importedValue
    {
        public string Datum { get; set; }
        public string Vrednost { get; set; }
        public string Komentar { get; set; }
        public importedValue()
        {

        }
        public importedValue(string Datum, string Vrednost, string Komentar)
        {
            this.Datum = Datum;
            this.Vrednost = Vrednost;
            this.Komentar = Komentar;
        }
    }
}
