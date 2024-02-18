using System;
using System.Collections.Generic;
using System.Linq;
using luacsharp.API;
using LuaType = System.Int32;
using Math = luacsharp.number.Math;

namespace luacsharp.state
{
    public class LuaTable
    {
        private object[] _arr;
        private Dictionary<object, object> _map;
        private Dictionary<object, object> _keys;
        public LuaTable metatable;
        private object _lastKey;
        private bool _changed;

        public LuaTable(int nArr, int nRec)
        {
            if (nArr >= 0)
            {
                _arr = new object[nArr];
            }

            if (nRec >= 0)
            {
                _map = new Dictionary<object, object>(nRec);
            }
        }
        
        public object NextKey(object key)
        {
            if (_keys is null || key is null && _changed)
            {
                InitKeys();
                _changed = false;
            }

            _keys.TryGetValue(key ?? Consts.NULL_ALIAS, out var nextKey);

            if (nextKey is null && key != null && key != _lastKey)
            {
                throw new Exception("invalid key to 'next'");
            }

            return nextKey;
        }

        private void InitKeys()
        {
            if (_keys is null)
            {
                _keys = new Dictionary<object, object>();
            }
            else
            {
                _keys.Clear();
            }

            object key = Consts.NULL_ALIAS;
            if (_arr != null)
            {
                for (var i = 0; i < _arr.Length; i++)
                {
                    if (_arr[i] == null) continue;
                    long nextKey = i + 1;
                    _keys.Add(key, nextKey);
                    key = nextKey;
                }
            }

            if (_map != null)
            {
                foreach (var k in _map.Keys)
                {
                    var v = _map[k];
                    if (v is null) continue;
                    _keys.Add(key, k);
                    key = k;
                }
            }

            _lastKey = key;
        }

        public object get(object key)
        {
            key = _floatToInteger(key);
            if (LuaValue.isInteger(key) && _arr != null)
            {
                var idx = LuaValue.toInteger(key);
                if (idx >= 1 && idx <= _arr.Length)
                {
                    return _arr[idx - 1];
                }
            }

            if (_map.TryGetValue(key, out object returnValue))
            {
                return returnValue;
            }
            else
            {
                return null;
            }
           
        }

        object _floatToInteger(object key)
        {
            if (LuaValue.isFloat(key))
            {
                var f = LuaValue.toFloat(key);
                return Math.FloatToInteger(f).Item1;
            }

            return key;
        }

        void _shrinkArray()
        {
            for (var i = _arr.Length - 1; i >= 0; i--)
            {
                if (_arr[i] == null)
                {
                    Array.Copy(_arr, 0, _arr, 0, i);
                }
            }
        }

        void _expandArray()
        {
            for (var idx = _arr.Length + 1; true; idx++)
            {
                if (_map != null && _map.ContainsKey(idx))
                {
                    var val = _map.Values.ElementAt(idx);
                    _map.Remove(idx);
                    var b = _arr.ToList();
                    b.Add(val);
                    _arr = b.ToArray();
                }
                else
                {
                    break;
                }
            }
        }

        public int len()
        {
            return _arr.Length;
        }

        public void put(object key, object val)
        {
            if (key == null)
            {
                throw new Exception("table index is nil!");
            }

            if (LuaValue.isFloat(key) && double.IsNaN(LuaValue.toFloat(key)))
            {
                throw new Exception("table index is NaN!");
            }

            key = _floatToInteger(key);

            if (LuaValue.isInteger(key))
            {
                var idx = LuaValue.toInteger(key);
                if (idx >= 1)
                {
                    var arrLen = _arr?.Length ?? 0;
                    if (idx <= arrLen)
                    {
                        _arr[idx - 1] = val;
                        if (idx == arrLen && val == null)
                        {
                            _shrinkArray();
                        }

                        return;
                    }

                    if (idx == arrLen + 1)
                    {
                        _map?.Remove(idx);

                        if (val != null)
                        {
                            if (_arr == null)
                            {
                                var b = new List<object> {val};
                                _arr = b.ToArray();
                                _expandArray();
                            }
                            else
                            {
                                var b = _arr.ToList();
                                b.Add(val);
                                _arr = b.ToArray();
                                _expandArray();
                            }
                        }

                        return;
                    }
                }
            }

            if (val != null)
            {
                if (_map == null)
                {
                    _map = new Dictionary<object, object>(8);
                }

                if (!_map.ContainsKey(key))
                {
                    _map.Add(key, val);
                }
                else
                {
                    _map[key] = val;
                }
            }
            else
            {
                _map.Remove(key);
            }
        }

        public bool hasMetafield(string fieldName)
        {
            return metatable != null && metatable.get(fieldName) != null;
        }
    }
}
