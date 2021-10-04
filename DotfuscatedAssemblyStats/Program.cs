using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotfuscatedAssemblyStats
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("hello world");
            XDocument doc = XDocument.Parse(System.IO.File.ReadAllText(args[0]));
           
            List<InputAssembly> inputAssemblies = doc.Descendants("module").Select(item =>
            {
                return new InputAssembly()
                {
                    Name = item.Element("name").Value,
                    Types = new InputAssemblyStatistics()
                    {
                        Count = item.Descendants("type").Count(),
                        Renamed = item.Descendants("type").Elements("newname").Count(),
                    },
                    Methods = new InputAssemblyStatistics() 
                    {
                        Count = item.Descendants("method").Count(),
                        Renamed = item.Descendants("method").Elements("newname").Count(),
                    },
                    Fields = new InputAssemblyStatistics()
                    {
                        Count = item.Descendants("field").Count(),
                        Renamed = item.Descendants("field").Elements("newname").Count(),
                    }
                };
            }).ToList();


            InputAssembly allAssemblyTotals = new InputAssembly()
            {
                Name = "ALL ASSEMBLY TOTALS",
                Types = new InputAssemblyStatistics(),
                Methods = new InputAssemblyStatistics(),
                Fields = new InputAssemblyStatistics()
            };

            foreach (var inputAssembly in inputAssemblies)
            {
                allAssemblyTotals.Types.Count += inputAssembly.Types.Count;
                allAssemblyTotals.Types.Renamed += inputAssembly.Types.Renamed;
                allAssemblyTotals.Methods.Count += inputAssembly.Methods.Count;
                allAssemblyTotals.Methods.Renamed += inputAssembly.Methods.Renamed;
                allAssemblyTotals.Fields.Count += inputAssembly.Fields.Count;
                allAssemblyTotals.Fields.Renamed += inputAssembly.Fields.Renamed;
                CalculatePercentages(inputAssembly);
            }

            CalculatePercentages(allAssemblyTotals);
            Console.WriteLine(allAssemblyTotals.Name);
            Console.WriteLine(String.Format("\tTypes Renamed: {0} of {1} ({2:F2}%)", allAssemblyTotals.Types.Renamed, allAssemblyTotals.Types.Count, allAssemblyTotals.Types.RenamedPercent));
            Console.WriteLine(String.Format("\tMethods Renamed: {0} of {1} ({2:F2}%)", allAssemblyTotals.Methods.Renamed, allAssemblyTotals.Methods.Count, allAssemblyTotals.Methods.RenamedPercent));
            Console.WriteLine(String.Format("\tFields Renamed: {0} of {1} ({2:F2}%)", allAssemblyTotals.Fields.Renamed, allAssemblyTotals.Fields.Count, allAssemblyTotals.Fields.RenamedPercent));

            foreach (var inputAssembly in inputAssemblies)
            {
                Console.WriteLine(inputAssembly.Name);
                Console.WriteLine(String.Format("\tTypes Renamed: {0} of {1} ({2:F2}%)", inputAssembly.Types.Renamed, inputAssembly.Types.Count, inputAssembly.Types.RenamedPercent));
                Console.WriteLine(String.Format("\tMethods Renamed: {0} of {1} ({2:F2}%)", inputAssembly.Methods.Renamed, inputAssembly.Methods.Count, inputAssembly.Methods.RenamedPercent));
                Console.WriteLine(String.Format("\tFields Renamed: {0} of {1} ({2:F2}%)", inputAssembly.Fields.Renamed, inputAssembly.Fields.Count, inputAssembly.Fields.RenamedPercent));
            }

            Console.ReadKey();

        }

        static private void CalculatePercentages(InputAssembly inputAssembly)
        {
            if (inputAssembly.Types.Count > 0)
                inputAssembly.Types.RenamedPercent = ((double)inputAssembly.Types.Renamed / inputAssembly.Types.Count) * 100;
            if (inputAssembly.Methods.Count > 0)
                inputAssembly.Methods.RenamedPercent = ((double)inputAssembly.Methods.Renamed / inputAssembly.Methods.Count) * 100;
            if (inputAssembly.Fields.Count > 0)
                inputAssembly.Fields.RenamedPercent = ((double)inputAssembly.Fields.Renamed / inputAssembly.Fields.Count) * 100;
        }

        class InputAssembly
        {
            public string Name { get; set; }
            public InputAssemblyStatistics Types { get; set; }
            public InputAssemblyStatistics Methods { get; set; }
            public InputAssemblyStatistics Fields { get; set; }
            //public InputAssemblyStatistics Properties { get; set; }
        }

        class InputAssemblyStatistics
        {
            public int Count { get; set; }
            public int Renamed { get; set; }
            public double RenamedPercent { get; set; }
        }


    }

}

