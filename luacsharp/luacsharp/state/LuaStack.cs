using System;
using System.Collections.Generic;
using System.Linq;
using luacsharp.API;

namespace luacsharp.state
{
    public class LuaStack
    {
        public List<object> slots;
        internal LuaState state;
        internal LuaStack prev;
        internal Closure closure;
        internal List<object> varargs;
        internal int pc;
        internal Dictionary<int, Upvalue> openuvs;

        // internal static LuaStack newLuaStack(int size, LuaState state)
        // {
        //     return new LuaStack
        //     {
        //         slots = new object[size],
        //         top = 0,
        //         state = state
        //     };   
        // }

        internal LuaStack(LuaState state)
        {
            slots = new List<object>();
            this.state = state;
        }
        
        internal int top()
        {
            return slots.Count();
        }

        // internal void check(int n)
        // {
        //     var free = slots.Length - top;
        //     var slotList = slots.ToList();
        //     for (var i = free; i < n; i++)
        //     {
        //         slotList.Add(null);
        //     }
        //
        //     slots = slotList.ToArray();
        // }

        internal void push(object val)
        {
            if (slots.Count() > slots.Capacity)
            {
                throw new StackOverflowException();
            }

            slots.Add(val);
        }

        internal int absIndex(int idx)
        {
            return idx >= 0 || idx <= Consts.LUA_REGISTRYINDEX
                ? idx
                : idx + slots.Count() + 1;
        }

        internal bool isValid(int idx)
        {
            if (idx < Consts.LUA_REGISTRYINDEX)
            {
                /* upvalues */
                int uvIdx = Consts.LUA_REGISTRYINDEX - idx - 1;
                return closure != null && uvIdx < closure.upvals.Length;
            }

            if (idx == Consts.LUA_REGISTRYINDEX)
            {
                return true;
            }

            var absIdx = absIndex(idx);
            return absIdx > 0 && absIdx <= slots.Count();

//            if (idx < Consts.LUA_REGISTRYINDEX)
//            {
//                var uvIdx = Consts.LUA_REGISTRYINDEX - idx - 1;
//                var c = closure;
//                return c != null && (uvIdx < c.upvals.Length);
//            }
//
//            var absIdx = absIndex(idx);
//            return absIdx > 0 && absIdx <= top;
        }

        internal object get(int idx)
        {
            if (idx < Consts.LUA_REGISTRYINDEX)
            {
                var uvIdx = Consts.LUA_REGISTRYINDEX - idx - 1;
                var c = closure;
                if (c == null || uvIdx >= c.upvals.Length)
                {
                    return null;
                }

                return c.upvals[uvIdx].Get();
            }

            if (idx == Consts.LUA_REGISTRYINDEX)
            {
                return state.registry;
            }

            var absIdx = absIndex(idx);
            if (absIdx > 0 && absIdx <= top())
            {
                return slots[absIdx - 1];
            }

            return null;
        }

        internal object pop()
        {
            var v = slots.Last();
            slots.RemoveAt(slots.Count - 1);
            return v;
        }

        internal void pushN(List<object> vals, int n)
        {
            int nVals = vals?.Count() ?? 0;
            if (n < 0)
            {
                n = nVals;
            }

            for (int i = 0; i < n; i++)
            {
                push(i < nVals ? vals[i] : null);
            }
        }

        internal List<object> popN(int n)
        {
            var vals = new List<object>(n);
            for (int i = 0; i < n; i++)
            {
                vals.Add(pop());
            }

            vals.Reverse();
            return vals;
        }


        internal void set(int idx, object val)
        {
            if (idx < Consts.LUA_REGISTRYINDEX)
            {
                /* upvalues */
                int uvIdx = Consts.LUA_REGISTRYINDEX - idx - 1;
                if (closure != null
                    && closure.upvals.Length > uvIdx
                    && closure.upvals[uvIdx] != null)
                {
                    closure.upvals[uvIdx].Set(val);
                }

                return;
            }

            if (idx == Consts.LUA_REGISTRYINDEX)
            {
                state.registry = (LuaTable) val;
                return;
            }

            int absIdx = absIndex(idx);
            slots.RemoveAt(absIdx - 1);
            slots.Insert(absIdx - 1, val);
        }

        internal void reverse(int from, int to)
        {
            slots.Reverse(from, to - from + 1);
        }
    }
}