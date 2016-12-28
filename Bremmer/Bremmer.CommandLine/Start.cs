using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bremmer;

namespace Bremmer.CommandLine
{
    public class Start
    {
        public static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                string source = args[0];
                try
                {
                    SiteBuilder builder = new SiteBuilder(source);
                    builder.Processed += Builder_Processed;

                    System.Console.WriteLine("Site build started...");
                    builder.Build();
                    System.Console.ReadKey();

                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    System.Console.ReadKey();
                }

            }
        }

        private static void Builder_Processed(object sender, EventArgs e)
        {
            System.Console.WriteLine(((ProcessEventArgs)e).Description);
        }
    }
}
