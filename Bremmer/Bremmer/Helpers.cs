using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bremmer
{
    public static class Helpers
    {
        public static string RemoveExtension(this string s, string extension)
        {
            if (s.ToLower().EndsWith(extension.ToLower()))
            {
                return s.Substring(0, s.Length - extension.Length);
            }
            else
            {
                return s;
            }
        }
    }
}
