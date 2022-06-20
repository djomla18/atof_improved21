using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace atof_improved
{
    class Program
    {
        public static string[] headers;
        static List<ImportedValue> lstImportedValues = new List<ImportedValue>();
        static List<OutputValue> lstOutputValues = new List<OutputValue>();
        static List<string[]> listOfStringLines = new List<string[]>();
        static string path;

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the exact path of the input.csv file: ");
            path = Console.ReadLine().Trim();

            ReadCsv(); // citanje podataka iz CSV fajla
            
            for(int i = 0; i < lstImportedValues.Count; i++)
            {
                if (!CheckIfEverythingIsOk(lstImportedValues[i].Vrednost.ToCharArray()))
                {
                    string[] error = { $"Line {i + 1} cannot be converted into a number. Original value {lstImportedValues[i].Vrednost} date {lstImportedValues[i].Datum.ToString("dd.MM.yyyy.")}" };
                    File.WriteAllLines(path + @"\output.err", error);
                    continue;
                }
            }
            Calculate();
            WriteCsv();
        }

        private static double atof_improved(char[] str)
        {
            double result = 0;
            int numberSign = 1; // 1 - pozitivno, -1 - negativno
            int i = 0;
            int dIndex = 0;
            double e = 0; // incijalizujemo vrednost broja e iz naseg stringa, koja je za sada 0

            if (str[0].Equals('-'))
            {
                numberSign = -1; // ako je minus, menjamo znak i pomeramo iterator za 1
                i++;
            }

            for(; i < str.Length;)
            {
                if(str[i] == '.')
                {
                    // pamtimo indeks na kojem se nalazi tacka i kada naidjemo na nju samo iterator povecavamo za jedan i nastavljamo dalje
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

            if(dIndex != 0 && e == 0)
            // Isti princip kao i kod racunanja broj decimala kada imam broj e, samo sto je ovo slucaj
            // kada nam je kao vrednost prosledjen decimalni broj.
            {
                int decimalLength = str.Length - (dIndex + 1);
                int valueDecimals = GetNumberOfDecimals(decimalLength);
                result = result / valueDecimals;
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
            TextFieldParser csvParser = new TextFieldParser(path + @"\input.csv");

            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;


            headers = (csvParser.ReadLine()).Split(',', 3);

            for (int i = 0; i < headers.Length; i++)
            {
                headers[i] = headers[i].Trim('"');
            }

            EvaluateFields(csvParser);
        }
        private static void EvaluateFields(TextFieldParser csvParser)
        {
            while (!csvParser.EndOfData)
            {
                string line = csvParser.ReadLine();
                char[] parameters = new char[] { ',' };
                string[] subStrings = line.Split(parameters);
                for (int i = 0; i < subStrings.Length; i++)
                {
                    subStrings[i] = subStrings[i].Trim('"');
                }
                string[] formats = new string[]{ "dd.M.yyyy", "dd.MM.yyyy.", "dd/M/yyyy" };
                DateTime dt = DateTime.ParseExact(subStrings[0], formats, null );
                lstImportedValues.Add(new ImportedValue(dt, subStrings[1], subStrings[2]));
            }
        }
        private static void WriteCsv()
        {
            string headerText = "Mesec," + "Godina," + "UkupnoMerenja," + "Suma\n";
            
            File.WriteAllText(path + @"\output.csv", headerText);

            for(int i =0; i < lstOutputValues.Count; i++) { 

                string forWrite = $"{lstOutputValues[i].Mesec}" + "," + "2022," + $"{lstOutputValues[i].UkupnoMerenja}" + "," + $"{lstOutputValues[i].Suma}\n";
                File.AppendAllText(path + @"\output.csv", forWrite);   
            }
        }
        private static void Calculate()
        {
            double sum = 0;
            List<int> evaluatedMonths = new List<int>();
            int counter = 0;

            for(int i = 0; i < lstImportedValues.Count; i++)
            {
                if (CheckIfEverythingIsOk(lstImportedValues[i].Vrednost.ToCharArray()))
                {
                    int month = lstImportedValues[i].Datum.Month;
                    if (!evaluatedMonths.Contains(month))
                    {
                        counter++;
                        sum += atof_improved(lstImportedValues[i].Vrednost.ToCharArray());
                        for (int j = i + 1; j < lstImportedValues.Count; j++)
                        {
                            if (CheckIfEverythingIsOk(lstImportedValues[j].Vrednost.ToCharArray()))
                            {
                                if (lstImportedValues[j].Datum.Month == month)
                                {
                                    sum += atof_improved(lstImportedValues[j].Vrednost.ToCharArray());
                                    counter++;
                                }
                            }
                        }

                        evaluatedMonths.Add(month);
                        if (!GetMonth(month).Equals("Error"))
                        {
                            lstOutputValues.Add(new OutputValue(GetMonth(month), counter, sum));
                        }
                        counter = 0;
                        sum = 0;
                    }
                }
            }

        }
        private static string GetMonth(int monthNumber)
        {
            switch (monthNumber)
            {
                case 1: return "Januar";
                case 2: return "Februar";
                case 3: return "Mart";
                case 4: return "April";
                case 5: return "Maj";
                case 6: return "Jun";
                case 7: return "Jul";
                case 8: return "Avgust";
                case 9: return "Septembar";
                case 10: return "Oktobar";
                case 11: return "Novembar";
                case 12: return "Decembar";
                default: return "Error";
            }
        }
    }
}
