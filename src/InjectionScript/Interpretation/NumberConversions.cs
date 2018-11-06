using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
{
    public static class NumberConversions
    {
        public static int Str2Int(string str)
        {
            if (str.StartsWith("0x"))
            {
                str = str.Substring(2);
                return int.Parse(str, NumberStyles.HexNumber);
            }

            return int.Parse(str, NumberStyles.Integer);
        }

        public static string Int2Hex(int id) => $"0x{id:X8}";
    }
}
