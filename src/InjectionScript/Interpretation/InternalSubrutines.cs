using System.Globalization;

namespace InjectionScript.Interpretation
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
    }
}
