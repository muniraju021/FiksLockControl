﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleAppln
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                var msg = "+CMT: \"+919739402306\",\"\",\"20/09/05,18:13:54+22\"";
                var lst = Regex.Split(msg, "(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)").ToList();
                lst.RemoveAll(i => i.Length == 0);
                lst = lst.Select(i => i.Replace("\"", string.Empty)).ToList();

                var firstMsg = lst.FirstOrDefault();
                if (firstMsg.IndexOf("+91") != -1)
                {
                    var index = firstMsg.IndexOf("+91") + 3;
                    var senderPhNo = firstMsg.Substring(index);
                }

                CultureInfo enUS = new CultureInfo("en-US");
                DateTime msgSentTime;
                DateTime.TryParseExact(lst[2].Replace("\"",string.Empty),"yy/MM/dd,HH:mm:ss+ff",CultureInfo.InvariantCulture,DateTimeStyles.None,out msgSentTime);

                Console.WriteLine($"ReceivedSmsMessage: ProcessActualMessages - Message:[]");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
    }
}
