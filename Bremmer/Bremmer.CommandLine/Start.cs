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
                SiteBuilder builder = new SiteBuilder(source);
                builder.Build();
            }
        }
    }
}
