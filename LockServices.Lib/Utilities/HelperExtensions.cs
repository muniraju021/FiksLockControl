using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LockServices.Lib.Utilities
{
    public static class HelperExtensions
    {
        public static List<string> SplitToCsv(this string str)
        {
            var lst = Regex.Split(str, "(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)").ToList();
            lst.RemoveAll(i => i.Length == 0);
            lst = lst.Select(i => i.Replace("\"", string.Empty)).ToList();
            return lst;
        }
    }
}
