using luacsharp.API;
using luacsharp.binchunk;

namespace luacsharp.state
{
    public struct Closure
    {
        internal binchunk.Prototype proto;
        internal CsharpFunction csharpFunc;

        internal static Closure newLuaClosure(ref binchunk.Prototype proto)
        {
            return new Closure
            {
                proto = proto
            };
        }
        
        internal static Closure newCsharpClosure(CsharpFunction charpFunc)
        {
            return new Closure
            {
                csharpFunc = charpFunc
            };
        }
    }
}