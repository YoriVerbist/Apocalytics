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

            Console.WriteLine("Read?");
            Console.ReadKey();

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
                    if (c.year[i] == c.year.Max()) //start jaar
                        Console.WriteLine($"{c.country}: {c.year[i]}: kills: {c.value[i].average}/{c.wpop.GetCountryMax(c.country, c.year[i])} ({c.WorldProcent[i].ToString("0.0000")}%)");
                    else
                        Console.WriteLine($"{c.country}: {c.year[i]}: kills: {c.value[i].average}/{c.wpop.GetCountryMax(c.country, c.year[i])} ({c.WorldProcent[i].ToString("0.0000")}%), Change: {c.IncreaseProcent[i].ToString("0.0000")}%");
                }
                Console.WriteLine("--------------------------------------");
            }

            FindOutbreakCountry(Calculations);

            //while (true)
            //{
                //Console.WriteLine("Calculate next decenia");
                Console.ReadKey();

                foreach (var c in Calculations)
                {
                    c.CalculateNextDecenia();

                    for (int i = 0; i < c.year.Count; i++)
                    {
                        if (c.year[i] == c.year.Min()) //start jaar
                            Console.WriteLine($"{c.country}: {c.year[i]}: kills: {c.value[i].average}/{c.wpop.GetCountryMax(c.country, c.year[i])} ({c.WorldProcent[i].ToString("0.0000")}%)");
                        else
                            Console.WriteLine($"{c.country}: {c.year[i]}: kills: {c.value[i].average}/{c.wpop.GetCountryMax(c.country, c.year[i])} ({c.WorldProcent[i].ToString("0.0000")}%), Change: {c.IncreaseProcent[i].ToString("0.0000")}%");
                    }
                    Console.WriteLine("--------------------------------------");
                }
            //}

            MergerPredictions(Calculations, ref obj);
            string DatasetString = JsonConvert.SerializeObject(obj);
            File.WriteAllText("dataset/hiv_predict.json", DatasetString);
            File.WriteAllText("dataset/hiv_predict.csv", ConvertToCsv(Calculations));

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        static void FindOutbreakCountry(List<PredictionMath> predictionMaths)
        {
            for (int i = 0; i < predictionMaths.Count; i++)
                predictionMaths[i].Evaluate();

            var HighestScore = predictionMaths.Max(x => x.outbreakScore);
            var outbreakZone = predictionMaths.Find(x => x.outbreakScore == HighestScore);

            //Console.WriteLine($"Outbreak Country: {outbreakZone.country}");
            printBanner($"{outbreakZone.country} after we found out:");

        }

        static void MergerPredictions(List<PredictionMath> predictionMaths, ref DataSetObj dataset)
        {
            foreach(var p in predictionMaths)
            {
                Dims dims = new Dims()
                {
                    COUNTRY = p.country,
                    YEAR = p.year.Last().ToString()
                };
                Fact fact = new Fact()
                {
                    Value = p.value.Last().average.ToString(),
                    dims = dims

                };
                dataset.fact.Add(fact);
            }
        }

        static string ConvertToCsv(List<PredictionMath> predictionMaths)
        {
            //country,year,value,country_population,changeProcent,CountryProcent
            string csvContent = "";
            foreach(var p in predictionMaths)
            {
                for (int i = 0; i < p.year.Count; i++)
                    csvContent += $"{p.country};{p.year[i]};{p.value[i].average};{p.wpop.GetCountryMax(p.country, p.year[i]).ToString()};{p.IncreaseProcent[i].ToString()};{p.WorldProcent[i].ToString()}\n";
            }
            return csvContent;
        }


        static void printBanner(string text)
        {
            //Console.WriteLine(@"      ###,                                                                                        @@@@                                             (##                                                  ");
            //Console.WriteLine(@"      (%#                                                                                         @@@@                                             ##&.                                                 ");
            //Console.WriteLine(@"      ###                                                                                         @@@@                                             ###*                                                 ");
            //Console.WriteLine(@"      ###&                                                                                        @@@@                                             %##*                                                 ");
            //Console.WriteLine(@"      #%##                                                                                        @@@@                                             ###.                                                 ");
            //Console.WriteLine(@"      ####                                                                                        @@@@                                             ##%                                                  ");
            Console.WriteLine(@"      ,#&#                                                                                        @@@@                                             &##                                                  ");
            Console.WriteLine(@"       ###*                                                                                       @@@@                                             ##&.                                                 ");
            Console.WriteLine(@"       %%@#                                                                                       @@@@                                             %##.                                                 ");
            Console.Write($"        #%@             ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(text.PadLeft(40).PadRight(40));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("                                  @@@@                                             ###*                                                 ");
            Console.WriteLine(@"        ###                                                                                       @@@@                                             %##*                                                 ");
            Console.WriteLine(@"        ///                                                                                       @@@@                                            .###                                                  ");
            Console.WriteLine(@"        ##%(                                                                                      @@@@                                            .##%                                                  ");
            Console.WriteLine(@"        ####                                                                                      @@@@                                             %##.                                                 ");
            Console.WriteLine(@"        ####                                                                                      @@@@                                             ##%#                                                 ");
            Console.WriteLine(@"        *###                             @((((((                                                  @@@@                                             ,%##                                                 ");
            Console.WriteLine(@"        ,###                   &&@@&   &((((((((((%%.                                             @@@@                                              ###                                                 ");
            Console.WriteLine(@"         ###,             ,%((((((((((((%(((((@(((((((((/                                         @@@@                                              #&#*                                                ");
            Console.WriteLine(@"         ####           %((((((((((@#(((((((@(((((((((((((#                                       @@@@                                              #&#*                                                ");
            Console.WriteLine(@"         ,##%          #((((((((((((((((((((((@(((((@/(&@@@&(@                                    @@@@                                              ###*                  *# . (/(,,                    ");
            Console.WriteLine(@"          ###,       %%((((((((((((((((((@&&%&((((@(@&%((((&/(%                                   @@@@                                              ###*     /%(((((((&(((((((((((((((((((.             ");
            Console.WriteLine(@"          ###,      &(((((((((((((((&%(@@@@@%#(((((((((((((((&((/                                 @@@@                                              ##%/ (((((((((((((((((((((((((((((((((((%           ");
            Console.WriteLine(@"          ###(     ((((((((@((((((@(@@@/@(((((((((((((((((((((((((&(&                             @@@@                                              ###((((((((((((((((((((((((((((((((((((((((/        ");
            Console.WriteLine(@"          &##,     %((((((((((((#&&@(#&((&(@(((((((((((((((((((#@#(/#                             @@@@                                            ./((((((((((((((((((((((((((((((((((((((((((((%       ");
            Console.WriteLine(@"         *###      %(((((((((%&(@(((@((&@&((((((((((((((((%#((((@   .                             @@@@                                          //(((((((((((((((((((((((#((%@@((/(((((((((((((((,      ");
            Console.WriteLine(@"         *#(#      &((((((((((((&(((((@((((((((((((#%(((#(  *  @&@@@@/                            @@@@                                        ,((((((((&#/#@@@@((&(((((((((((/(/%@(((((%##/((((((&(((((&");
            Console.WriteLine(@"          %##,     ((((((((((((((((#@((((((((((&((#&, (  &&@@@@@@@@@/(                            @@@@                                       %(((((((((&((((((@(((@#(/&(/@@@&/(((((((@(@#(#((/(((/((((((");
            Console.WriteLine(@"          .###     ,((((((((((((%#&%((((((/%(((@(   (#@@@@@@@@@@@@&(((.                           @@@@                                      &((((((((((((/(((#((%((@((((((((((/(@(((((#(((((&(((((((((((");
            Console.WriteLine(@"           &##(     ((((((((((&(%#(&@@%(((@&    @@@@@@@@@@@@@@@@&(((((                            @@@@                                     #((((((((/((((#@@((#((%((((((((((((((((@((((@((((((#(((((((((");
            Console.WriteLine(@"           ,###     %((((((((((##((%%% &   *%@@@@@@@@@@@@@@@@&((((@((,                            @@@@                                    #(((((((((((((((&@((/(((%(%((((((((((((((%((((((((((((((%#&%((");
            Console.WriteLine(@"            #&#(     ((((((((((((((#((%%&@@@@@@@@@@@@@@@&((((((#@(((&                             @@@@                                    ((((((((((((((/(&@&((&((((@(%(((((((((((((%(((((((((((((((((((");
            Console.WriteLine(@"            .####     %((((((((((((((%((((((/@#(((((((((((((&/(((((%                              @@@@                                   #(((((((((((((((&#@@@(#((((((#((((((##@%#%(((((((((((((((((((((");
            Console.WriteLine(@"              ###,     &(((((((((((((((((((((((((((((((((((((((((((                               @@@@                                  %((((((((((((((((%(&@&&(/((((((((#&&%&&(((((((((((((#((((((((%&@");
            Console.WriteLine(@"              * &##       &((((((((((((((((((((((((((((((((((((((((.                              @@@@                                 *(((((((((((((((((%(&@@/*((((((((((((((((((((((((((((((#(((((((((");
            Console.WriteLine(@"               ####        %(((((((((((((((((((((((((((((((((((((/                                @@@@                                 #(((((((((((((((((((/@@&  %(/((((((((((((((((((((((((((((((((((((");
            Console.WriteLine(@"                ####    .*   ,((((((((((((%%(((((((((((((((((((#%                                 @@@@                                *((((((((((((((((((%((&@@@&  ((&((((((((((((((((((((((&(((((((((((");
            Console.WriteLine(@"                 #&%#( /##%####*(/((((((((((%(((((((((((((((((@(/                                 @@@@                                #(((((((((((((((((((((/@@@@@ .,#(((((((((((((((((((((((@((((((((((");
            Console.WriteLine(@"                   (####%%#####%%#(((((((((((&(((((((((((((((#(#####&%,,                          @@@@                               (((((((((((((((((((((##(/@@@@@&   %(%(((((((((((((((((((((#&@&&(.  ");
            Console.WriteLine(@"                      .#@#&##&##%%#####((((((((#(((((((((((%(########%%%%%%%%%%%%%/               @@@@                               %(((((((((((((((((((((/(((@@@@@@@#&  &##(((((((((((((((((((((      ");
            Console.WriteLine(@"                       /(,/######%%####&###@((((&(((((((###########*%%%%%%%%%%%%%%%%@             @@@@                               #(((((((((((((((((((((((((((@@@@@@@@, %.@(%(((((((((((((((((&      ");
            Console.WriteLine(@"                             %%%%%%%%###%%##%###%#&##&#######%%#*...&%%%%%%%%%%%%%%%%%(           @@@@                                ((((((((((((((((((((((((%(((&@@@@@@@@@#  %.#((%(((((((((((#       ");
            Console.WriteLine(@"                        %#%%%%%%%%%%%%% ,(%######%##%###%###% ..... %%%%%%%%%%%%%%%%%%%           @@@@                                 (((((((((((((((((((((((((%(((&@@@@@@@@@@@@%%.*%(#((%%&%%(&       ");
            Console.WriteLine(@"                   %%%%%%%%%%%%%%%%%%%%%%...........&##########&...@/%%%%%%%%%%%%%%%%%%           @@@@                                 %((((((((((((((((((((((((((%(((%@@@@@@@@@@@@@@@@@%&/%#**.        ");
            Console.WriteLine(@"             ,%%%%%%%%%%%%%%%%%%%%%%%%%%%%%&.......&############% %./%%%%%%%%%%%%%%%%%%           @@@@                                ,##%##(((((((((((((((((((((((((/&(#(&@@@@@@@@@@@@@@(              ");
            Console.WriteLine(@"          &%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%.. *.%###########....&%%%%%%%%%%%%%%%%%%           @@@@                                &%###&&%##(((((((#@#(((((((((((((((/&#((#&@&@@@@@&/               ");
            Console.WriteLine(@"        #%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%.....##%##%(#@...%%%%%%%%%%%%%%%%%%%%           @@@@                            *%%%%%%%########((((((((((((&@#/((((#((((((((&((((#(&%%%%%&           ");
            Console.WriteLine(@"      .%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%,...*##&####,&%%%%%%%%%%%%%%%%%%%&           @@@@                          #%%%%%%%%%%%%###########((((((((((#(###&###%###%%%%%%%%%%%%%#%@         ");
            Console.WriteLine(@"      @%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#&#######&%%%%%%%%%%%%%%%%%%%@%           @@@@                       %%%%%%%%%%%%%%%%%%%*###############&###%####&####%%%%%%%%%%%%%%%%%%.       ");
            Console.WriteLine(@"     (%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#&###%%%%%%%%%%%%%%%%%%%%%@           @@@@                    %%%%%%%%%%%%%%%%%%%%%%%&..@.#############&..%#%%%%%%%%%%%%%%%%%%%%%%%%%       ");
            Console.WriteLine(@"     %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%            @@@@                 ,%%%%%%%%%%%%%%%%%%%%%%%%%%% ...%##########&.@. #%%%%%%%%%%%%%%%%%%%%%%%%%%      ");
            Console.WriteLine(@"    &%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%&%%            @@@@               (%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%(,...####&(#(.....%%%%%%%%%%%%%%%%%%%%%%%%%%%%      ");
            Console.WriteLine(@"   (%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#%%&            @@@@            @%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%/ .#######/..&%%%%%%%%%%%%%%%%%%%%%%%%%%%%%*     ");
            Console.WriteLine(@"   %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#%%%&            @@@@           &%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%@########&%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%/     ");
            Console.WriteLine(@"  %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#%%%%%%%%#%%%%@            @@@@         .%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%##(%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#      ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%&/((((@%%%%#%%%%#             @@@@         &%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%&      ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#%&/@((&%#&((((((((%%&((@(%%%%%&             @@@@        .%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%       ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%%#%%%%%%%%%%%%%%%%%%%%%%%%%%#(((((((%&((((((((@&/((((((/%%%%%%%#             @@@@        *%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%        ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%%&%%%%%%%%%%%%%%%%%%%%%%#(((((((@%((((((((@(@(((((((&#&&%%%%%%%              @@@@        %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#         ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%#%%%%%%%%%%%%%%%%%%%%##(((((%((((((((((&((((((((#@#(((##%%%%%%@              @@@@        &%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%          ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%#%%%%%%%%%%%%%%%%%%%%#(((((((((((((((((((((((@&(((((/@&%%%%%%%%               @@@@        &%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%           ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%&..#((((((((((((((((((((&(((((/@&(((/&#%%%%%%&*              @@@@        @%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%(           ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%%%%%%%%&&%%%%%...%((((((((((((((((((((((((((@%((((&%%%%%%%%%%(               @@@@        *%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%&            ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%%&%%%%%%%%%%%#....(((((((((((((((((((((((((((((&%%%%%%%%%%%%%                @@@@        *%%%%%%%%%%%%%%%%%%%%%%%%&%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%             ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%&&%%%%%%%%%%%%%%%%%....#(((((((((((((((((((((((((%%%%%%%%%%%%%%%%&                @@@@         %%%%%%%%%%%%%%%%%%%%%%%%&%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#              ");
            Console.WriteLine(@" %%%%%%%%%#%%%%%%%%%%%%%%%%%%%%%%%%%....&((((((((((((((((((((#%%%%%%%%%%%%%%%%%%%&                @@@@         &%%%%%%%%%%%%%%%%%%%%%%%#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%(              ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%....(((((((((((%&%%%%%%%%%%%%%%%%%%%%%%%%%%%,                @@@@         &%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%               ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%/...####%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%@                 @@@@         &%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%&               ");
            Console.WriteLine(@" %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%/.  *&%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#                  @@@@         *%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%                ");
            Console.Write(@" %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%                   @@@@          #%%%%%%%%%%%%%%%%%%%%%%&%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%&                ");
        }
    }
}






