using LuaVM = luacsharp.state.LuaState;
namespace luacsharp.vm
{
    public class InstUpvalue
    {
        internal static void getTabUp(Instruction i, ref LuaVM vm)
        {
            var a_c = i.ABC();
            var a = a_c.Item1 + 1;
            var c = a_c.Item3;

            vm.PushGlobalTable();
            vm.GetRK(c);
            vm.GetTable(-2);
            vm.Replace(a);
            vm.Pop(1);
        }
    }
}