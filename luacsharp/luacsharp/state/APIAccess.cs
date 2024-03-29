using System;
using luacsharp.API;
using LuaType = System.Int32;

namespace luacsharp.state
{
    public partial class LuaState
    {
        public string TypeName(LuaType tp)
        {
            switch (tp)
            {
                case Consts.LUA_TNONE: return "no value";
                case Consts.LUA_TNIL: return "nil";
                case Consts.LUA_TBOOLEAN: return "boolean";
                case Consts.LUA_TNUMBER: return "number";
                case Consts.LUA_TSTRING: return "string";
                case Consts.LUA_TTABLE: return "table";
                case Consts.LUA_TFUNCTION: return "function";
                case Consts.LUA_TTHREAD: return "thread";
                default: return "userdata";
            }
        }

        public LuaType Type(int idx)
        {
            if (!stack.isValid(idx))
            {
                return Consts.LUA_TNONE;
            }

            var val = stack.get(idx);
            return LuaValue.typeOf(val);
        }

        public bool IsNone(int idx)
        {
            return Type(idx) == Consts.LUA_TNONE;
        }

        public bool IsNil(int idx)
        {
            return Type(idx) == Consts.LUA_TNIL;
        }

        public bool IsNoneOrNil(int idx)
        {
            return Type(idx) <= Consts.LUA_TNIL;
        }

        public bool IsBoolean(int idx)
        {
            return Type(idx) == Consts.LUA_TBOOLEAN;
        }

        public bool IsString(int idx)
        {
            var t = Type(idx);
            return t == Consts.LUA_TSTRING || t == Consts.LUA_TNUMBER;
        }

        public bool IsNumber(int idx)
        {
            return ToNumberX(idx).Item2;
        }

        public bool IsInteger(int idx)
        {
            var val = stack.get(idx);
            return val.GetType().Name.Equals("Int64");
        }

        public bool IsCsharpFunction(int idx)
        {
            var val = stack.get(idx);
            if (val.GetType().IsEquivalentTo(typeof(Closure)))
            {
                var c = (Closure) val;
                return c.csharpFunc != null;
            }

            return false;
        }

        public bool ToBoolean(int idx)
        {
            var val = stack.get(idx);
            return ConvertToBoolean(val);
        }


        private static bool ConvertToBoolean(object val)
        {
            if (val == null)
            {
                return false;
            }

            switch (val.GetType().Name)
            {
                case "Boolean": return (bool) val;
                default: return true;
            }
        }

        public double ToNumber(int idx)
        {
            return ToNumberX(idx).Item1;
        }

        public (double, bool) ToNumberX(int idx)
        {
            var val = stack.get(idx);
            return LuaValue.convertToFloat(val);
        }

        public (long, bool) ToIntegerX(int idx)
        {
            var val = stack.get(idx);
            return LuaValue.convertToInteger(val);
        }

        public long ToInteger(int idx)
        {
            var val = ToIntegerX(idx);
            return val.Item1;
        }

        public string ToString(int idx)
        {
            return ToStringX(idx).Item1;
        }

        public (string, bool) ToStringX(int idx)
        {
            var val = stack.get(idx);
            switch (val.GetType().Name)
            {
                case "String": return (LuaValue.toString(val), true);
                case "Int64":
                case "Double":
                    var s = val;
                    stack.set(idx, s);
                    return (Convert.ToString(s), true);
                default: return ("", false);
            }
        }

        public CsharpFunction ToCsharpFunction(int idx)
        {
            var val = stack.get(idx);
            if (val.GetType().IsEquivalentTo(typeof(Closure)))
            {
                var c = (Closure) val;
                return c.csharpFunc;
            }

            return null;
        }
    }
}