using InjectionScript.Runtime.ObjectTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InjectionScript.Runtime
{
    public struct InjectionValue
    {
        public static InjectionValue Unit { get; } = new InjectionValue(InjectionValueKind.Unit);
        public static InjectionValue True { get; } = new InjectionValue(1);
        public static InjectionValue False { get; } = new InjectionValue(0);
        public static InjectionValue MinusOne { get; } = new InjectionValue(-1);
        public static InjectionValue Zero { get; } = new InjectionValue(0);

        public InjectionObject Object { get; }
        public int Integer { get; }
        public double Decimal { get; }
        public string String { get; }
        public InjectionValue[] Array { get; }
        public InjectionValueKind Kind { get; set; }

        public InjectionValue(object value, Type type)
        {
            if (type == typeof(void))
            {
                Integer = 0;
                Decimal = 0;
                String = null;
                Array = System.Array.Empty<InjectionValue>();
                Object = null;
                Kind = InjectionValueKind.Unit;
            }
            else if (value is string str)
            {
                String = str;
                Integer = 0;
                Decimal = 0;
                Array = System.Array.Empty<InjectionValue>();
                Object = null;
                Kind = InjectionValueKind.String;
            }
            else if (value is int i)
            {
                Integer = i;
                String = null;
                Decimal = 0;
                Array = System.Array.Empty<InjectionValue>();
                Object = null;
                Kind = InjectionValueKind.Integer;
            }
            else if (value is double d)
            {
                Integer = 0;
                String = null;
                Decimal = d;
                Array = System.Array.Empty<InjectionValue>();
                Object = null;
                Kind = InjectionValueKind.Decimal;
            }
            else if (value is IEnumerable<InjectionValue> array)
            {
                Integer = 0;
                String = null;
                Decimal = 0;
                Array = array.ToArray();
                Object = null;
                Kind = InjectionValueKind.Array;
            }
            else if (value is InjectionObject obj)
            {
                Integer = 0;
                String = null;
                Decimal = 0;
                Array = System.Array.Empty<InjectionValue>();
                Object = null;
                Kind = InjectionValueKind.Object;
            }
            else
                throw new NotSupportedException($"Unsupported return type {value.GetType()}");
        }

        public InjectionValue(int number)
        {
            Integer = number;
            String = null;
            Decimal = 0;
            Array = System.Array.Empty<InjectionValue>();
            Object = null;
            Kind = InjectionValueKind.Integer;
        }

        internal object ConvertTo(InjectionValueKind targetKind)
        {
            switch (targetKind)
            {
                case InjectionValueKind.Any:
                    return this;
                case InjectionValueKind.Integer:
                    return (int)this;
                case InjectionValueKind.Decimal:
                    return (double)this;
                case InjectionValueKind.String:
                    return (string)this;
                case InjectionValueKind.Array:
                    return (InjectionValue[])this;
                case InjectionValueKind.Object:
                    return (InjectionObject)this;
                default:
                    throw new NotImplementedException($"Conversion from {this.Kind} to {targetKind}.");
            }
        }

        public InjectionValue(string str)
        {
            String = str;
            Integer = 0;
            Decimal = 0;
            Array = System.Array.Empty<InjectionValue>();
            Object = null;
            Kind = InjectionValueKind.String;
        }

        public InjectionValue(double d)
        {
            String = null;
            Integer = 0;
            Decimal = d;
            Array = System.Array.Empty<InjectionValue>();
            Object = null;

            Kind = InjectionValueKind.Decimal;
        }

        public InjectionValue(InjectionValue[] array)
        {
            String = null;
            Integer = 0;
            Decimal = 0;
            Array = array;
            Object = null;

            Kind = InjectionValueKind.Array;
        }

        public InjectionValue(InjectionObject obj)
        {
            String = null;
            Integer = 0;
            Decimal = 0;
            Array = System.Array.Empty<InjectionValue>();
            Object = obj;

            Kind = InjectionValueKind.Object;
        }

        public static InjectionValueKind GetKind(Type type)
        {
            if (type.Equals(typeof(string)))
                return InjectionValueKind.String;
            else if (type.Equals(typeof(int)))
                return InjectionValueKind.Integer;
            else if (type.Equals(typeof(void)))
                return InjectionValueKind.Unit;
            else if (type.Equals(typeof(double)))
                return InjectionValueKind.Decimal;
            else if (typeof(Array).IsAssignableFrom(type))
                return InjectionValueKind.Array;
            else if (typeof(InjectionObject).IsAssignableFrom(type))
                return InjectionValueKind.Object;

            throw new NotSupportedException($"Unsupported type {type.Name}.");
        }

        private InjectionValue(InjectionValueKind kind) : this()
        {
            Integer = 0;
            String = null;
            Kind = kind;
            Decimal = 0;
            Array = System.Array.Empty<InjectionValue>();
            Object = null;
        }

        public static InjectionValue operator +(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Integer + v2.Integer);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Decimal + v2.Integer);
            else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Integer + v2.Decimal);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Decimal + v2.Decimal);
            else if (v1.Kind == InjectionValueKind.String && v2.Kind == InjectionValueKind.String)
                return new InjectionValue(v1.String + v2.String);

            throw new NotImplementedException();
        }

        public static InjectionValue operator -(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Integer - v2.Integer);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Decimal - v2.Integer);
            else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Integer - v2.Decimal);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Decimal - v2.Decimal);

            throw new NotImplementedException();
        }

        public static InjectionValue operator /(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue((double)v1.Integer / v2.Integer);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Decimal / v2.Integer);
            else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Integer / v2.Decimal);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Decimal / v2.Decimal);

            throw new NotImplementedException();
        }

        public static InjectionValue operator *(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Integer * v2.Integer);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Decimal * v2.Integer);
            else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Integer * v2.Decimal);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Decimal * v2.Decimal);

            throw new NotImplementedException();
        }

        public static InjectionValue operator &(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Integer != 0 && v2.Integer != 0 ? 1 : 0);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Decimal != 0 && v2.Integer != 0 ? 1 : 0);
            else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Integer != 0 && v2.Decimal != 0 ? 1 : 0);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Decimal != 0 && v2.Decimal != 0 ? 1 : 0);

            throw new NotImplementedException();
        }

        public static InjectionValue operator |(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Integer != 0 || v2.Integer != 0 ? 1 : 0);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                return new InjectionValue(v1.Decimal != 0 || v2.Integer != 0 ? 1 : 0);
            else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Integer != 0 || v2.Decimal != 0 ? 1 : 0);
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                return new InjectionValue(v1.Decimal != 0 || v2.Decimal != 0 ? 1 : 0);

            throw new NotImplementedException();
        }

        public static bool operator ==(InjectionValue v1, InjectionValue v2) => v1.Equals(v2);
        public static bool operator !=(InjectionValue v1, InjectionValue v2) => !v1.Equals(v2);

        public static bool operator >(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Integer)
                return v1.Integer > v2.Integer;
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                return v1.Decimal > v2.Integer;
            else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                return v1.Integer > v2.Decimal;
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                return v1.Integer > v2.Integer;

            throw new NotImplementedException();
        }

        public static bool operator >=(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Integer)
                return v1.Integer >= v2.Integer;
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                return v1.Decimal >= v2.Integer;
            else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                return v1.Integer >= v2.Decimal;
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                return v1.Integer >= v2.Integer;

            throw new NotImplementedException();
        }

        public static bool operator <(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Integer)
                return v1.Integer < v2.Integer;
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                return v1.Decimal < v2.Integer;
            else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                return v1.Integer < v2.Decimal;
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                return v1.Integer < v2.Integer;

            throw new NotImplementedException();
        }

        public static bool operator <=(InjectionValue v1, InjectionValue v2)
        {
            if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Integer)
                return v1.Integer <= v2.Integer;
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                return v1.Decimal <= v2.Integer;
            else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                return v1.Integer <= v2.Decimal;
            else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                return v1.Integer <= v2.Integer;

            throw new NotImplementedException();
        }

        public static explicit operator int(InjectionValue v1)
        {
            if (v1.Kind != InjectionValueKind.Integer)
                throw new NotImplementedException();

            return v1.Integer;
        }

        public static explicit operator double(InjectionValue v1)
        {
            if (v1.Kind != InjectionValueKind.Decimal)
                throw new NotImplementedException();

            return v1.Decimal;
        }

        public static explicit operator string(InjectionValue v1)
        {
            if (v1.Kind != InjectionValueKind.String)
                throw new NotImplementedException();

            return v1.String;
        }

        public static explicit operator InjectionValue[](InjectionValue v1)
        {
            if (v1.Kind != InjectionValueKind.Array)
                throw new NotImplementedException();

            return v1.Array;
        }

        public static explicit operator InjectionObject(InjectionValue v1)
        {
            if (v1.Kind != InjectionValueKind.Object)
                throw new NotImplementedException();

            return v1.Object;
        }

        public object ToValue()
        {
            switch (Kind)
            {
                case InjectionValueKind.Integer:
                    return Integer;
                case InjectionValueKind.String:
                    return String;
                case InjectionValueKind.Decimal:
                    return Decimal;
                case InjectionValueKind.Unit:
                    return Unit;
                case InjectionValueKind.Array:
                    return this.Array;
                case InjectionValueKind.Object:
                    return this.Object;
                default:
                    throw new NotImplementedException(Kind.ToString());
            }
        }

        public override string ToString()
        {
            switch (Kind)
            {
                case InjectionValueKind.Unit:
                    return "<unit>";
                case InjectionValueKind.Integer:
                    return Integer.ToString();
                case InjectionValueKind.String:
                    return String;
                case InjectionValueKind.Decimal:
                    return Decimal.ToString();
                case InjectionValueKind.Array:
                    return Array.ToString();
                case InjectionValueKind.Object:
                    return Object.Name;
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is InjectionValue v2)
            {
                var v1 = this;

                if (v1.Kind == v2.Kind && v1.Integer == v2.Integer && v1.String == v2.String && v1.Decimal == v2.Decimal)
                    return true;
                else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Integer)
                    return v1.Decimal == v2.Integer;
                else if (v1.Kind == InjectionValueKind.Integer && v2.Kind == InjectionValueKind.Decimal)
                    return v1.Integer == v2.Decimal;
                else if (v1.Kind == InjectionValueKind.Decimal && v2.Kind == InjectionValueKind.Decimal)
                    return v1.Decimal == v2.Decimal;
            }
            else if (obj is int i)
            {
                return Kind == InjectionValueKind.Integer && Integer == i;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 596824379;
            hashCode = hashCode * -1521134295 + Integer.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(String);
            hashCode = hashCode * -1521134295 + Kind.GetHashCode();
            hashCode = hashCode * -1521134295 + Decimal.GetHashCode();
            hashCode = hashCode * -1521134295 + Array.GetHashCode();
            hashCode = hashCode * -1521134295 + Object.GetHashCode();
            return hashCode;
        }

        public static bool IsSupported(Type type)
        {
            return type == typeof(string) || type == typeof(int) || type == typeof(void) || type == typeof(double)
                || type == typeof(InjectionValue) || typeof(IEnumerable<InjectionValue>).IsAssignableFrom(type)
                || type == typeof(InjectionObject);
        }
    }
}
