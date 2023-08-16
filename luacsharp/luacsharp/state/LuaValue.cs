using System;
using luacsharp.API;
using LuaType = System.Int32;

namespace luacsharp.state
{
    public class LuaValue
    {
        internal readonly object value;

        public LuaValue(object value)
        {
            this.value = value;
        }
        
        public LuaTable toLuaTable()
        {
            return (LuaTable) value;
        }

        public string toString()
        {
            return Convert.ToString(value);
        }

        public long toInteger()
        {
            return Convert.ToInt64(value);
        }

        public double toFloat()
        {
            return Convert.ToDouble(value);
        }

        public bool isString()
        {
            return value.GetType().IsEquivalentTo(typeof(string));
        }

        public bool isLuaTable()
        {
            return value.GetType().IsEquivalentTo(typeof(LuaTable));
        }

        public bool isFloat()
        {
            return value.GetType().IsEquivalentTo(typeof(double));
        }

        public bool isInteger()
        {
            return value.GetType().IsEquivalentTo(typeof(long));
        }
        
        internal static LuaType typeOf(object val)
        {
            if (val == null)
            {
                return Consts.LUA_TNIL;
            }
            
            switch (val.GetType().Name)
            {
                case "Boolean": return Consts.LUA_TBOOLEAN;
                case "Double": return Consts.LUA_TNUMBER;
                case "Int64": return Consts.LUA_TNUMBER;
                case "String": return Consts.LUA_TSTRING;
                default: throw new Exception("todo!");
            }
        }
        
        internal static Tuple<double, bool> convertToFloat(object val)
        {
            switch (val.GetType().Name)
            {
                case "Double": return Tuple.Create((double) val, true);
                case "Int64": return Tuple.Create(Convert.ToDouble(val), true);
                case "String": return number.Parser.ParseFloat((string) val);
                default: return Tuple.Create(0d, false);
            }
        }

        internal static Tuple<long, bool> convertToInteger(object val)
        {
            switch (val.GetType().Name)
            {
                case "Int64": return Tuple.Create<long, bool>((long) val, true);
                case "Double": return number.Math.FloatToInteger((double) val);
                case "String": return Tuple.Create(Convert.ToInt64(val), true);
                default: return Tuple.Create(0L, false);
            }
        }
        
        private Tuple<long, bool> _stringToInteger(string s)
        {
            var v = number.Parser.ParseInteger(s);
            if (v.Item2)
            {
                return Tuple.Create(v.Item1, true);
            }

            var v2 = number.Parser.ParseFloat(s);
            if (v2.Item2)
            {
                return number.Math.FloatToInteger(v2.Item1);
            }

            return Tuple.Create<long, bool>(0L, false);
        }
    }
}