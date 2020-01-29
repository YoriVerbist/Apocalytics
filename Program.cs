using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Apocalythics
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var obj = JsonConvert.DeserializeObject<DataSetObj>(File.ReadAllText("dataset/hiv.json"));
            wpopulation wpop = new wpopulation("dataset/WorldPopulation.csv");

            List<PredictionMath> Calculations = new List<PredictionMath>();
           

            foreach (var o in obj.fact)
            {
                o.ValueFixed = new HivNumber(o.Value); //fix values
                Console.WriteLine($"{o.ValueFixed.average} - {o.dims.COUNTRY} - {o.dims.GHO} - {o.dims.YEAR}");

                PredictionMath CurrentMath = Calculations.Find(x => x.country == o.dims.COUNTRY);
                if (CurrentMath == null)
                {
                    Calculations.Add(new PredictionMath(o.dims.COUNTRY, o.ValueFixed, o.dims.YEAR, wpop));
                }
                   
                else
                {
                    CurrentMath.value.Add(o.ValueFixed);
                    CurrentMath.year.Add(Convert.ToInt32(o.dims.YEAR));
                }
                
            }


            foreach (var c in Calculations)
            {
                c.CalculateDiffrence();
 
                for(int i = 0; i < c.year.Count; i++)
                {
                    if (c.year[i] == 2000) //start jaar
                        Console.WriteLine($"{c.country}: {c.year[i]}: kills: {c.value[i].average}/{c.wpop.GetCountryMax(c.country, c.year[i])} ({c.WorldProcent[i].ToString("0.0000")}%)");
                    else
                        Console.WriteLine($"{c.country}: {c.year[i]}: kills: {c.value[i].average}/{c.wpop.GetCountryMax(c.country, c.year[i])} ({c.WorldProcent[i].ToString("0.0000")}%), Change: {c.IncreaseProcent[i].ToString("0.0000")}%");
                }
                Console.WriteLine("--------------------------------------");
            }

            FindOutbreakCountry(Calculations);

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        static void FindOutbreakCountry(List<PredictionMath> predictionMaths)
        {
            for (int i = 0; i < predictionMaths.Count; i++)
                predictionMaths[i].Evaluate();

            var HighestScore = predictionMaths.Max(x => x.outbreakScore);
            var outbreakZone = predictionMaths.Find(x => x.outbreakScore == HighestScore);

            Console.WriteLine($"{outbreakZone.country}");

        }
    }
}
