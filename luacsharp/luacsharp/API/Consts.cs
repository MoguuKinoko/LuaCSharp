namespace luacsharp.API
{
    public class Consts
    {
        internal const byte LUA_OK = 0;
        internal const byte LUA_YIELD = 1;
        internal const byte LUA_ERRRUN = 2;
        internal const byte LUA_ERRSYNTAX = 3;
        internal const byte LUA_ERRMEM = 4;
        internal const byte LUA_ERRGCMM = 5;
        internal const byte LUA_ERRERR = 6;
        internal const byte LUA_ERRFILE = 7;
        
        public const string NULL_ALIAS = "null";
        
        internal const int LUA_TNONE = -1;
        public const int LUA_TNIL = 0;
        public const int LUA_TBOOLEAN = 1;
        public const int LUA_TLIGHTUSERDATA = 2;
        public const int LUA_TNUMBER = 3;
        public const int LUA_TSTRING = 4;
        public const int LUA_TTABLE = 5;
        public const int LUA_TFUNCTION = 6;
        public const int LUA_TUSERDATA = 7;
        public const int LUA_TTHREAD = 8;

        public const int LUA_OPADD = 0;
        public const int LUA_OPSUB = 1;
        public const int LUA_OPMUL = 2;
        public const int LUA_OPMOD = 3;
        public const int LUA_OPPOW = 4;
        public const int LUA_OPDIV = 5;
        public const int LUA_OPIDIV = 6;
        public const int LUA_OPBAND = 7;
        public const int LUA_OPBOR = 8;
        public const int LUA_OPBXOR = 9;
        public const int LUA_OPSHL = 10;
        public const int LUA_OPSHR = 11;
        public const int LUA_OPUNM = 12;
        public const int LUA_OPBNOT = 13;


        public const int LUA_OPEQ = 0;
        public const int LUA_OPLT = 1;
        public const int LUA_OPLE = 2;
        
        public const int LUA_MINSTACK = 20;
        public const int LUAI_MAXSTACK = 1000000;
        public const int LUA_REGISTRYINDEX = -LUAI_MAXSTACK - 1000;
        public const long LUA_RIDX_GLOBALS = 2;
    }
}