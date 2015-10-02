/* 
XML-RPC.NET library
Copyright (c) 2001-2009, Charles Cook <charlescook@cookcomputing.com>

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;

namespace CookComputing.XmlRpc
{
    [Serializable]
    public class XmlRpcStruct : IDictionary, ISerializable, IDeserializationCallback, ICloneable
    {
        private readonly List<string> _keys = new List<string>();
        private readonly List<object> _values = new List<object>();
        private readonly Dictionary<string, object> _base = new Dictionary<string, object>();
        private readonly object _syncRoot = new object();

        public XmlRpcStruct() { }

        protected XmlRpcStruct(SerializationInfo info, StreamingContext context)
        {
        }

        public void Add(object key, object value)
        {
            if (!(key is string))
                throw new ArgumentException("XmlRpcStruct key must be a string.");

            _base.Add((string)key, value);
            _keys.Add((string)key);
            _values.Add(value);
        }

        public void Clear()
        {
            _base.Clear();
            _keys.Clear();
            _values.Clear();
        }

        public bool Contains(object key)
        {
            var theKey = key as string;
            return theKey != null && _base.ContainsKey(theKey);
        }

        public bool ContainsKey(object key)
        {
            var theKey = key as string;
            return theKey != null && _base.ContainsKey(theKey);
        }

        public bool ContainsValue(object value)
        {
            return _base.ContainsValue(value as string);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return new XmlRpcStruct.Enumerator(_keys, _values);
        }

        public bool IsFixedSize { get { return false; } }

        public bool IsReadOnly { get { return false; } }

        public ICollection Keys { get { return _keys; } }

        public void Remove(object key)
        {
            var theKey = key as string;
            if (theKey == null)
                return;

            _base.Remove(theKey);
            var idx = _keys.IndexOf(theKey);
            if (idx < 0)
                return;

            _keys.RemoveAt(idx);
            _values.RemoveAt(idx);
        }

        public ICollection Values { get { return _values; } }

        public object this[object key]
        {
            get
            {
                var theKey = key as string;
                return theKey == null ? null : _base[theKey];
            }

            set
            {
                var theKey = key as string;
                if (theKey == null)
                    throw new ArgumentException("XmlRpcStruct key must be a string.");

                _base[theKey] = value;
                _keys.Add(theKey);
                _values.Add(value);
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException(); // TODO: implement
        }

        public int Count { get { return _base.Count; } }

        public bool IsSynchronized { get { return false; } }

        public object SyncRoot { get { return _syncRoot; } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new XmlRpcStruct.Enumerator(_keys, _values);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException(); // TODO: implement
        }

        int ICollection.Count { get { return _base.Count; } }

        bool ICollection.IsSynchronized { get { return false; } }

        object ICollection.SyncRoot { get { return _syncRoot; } }

        [SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        public void OnDeserialization(object sender)
        {
            throw new NotImplementedException(); // TODO: implement
        }

        public object Clone()
        {
            throw new NotImplementedException(); // TODO: implement
        }

        private class Enumerator : IDictionaryEnumerator
        {
            private readonly List<string> _keys;
            private readonly List<object> _values;
            private int _index;

            public Enumerator(List<string> keys, List<object> values)
            {
                _keys = keys;
                _values = values;
                _index = -1;
            }

            public void Reset()
            {
                _index = -1;
            }

            public object Current
            {
                get
                {
                    CheckIndex();
                    return new DictionaryEntry(_keys[_index], _values[_index]);
                }
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _keys.Count;
            }

            public DictionaryEntry Entry
            {
                get
                {
                    CheckIndex();
                    return new DictionaryEntry(_keys[_index], _values[_index]);
                }
            }

            public object Key
            {
                get
                {
                    CheckIndex();
                    return _keys[_index];
                }
            }

            public object Value
            {
                get
                {
                    CheckIndex();
                    return _values[_index];
                }
            }

            private void CheckIndex()
            {
                if (_index < 0 || _index >= _keys.Count)
                    throw new InvalidOperationException(
                      "Enumeration has either not started or has already finished.");
            }
        }

        #region Implementation of ISerializable

        #endregion
    }
}