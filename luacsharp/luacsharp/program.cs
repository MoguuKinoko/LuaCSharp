using System;
using System.IO;
using luacsharp.API;
using luacsharp.binchunk;
using luacsharp.vm;


namespace luacsharp
{

    public class Enter
    {
        
        
    }
    
    public static class program
    {
        public static void Main()
        {
            try
            {
                string[] args = new string[] { "E:/luacsharp/luacsharp/luacsharp/lua/luac.out" };
                
                if (args.Length <= 0) return;
                try
                {
                    var fs = File.OpenRead(args[0]);
                    var data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);

                    var ls = new state.LuaState();
                    ls.Register("print", print);
                    ls.Register("getmetatable", getMetatable);
                    ls.Register("setmetatable", setMetatable);
                    ls.Register("next", next);
                    ls.Register("pairs", pairs);
                    ls.Register("ipairs", iPairs);
                    ls.Load(ref data, "chunk", "b");
                    ls.Call(0, 0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static int print(LuaState ls)
        {
            var nArgs = ls.GetTop();
            for (var i = 1; i <= nArgs; i++)
            {
                if (ls.IsBoolean(i))
                {
                    Console.Write("{0}", ls.ToBoolean(i));
                }
                else if (ls.IsString(i))
                {
                    Console.Write(ls.ToString(i));
                }
                else
                {
                    Console.Write(ls.TypeName(ls.Type(i)));
                }

                if (i < nArgs)
                {
                    Console.Write("\t");
                }
            }

            Console.WriteLine();
            return 0;
        }
        
        // static void luaMain(Prototype proto)
        // {
        //     var nRegs = proto.MaxStackSize;
        //     var ls = new state.LuaState().New(nRegs + 8, proto);
        //     ls.SetTop(nRegs);
        //     for (;;)
        //     {
        //         var pc = ls.PC();
        //         var inst = new Instruction(ls.Fetch());
        //         var opCode = inst.Opcode();
        //         if (opCode != OpCodes.OP_RETURN)
        //         {
        //             // inst.Execute(ls);
        //             Console.Write("[{0:D2}] {1}", pc + 1, inst.OpName());
        //             printStack(ls);
        //         }
        //         else
        //         {
        //             break;
        //         }
        //     }
        // }
        
        private static void list(Prototype f)
        {
            printHeader(f);
            printCode(f);
            printDetail(f);
            foreach (var p in f.Protos)
            {
                list(p);
            }
        }

        private static void printHeader(Prototype f)
        {
            var funcType = "main";
            if (f.LineDefined > 0)
            {
                funcType = "function";
            }

            var varargFlag = "";
            if (f.IsVararg > 0)
            {
                varargFlag = "+";
            }

            Console.Write("\n{0} <{1}:{2},{3}> ({4} instruction)\n", funcType, f.Source, f.LineDefined,
                f.LastLineDefined, f.Code.Length);
            Console.Write("{0,1} params, {2} slots, {3} upvalues, ", f.NumParams, varargFlag, f.MaxStackSize,
                f.Upvalues.Length);
            Console.Write("{0} locals, {1} constants, {2} functions\n", f.LocVars.Length, f.Constants.Length,
                f.Protos.Length);
        }

        private static void printCode(Prototype f)
        {
            for (var pc = 0; pc < f.Code.Length; pc++)
            {
                var c = f.Code[pc];
                var line = "-";
                if (f.LineInfo.Length > 0)
                {
                    line = f.LineInfo[pc].ToString();
                }

                var i = new Instruction(c);
                Console.Write("\t{0}\t[{1}]\t{2:x8} \t", pc + 1, line, i.OpName());
                printOperands(i);
                Console.WriteLine();
                Console.Write("\t{0}\t[{1}]\t0x{2:x8}\n", pc + 1, line, c);
            }
        }

        private static void printDetail(Prototype f)
        {
            Console.Write("constants ({0}):\n", f.Constants.Length);
            for (var i = 0; i < f.Constants.Length; i++)
            {
                var k = f.Constants[i];
                Console.Write("\t{0}\t{1}\n", i + 1, constantToString(k));
            }

            Console.Write("locals ({0}):\n", f.LocVars.Length);
            for (var i = 0; i < f.LocVars.Length; i++)
            {
                var locVar = f.LocVars[i];
                Console.Write("\t{0}\t{1}\t{2}\t{3}\n", i, locVar.VarName, locVar.StartPC + 1, locVar.EndPC + 1);
            }

            Console.Write("upvalues ({0}):\n", f.Upvalues.Length);
            for (var i = 0; i < f.Upvalues.Length; i++)
            {
                var upval = f.Upvalues[i];
                Console.Write("\t{0}\t{1}\t{2}\t{3}\n", i, upvalName(f, i), upval.Instack, upval.Idx);
            }
        }

        private static string upvalName(Prototype f, int idx)
        {
            return f.UpvalueNames.Length > 0 ? f.UpvalueNames[idx] : "-";
        }

        private static object constantToString(object k)
        {
            if (k == null)
            {
                return "nil";
            }

            switch (k.GetType().Name)
            {
                case "Boolean": return (bool)k;
                case "Double": return (double)k;
                case "Long": return (long)k;
                case "String": return (string)k;
                default: return "?";
            }
        }

        private static void printOperands(Instruction i)
        {
            int a, b, c, ax, bx, sbx;
            switch (i.OpMode())
            {
                case OpCodes.IABC:
                    var abc = i.ABC();
                    Console.Write($"{abc.Item1:D}", abc.Item1);
                    if (i.BMode() != OpCodes.OpArgN)
                    {
                        if (abc.Item2 > 0xFF)
                        {
                            Console.Write($" {-1 - (abc.Item2 & 0xFF):D}");
                        }
                        else
                        {
                            Console.Write($" {abc.Item2:D}");
                        }
                    }

                    if (i.CMode() != OpCodes.OpArgN)
                    {
                        if (abc.Item3 > 0xFF)
                        {
                            Console.Write($"{-1 - (abc.Item3 & 0xFF):D}");
                        }
                        else
                        {
                            Console.Write($" {abc.Item3:D}");
                        }
                    }

                    break;
                case OpCodes.IABx:
                    var aBx = i.ABx();
                    Console.Write($" {aBx.Item1:D}");
                    if (i.BMode() == OpCodes.OpArgK)
                    {
                        Console.Write($"{-1 - aBx.Item2:D}");
                    }
                    else if (i.BMode() == OpCodes.OpArgU)
                    {
                        Console.Write($" {aBx.Item2:D}");
                    }

                    break;
                case OpCodes.IAsBx:
                    var asBx = i.AsBx();
                    Console.Write($"{asBx.Item1:D} {asBx.Item2:D}");
                    break;
                case OpCodes.IAx:
                    ax = i.Ax();
                    Console.Write($"{-1 - ax:D}");
                    break;
            }
        }
        
        internal static void printStack(LuaState ls)
        {
            var top = ls.GetTop();
            for (var i = 1; i <= top; i++)
            {
                var t = ls.Type(i);
                switch (t)
                {
                    case Consts.LUA_TBOOLEAN:
                        Console.Write($"[{ls.ToBoolean(i)}]");
                        break;
                    case Consts.LUA_TNUMBER:
                        Console.Write($"[{ls.ToNumber(i)}]");
                        break;
                    case Consts.LUA_TSTRING:
                        Console.Write($"[{ls.ToString(i)}]");
                        break;
                    default:
                        Console.Write($"[{ls.TypeName(t)}]");
                        break;
                }
            }

            Console.WriteLine();
        }
        
        private static int next(LuaState ls)
        {
            ls.SetTop(2);
            if (ls.Next(1))
            {
                return 2;
            }
            else
            {
                ls.PushNil();
                return 1;
            }
        }
        
        private static int pairs(LuaState ls)
        {
            ls.PushCsharpFunction(next); /* will return generator, */
            ls.PushValue(1); /* state, */
            ls.PushNil();
            return 3;
        }

        private static int iPairs(LuaState ls)
        {
            ls.PushCsharpFunction(iPairsAux); /* iteration function */
            ls.PushValue(1); /* state */
            ls.PushInteger(0); /* initial value */
            return 3;
        }
        
        private static int iPairsAux(LuaState ls)
        {
            var i = ls.ToInteger(2) + 1;
            ls.PushInteger(i);
            return ls.GetI(1, i) == Consts.LUA_TNIL ? 1 : 2;
        }
        
        private static int getMetatable(LuaState ls)
        {
            if (!ls.GetMetatable(1))
            {
                ls.PushNil();
            }

            return 1;
        }

        private static int setMetatable(LuaState ls)
        {
            ls.SetMetatable(1);
            return 1;
        }
    }
}