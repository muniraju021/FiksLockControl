using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockServices.Lib.Utilities
{
    public class CustomDateConverter : IsoDateTimeConverter
    {
        public CustomDateConverter()
        {
            DateTimeFormat = "dd-MM-yyyy";
        }
    }

    public class USDateConverter : IsoDateTimeConverter
    {
        public USDateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }

}
