using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Apocalythics
{
    public class WorldPopulation
    {
        //HDI Rank;Country;1980;1985;1990;2000;2005;2006;2007;2008;2009;2010;2011;2012;2013
        //Type;Name;Country Code; Area(km²);1990;2000;2010;2011;2012;2013;2014;2015;2016;2017;2018
        public string type { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string arekm { get; set; }
        public int y1990 { get; set; }
        public int y2000 { get; set; }
        public int y2010 { get; set; }
        public int y2011 { get; set; }
        public int y2012 { get; set; }
        public int y2013 { get; set; }
        public int y2014 { get; set; }
        public int y2015 { get; set; }
        public int y2016 { get; set; }
        public int y2017 { get; set; }
        public int y2018 { get; set; }

        public WorldPopulation(string[] colums)
        {
            type = colums[0];
            country = colums[1];
            countryCode = colums[2];
            arekm = colums[3];
            int val = 0;
            int.TryParse(colums[4], out val);
            y1990 = val;
            int.TryParse(colums[5], out val);
            y1990 = val;
            int.TryParse(colums[6], out val);
            y2010 = val;
            int.TryParse(colums[7], out val);
            y2011 = val;
            int.TryParse(colums[8], out val);
            y2012 = val;
            int.TryParse(colums[9], out val);
            y2013 = val;
            int.TryParse(colums[10], out val);
            y2014 = val;
            int.TryParse(colums[11], out val);
            y2015 = val;
            int.TryParse(colums[12], out val);
            y2016 = val;
            int.TryParse(colums[13], out val);
            y2017 = val;
            int.TryParse(colums[14], out val);
            y2018 = val;
        }
    }
    public class wpopulation
    {
        public List<WorldPopulation> WorldPopulationList { get; set; }
        public wpopulation(string file)
        {
            WorldPopulationList = new List<WorldPopulation>();
            string allText = File.ReadAllText(file);
            string[] rows = allText.Split('\n');
            foreach(var r in rows)
            {
                string[] colums = r.Split(';');
                if(colums.Length > 1)
                {
                    var WorldPop = new WorldPopulation(colums);
                    WorldPopulationList.Add(WorldPop);
                }
            }
        }

        public decimal GetPopulationPercent(string country, decimal value, int year)
        {
            decimal target = GetCountryMax(country, year);
            if(target != 0)
                return (value / target) * 100;
            return 0;

        }

        public decimal GetCountryMax(string country, int year)
        {
            //var Country = this.WorldPopulationList.Find(x => x.country.ToLower().Replace(" ","") == country.ToLower().Replace(" ",""));
            var Country = this.WorldPopulationList.Find(x => x.country.Replace(" ","").Contains(country.Replace(" ",""), StringComparison.InvariantCultureIgnoreCase));
            if (Country == null)
                return -1;

            int target = 0;

            switch (year)
            {
                case 1999:
                    target = Country.y1990;
                    break;
                case 2000:
                    target = Country.y2000;
                    break;
                case 2010:
                    target = Country.y2010;
                    break;
                case 2011:
                    target = Country.y2011;
                    break;
                case 2012:
                    target = Country.y2012;
                    break;
                case 2013:
                    target = Country.y2013;
                    break;
                case 2014:
                    target = Country.y2014;
                    break;
                case 2015:
                    target = Country.y2015;
                    break;
                case 2016:
                    target = Country.y2016;
                    break;
                case 2017:
                    target = Country.y2017;
                    break;
                case 2018:
                    target = Country.y2018;
                    break;
            }
            if (target == 0)
                target = Country.y1990;
            if (target == 0)
                target = Country.y2000;
            if (target == 0)
                target = Country.y2010; //meh
            if (target == 0)
                target = Country.y2013;
            if (target == 0)
                target = Country.y2016;

            return target;

        }

    }
}
