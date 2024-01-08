using System;
using luacsharp.vm;
using LuaVM = luacsharp.state.LuaState;


namespace luacsharp.vm
{
    public class InscMisc
    {
        internal static void move(Instruction i, ref LuaVM vm)
        {
            var ab_ = i.ABC();
            var a = ab_.Item1 + 1;
            var b = ab_.Item2 + 1;
            vm.Copy(b, a);
        }

        internal static void jmp(Instruction i, ref LuaVM vm)
        {
            var asBx = i.AsBx();
            var a = asBx.Item1;
            var sBx = asBx.Item2;
            
            vm.AddPC(sBx);
            if (a != 0)
            {
                throw new Exception("todo!");
            }
        }
    }
}