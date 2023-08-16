
namespace luacsharp
{
    struct binaryChunk
    {
        header header;
        byte sizeUpvalues;
        Prototype mainFunc;
    }

    struct header
    {
        byte[] signature;
        byte version;
        byte format;
        byte[] luacData;
        byte cintSize;
        byte sizetSize;
        byte instrctionSize;
        byte luaIntegerSize;
        byte luaNumberSize;
        int luaCInt;
        float luacNum;
    }
    
    struct Prototype
    {
        private string Source;
        private uint LineDefined;
        private uint LastLineDefined;
        private byte NumParams;
        private byte IsVararg;
        byte MaxStackSize;
        private uint[] Code;
        object[] Constants;
        Upvalue[] Upvalues;
        Prototype[] Protos;
        uint[] Lineinfo;
        LocVar[] locVars;
        string[] UpvalueNames;

    }

    struct Upvalue
    {
        private byte Instack;
        private byte Idx;
    }

    struct LocVar
    {
        private string VarName;
        private uint StartPC;
        private uint EndPC;
    }

    internal static class BinaryChunk
    {
        public const string LUA_SIGNATURE = "\x1bLua";
        public const byte LUAC_VERSION = 0x53;
        public const byte LUAC_FORMAT = 0;
        public const string LUAC_DATA = "\x19\x93\r\n\x1a\n";
        public const uint CINT_SIZE = 4;
        public const uint CSIZET_SIZE_32 = 4;
        public const uint CSIZET_SIZE_64 = 8;
        public const uint INSTRUCTION_SIZE = 4;
        public const uint LUA_INTEGER_SIZE = 8;
        public const uint LUA_NUMBER_SIZE = 8;
        public const ushort LUAC_INT = 0x5678;
        public const double LUAC_NUM = 370.5;

        public const byte TAG_NIL = 0x00;
        public const byte TAG_BOOLEAN = 0x01;
        public const byte TAG_NUMBER = 0x03;
        public const byte TAG_INTEGER = 0x13;
        public const byte TAG_SHORT_STR = 0x04;
        public const byte TAG_LONG_STR = 0x14;
        
        public static Prototype Undump(byte[] data)
        {
            var reader = new Reader {data = data};
            reader.checkHeader();
            reader.readByte();
            return reader.readProto("");
        }
    }
    
}