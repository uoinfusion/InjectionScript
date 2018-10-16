using System;
using System.Collections.Generic;

namespace InjectionScript.Interpretation
{
    public struct InjectionValue
    {
        public static InjectionValue Unit { get; } = new InjectionValue(InjectionValueKind.Unit);
        public static InjectionValue True { get; } = new InjectionValue(1);
        public static InjectionValue False { get; } = new InjectionValue(0);
        public static InjectionValue MinusOne { get; } = new InjectionValue(-1);
        public static InjectionValue Zero { get; } = new InjectionValue(0);

        public int Number { get; }
        public string String { get; }
        public InjectionValueKind Kind { get; set; }

        public InjectionValue(int number)
        {
            Number = number;
            String = null;
            Kind = InjectionValueKind.Number;
        }

        public InjectionValue(string str)
        {
            String = str;
            Number = 0;
            Kind = InjectionValueKind.String;
        }

        private InjectionValue(InjectionValueKind kind) : this()
        {
            Number = 0;
            String = null;
            Kind = kind;
        }

        public static InjectionValue operator +(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Number && v2.Kind == InjectionValueKind.Number)
                return new InjectionValue(v1.Number + v2.Number);

            throw new NotImplementedException();
        }

        public static InjectionValue operator -(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Number && v2.Kind == InjectionValueKind.Number)
                return new InjectionValue(v1.Number - v2.Number);

            throw new NotImplementedException();
        }

        public static InjectionValue operator /(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Number && v2.Kind == InjectionValueKind.Number)
                return new InjectionValue(v1.Number / v2.Number);

            throw new NotImplementedException();
        }

        public static InjectionValue operator *(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Number && v2.Kind == InjectionValueKind.Number)
                return new InjectionValue(v1.Number * v2.Number);

            throw new NotImplementedException();
        }

        public static InjectionValue operator &(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Number && v2.Kind == InjectionValueKind.Number)
                return new InjectionValue(v1.Number != 0 && v2.Number != 0 ? 1 : 0);

            throw new NotImplementedException();
        }

        public static InjectionValue operator |(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Number && v2.Kind == InjectionValueKind.Number)
                return new InjectionValue(v1.Number != 0 || v2.Number != 0 ? 1 : 0);

            throw new NotImplementedException();
        }

        public static bool operator ==(InjectionValue v1, InjectionValue v2)
        {
            return v1.Kind == v2.Kind && v1.Number == v2.Number && v1.String == v2.String;
        }

        public static bool operator !=(InjectionValue v1, InjectionValue v2)
        {
            return v1.Kind != v2.Kind || v1.Number != v2.Number || v1.String != v2.String;
        }

        public static bool operator >(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Number && v2.Kind == InjectionValueKind.Number)
                return v1.Number > v2.Number;

            throw new NotImplementedException();
        }

        public static bool operator >=(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Number && v2.Kind == InjectionValueKind.Number)
                return v1.Number >= v2.Number;

            throw new NotImplementedException();
        }

        public static bool operator <(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Number && v2.Kind == InjectionValueKind.Number)
                return v1.Number < v2.Number;

            throw new NotImplementedException();
        }

        public static bool operator <=(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Number && v2.Kind == InjectionValueKind.Number)
                return v1.Number <= v2.Number;

            throw new NotImplementedException();
        }

        public static explicit operator int(InjectionValue v1)
        {
            if (v1.Kind != InjectionValueKind.Number)
                throw new NotImplementedException();

            return v1.Number;
        }

        public override string ToString()
        {
            switch (Kind)
            {
                case InjectionValueKind.Unit:
                    return "<unit>";
                case InjectionValueKind.Number:
                    return Number.ToString();
                case InjectionValueKind.String:
                    return String;
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is InjectionValue))
            {
                return false;
            }

            var value = (InjectionValue)obj;
            return Number == value.Number &&
                   String == value.String &&
                   Kind == value.Kind;
        }

        public override int GetHashCode()
        {
            var hashCode = 596824379;
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(String);
            hashCode = hashCode * -1521134295 + Kind.GetHashCode();
            return hashCode;
        }
    }
}
