using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Debugging
{
    public struct SourceCodeLocation
    {
        public string FileName { get; }
        public int Line { get; set; }

        public SourceCodeLocation(string fileName, int line)
        {
            FileName = fileName;
            Line = line;
        }

        public override string ToString() => $"line: {Line}, {FileName}";

        public override bool Equals(object obj)
        {
            if (!(obj is SourceCodeLocation))
            {
                return false;
            }

            var location = (SourceCodeLocation)obj;
            return FileName == location.FileName &&
                   Line == location.Line;
        }

        public override int GetHashCode()
        {
            var hashCode = -843199729;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileName);
            hashCode = hashCode * -1521134295 + Line.GetHashCode();
            return hashCode;
        }
    }
}
