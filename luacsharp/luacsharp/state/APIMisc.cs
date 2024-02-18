using System;

namespace luacsharp.state
{
    public partial class LuaState
    {
        public void Len(int idx)
        {
            var val = stack.get(idx);
            if (LuaValue.isString(val))
            {
                var s = LuaValue.toString(val);
                stack.push((long) s.Length);
            }

            var (result, ok) = callMetamethod(val, val, "__len", this);
            if (ok)
            {
                stack.push(result);
            }
            
            if (LuaValue.isLuaTable(val))
            {
                var t = LuaValue.toLuaTable(val);
                stack.push((long) t.len());
            }
            else
            {
                throw new Exception("length error!");
            }
        }

        public void Concat(int n)
        {
            if (n == 0)
            {
                stack.push("");
            }
            else if (n >= 2)
            {
                for (var i = 1; i < n; i++)
                {
                    if (IsString(-1) && IsString(-2))
                    {
                        var s2 = ToString(-1);
                        var s1 = ToString(-2);
                        stack.pop();
                        stack.pop();
                        stack.push(s1 + s2);
                        continue;
                    }

                    var b = stack.pop();
                    var a = stack.pop();
                    var (result, ok) = callMetamethod(a, b, "__concat", this);
                    if (ok)
                    {
                        stack.push(result);
                        continue;
                    }

                    throw new Exception("concatenation error!");
                }

                // n==1, do nothing
            }
        }
        
        public uint RawLen(int idx)
        {
            var val = stack.get(idx);
            if (val is string valStr)
            {
                return (uint) valStr.Length;
            }

            if (val is LuaTable luaTable)
            {
                return (uint) (luaTable).len();
            }

            return 0;
        }
        
        public bool Next(int idx)
        {
            var val = stack.get(idx);
            if (!(val is LuaTable t))
            {
                throw new Exception("table expected!");
            }
            var key = stack.pop();
            var nextKey = t.NextKey(key);
            if (nextKey == null)
            {
                return false;
            }
            stack.push(nextKey);
            stack.push(t.get(nextKey));
            return true;
        }

        public int Error()
        {
            var err = stack.pop();
            throw new Exception(Convert.ToString(err));
        }
    }
}