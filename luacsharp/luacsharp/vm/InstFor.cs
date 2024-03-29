using System;
using luacsharp.API;
using luacsharp.vm;
using LuaVM = luacsharp.state.LuaState;

namespace luacsharp.vm
{
    public class InstFor
    {
        internal static void forPrep(Instruction i, ref LuaVM vm)
        {
            var asBx = i.AsBx();
            var a = asBx.Item1 + 1;
            var sBx = asBx.Item2;

            vm.PushValue(a);
            vm.PushValue(a + 2);
            vm.Arith(Consts.LUA_OPSUB);
            vm.Replace(a);
            vm.AddPC(sBx);
        }

        internal static void forLoop(Instruction i, ref LuaVM vm)
        {
            var asBx = i.AsBx();
            var a = asBx.Item1 + 1;
            var sBx = asBx.Item2;

            // R(A)+=R(A+2);
            vm.PushValue(a + 2);
            vm.PushValue(a);
            vm.Arith(Consts.LUA_OPADD);
            vm.Replace(a);

            var isPositiveStep = vm.ToNumber(a + 2) >= 0;
            if (
                (isPositiveStep && vm.Compare(a, a + 1, Consts.LUA_OPLE)) ||
                (!isPositiveStep && vm.Compare(a + 1, a, Consts.LUA_OPLE)))
            {
                // pc+=sBx; R(A+3)=R(A)
                vm.AddPC(sBx);
                vm.Copy(a, a + 3);
            }
        }
        
        // if R(A+1) ~= nil then {
        //   R(A)=R(A+1); pc += sBx
        // }
        internal static void TForLoop(Instruction i, ref LuaVM vm)
        {
            var (a, sBx ) = i.AsBx();
            a += 1;

            if (!vm.IsNil(a + 1))
            {
                vm.Copy(a + 1, a);
                vm.AddPC(sBx);
            }
        }
    }
}