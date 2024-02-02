namespace luacsharp.state
{
    public class Upvalue
    {
        internal readonly int Index;
        private LuaStack _stack;
        private object val;

        public Upvalue(object value)
        {
            val = value;
            Index = 0;
        }

        public Upvalue(LuaStack stack, int index)
        {
            _stack = stack;
            Index = index;
        }

        public object Get()
        {
            return _stack != null ? _stack.get(Index + 1) : val;
        }

        public void Set(object val)
        {
            if (_stack != null)
            {
                _stack.set(Index + 1, val);
            }
            else
            {
                val = val;
            }
        }

        public void Migrate()
        {
            if (_stack == null) return;
            val = _stack.get(Index + 1);
            _stack = null;
        }
    }
}