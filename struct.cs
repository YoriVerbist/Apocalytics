using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Apocalythics
{
    public class Dimension
    {
        public string label { get; set; }
        public string display { get; set; }
    }

    public class Dims
    {
        public string COUNTRY { get; set; }
        public string GHO { get; set; }
        public string YEAR { get; set; }
    }

    public class Fact
    {
        public Dims dims { get; set; }
        public string Value { get; set; }
        public HivNumber ValueFixed { get; set; }
    }

    public class RootObject
    {
        public List<Dimension> dimension { get; set; }
        public List<Fact> fact { get; set; }
    }

    public class Dataset
    {
        public string label { get; set; }
        public string display { get; set; }
    }

    public class Attribute
    {
        public string label { get; set; }
        public string display { get; set; }
    }

    public class Code
    {
        public string label { get; set; }
        public string display { get; set; }
        public int display_sequence { get; set; }
        public string url { get; set; }
        public List<object> attr { get; set; }
    }

    public class Dim
    {
        public string category { get; set; }
        public string code { get; set; }
    }

    public class Value
    {
        public string display { get; set; }
        public double? numeric { get; set; }
        public object low { get; set; }
        public object high { get; set; }
        public object stderr { get; set; }
        public object stddev { get; set; }
    }


    public class DataSetObj
    {
        public string copyright { get; set; }
        public List<Dataset> dataset { get; set; }
        public List<Attribute> attribute { get; set; }
        public List<Dimension> dimension { get; set; }
        public List<Fact> fact { get; set; }
    }

    public class HivNumber
    {
        public int average { get; set; }
        public int max { get; set; }
        public int min { get; set; }

        public HivNumber()
        { 
        }

        public HivNumber(string context)
        {
            context = context.Replace(" ", "");
            int avg = 0;
            if(int.TryParse(context, out avg))
                this.average = avg; //main number
            else
            {
                //Pare main
                if (context != "Nodata")
                {
                    string[] Split = context.Split('[');
                    int val = 0;
                    if (int.TryParse(Split[0], out val)) //parse avg
                        this.average = val;
                    else
                        if (int.TryParse(Split[0].Substring(1,Split[0].Length-1), out val)) //parse avg
                            this.average = val;


                    //pars min & max
                    string[] Split2 = Split[1].Split('–');

                    //min
                    val = 0;
                    if(int.TryParse(Split2[0], out val))
                        this.min = val;
                    else
                        if (int.TryParse(Split2[0].Substring(1, Split2[0].Length - 1), out val)) //parse min
                            this.min = val;

                    //max
                    if (int.TryParse(Split2[1], out val))
                        this.min = val;
                    else
                        if (int.TryParse(Split2[1].Substring(1, Split2[1].Length - 1), out val)) //parse min
                        this.min = val;
                }
            }
        }
    }

    public class PredictionMath
    {
        public string country { get; set; }
        public List<HivNumber> value { get; set; }
        public List<int> year { get; set; }
        public List<decimal> IncreaseProcent { get; set; }
        public List<decimal> WorldIncreaseProcent { get; set; }
        public List<decimal> WorldProcent { get; set; }
        public wpopulation wpop { get; set; }
        public decimal outbreakScore { get; set; }


        public PredictionMath(string country, HivNumber hivNumber, string year, wpopulation wpop)
        {
            this.value = new List<HivNumber>();
            this.year = new List<int>();
            this.IncreaseProcent = new List<decimal>();
            this.WorldIncreaseProcent = new List<decimal>();
            this.WorldProcent = new List<decimal>();
            this.wpop = wpop;
            this.country = country;
            this.value.Add(hivNumber);
            this.year.Add(Convert.ToInt32(year));
            this.outbreakScore = 0;
        }

        public bool CalculateDiffrence()
        {
            if (value.Count < 2)
                return false; //error handling

            //firs year start first
            var minValue = this.year.Min();
            if (this.year[0] != minValue)
            {
                //not in correct order
                this.year.Reverse();
                this.value.Reverse();
            }

            for(int i = 0; i < this.year.Count; i++) //Shit's upside down ;_;
            {
                if (year[i] == minValue) //skip min value
                {
                    this.IncreaseProcent.Add(0);
                    this.WorldIncreaseProcent.Add(0);
                    this.WorldProcent.Add(wpop.GetPopulationPercent(this.country, this.value[i].average, this.year[i]));
                }
                else
                {
                    if (this.value[i].average != 0 && this.value[i-1].average != 0)
                    {
                        this.IncreaseProcent.Add(((decimal)this.value[i].average / (decimal)this.value[i-1].average) * 100); //((100 / 120) - 1) * 100
                        this.WorldIncreaseProcent.Add(wpop.GetPopulationPercent(this.country, (this.value[i-1].average / this.value[i].average) * 100, this.year[i])); //((120 / 100) - 1) * 100
                        this.WorldProcent.Add(wpop.GetPopulationPercent(this.country, this.value[i].average, this.year[i]));
                    }
                    else
                    {
                        this.IncreaseProcent.Add(0);
                        this.WorldIncreaseProcent.Add(0);
                        this.WorldProcent.Add(wpop.GetPopulationPercent(this.country, this.value[i].average, this.year[i]));
                    }
                }
                
            }
            return true;
        }

        public bool Evaluate()
        {
            for (int i = 0; i < this.IncreaseProcent.Count; i++)
            {
                this.outbreakScore = (this.value[i].average / ((i + 1) * 0.1m));
            }
            return true;
        }

        public bool CalculateNextDecenia()
        {
            if(this.IncreaseProcent[this.IncreaseProcent.Count - 2] == 0 || this.IncreaseProcent[this.IncreaseProcent.Count - 1] == 0)
            {
                this.IncreaseProcent.Add(0);
                this.year.Add(this.year.Max() + 10);
                this.value.Add(new HivNumber()
                {
                    average = 0
                });
                this.WorldProcent.Add(0);
                return false;
            }
            else
            {
                var factor = this.IncreaseProcent[this.IncreaseProcent.Count - 1] / this.IncreaseProcent[this.IncreaseProcent.Count - 2];
                var newChange = this.IncreaseProcent[this.IncreaseProcent.Count - 1] * factor;
                this.IncreaseProcent.Add(newChange);
                this.year.Add(this.year.Max() + 10);
                this.value.Add(new HivNumber()
                {
                    average = (int)(this.value.Last().average * (this.IncreaseProcent.Last()/100))
                });
                this.WorldProcent.Add(wpop.GetPopulationPercent(this.country, this.value.Last().average, this.year.Last()));

            }
            return true;
        }

    }
}
