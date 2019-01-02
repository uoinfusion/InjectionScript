using System;
using System.Globalization;

namespace InjectionScript.Runtime
{
    public static class InternalSubrutines
    {
        public static InjectionValue Val(string str)
        {
            if (int.TryParse(str, out var integer))
                return new InjectionValue(integer);
            else if (double.TryParse(str, out var dec))
                return new InjectionValue(dec);

            return InjectionValue.Zero;
        }

        public static string Str(int value) => value.ToString(CultureInfo.InvariantCulture);
        public static string Str(double value) => value.ToString(CultureInfo.InvariantCulture);
        public static string Str(string value) => "0";

        public static int Len(string value) => value.Length;
        public static int Len(int value) => 0;
        public static int Len(double value) => 0;

        public static InjectionValue GetArrayLength(InjectionValue arg)
        {
            if (arg.Kind == InjectionValueKind.Array)
                return new InjectionValue(arg.Array.Length);

            return InjectionValue.Zero;
        }

        public static string Left(string str, int length)
        {
            length = str.Length < length ? str.Length : length;

            return str.Substring(0, length);
        }

        public static string Right(string str, int length)
        {
            length = str.Length < length ? str.Length : length;

            return str.Substring(str.Length - length, length);
        }

        public static string Mid(string str, int start, int length)
        {
            if (start > str.Length)
                return "";

            if (start < 0)
                return "";

            var end = start + length;
            if (end > str.Length)
                length = str.Length - start;

            return str.Substring(start, length);
        }
    }
}
