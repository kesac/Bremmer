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

                    System.Console.WriteLine("Site build started...");
                    builder.Build();
                    System.Console.WriteLine("Site build completed: " + builder.Target.FullName);

                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }
                
            }
        }
    }
}
