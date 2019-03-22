using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public static class NumberConversions
    {
        public static int ToInt(string str)
        {
            if (str.StartsWith("0x"))
            {
                str = str.Substring(2);
                return int.Parse(str, NumberStyles.HexNumber);
            }

            return int.Parse(str, NumberStyles.Integer);
        }

        public static int ToInt(InjectionValue value)
        {
            switch (value.Kind)
            {
                case InjectionValueKind.Integer:
                    return value.Integer;
                case InjectionValueKind.String:
                    return ToInt(value.String);
                case InjectionValueKind.Array:
                    return 0;
                case InjectionValueKind.Decimal:
                    return (int)value.Decimal;
                default:
                    throw new NotImplementedException($"InjectionValueKind {value.Kind}");
            }
        }

        public static bool TryStr2Int(string str, out int value)
        {
            if (str.StartsWith("0x"))
            {
                str = str.Substring(2);
                return int.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
            }

            return int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        public static string ToHex(int id) => $"0x{id:X8}";
        public static string ToHex(short id) => $"0x{id:X4}";
        public static string Int2Hex(uint id) => ToHex((int)id);
    }
}
