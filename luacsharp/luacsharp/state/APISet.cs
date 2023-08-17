using System;

namespace luacsharp.state
{
    partial struct LuaState
    {
        public void SetTable(int idx)
        {
            var v = stack.pop();
            var k = stack.pop();
            stack.setTable(idx, new LuaValue(k), new LuaValue(v));
        }
        
        public void SetField(int idx, string k)
        {
            var t = stack.get(idx);
            var v = stack.pop();
            stack.setTable(idx, new LuaValue(k), new LuaValue(v));
        }

        public void SetI(int idx, long n)
        {
            var t = stack.get(idx);
            var v = stack.pop();
            stack.setTable(idx, new LuaValue(n), new LuaValue(v));
        }
    }
}