using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace umbe.data {
    public class ListDataReader<T> : IDataReader {
        private List<T> _list;
        private int _counter;
        private T _current;
        private Dictionary<int, Func<T, object>> _delegates;
        private Dictionary<string, int> _propertyIndexes;
        private Dictionary<int, string> _propertyNames;
        private Dictionary<int, Type> _propertyTypes;
        public ListDataReader (List<T> list) {
            _list = list;
            _counter = -1;
            _delegates = new Dictionary<int, Func<T, object>> ();
            _propertyIndexes = new Dictionary<string, int> ();
            _propertyNames = new Dictionary<int, string>();
            _propertyTypes = new Dictionary<int, Type>();

            var properties = typeof (T).GetProperties (BindingFlags.Instance | BindingFlags.Public);
            for (var i = 0; i < properties.Length; ++i) {
                var property = properties[i];
                _delegates.Add (i, (Func<T, object>) Delegate.CreateDelegate (typeof (Func<T, object>), property.GetGetMethod (nonPublic: true)));
                _propertyIndexes.Add (property.Name, i);
                _propertyNames.Add (i, property.Name);
                _propertyTypes.Add(i, property.PropertyType);
            }
        }
        public object this [int i] => _delegates[i] (_current);

        public object this [string name] => _delegates[GetOrdinal (name)] (_current);

        public int Depth => 0;

        public bool IsClosed => false;

        public int RecordsAffected => 0;

        public int FieldCount => _propertyIndexes.Keys.Count;

        public void Close () {

        }

        public void Dispose () {

        }

        public bool GetBoolean (int i) {
            CheckIndex(i);
            return (bool) _delegates[i] (_current);
        }

        public byte GetByte (int i) {
            CheckIndex(i);
            return (byte) _delegates[i] (_current);
        }

        public long GetBytes (int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
            throw new NotImplementedException ();
        }

        public char GetChar (int i) {
            CheckIndex(i);
            return (char) _delegates[i] (_current);
        }

        public long GetChars (int i, long fieldoffset, char[] buffer, int bufferoffset, int length) {
            throw new NotImplementedException ();
        }

        public IDataReader GetData (int i) {
            return null;
        }

        public string GetDataTypeName (int i) {
            CheckIndex(i);
            return _propertyTypes[i].ToString();
        }

        public DateTime GetDateTime (int i) {
            CheckIndex(i);
            return (DateTime) _delegates[i] (_current);
        }

        public decimal GetDecimal (int i) {
            CheckIndex(i);
            return (decimal) _delegates[i] (_current);
        }

        public double GetDouble (int i) {
            CheckIndex(i);
            return (double) _delegates[i] (_current);
        }

        public Type GetFieldType (int i) {
            CheckIndex(i);
            return _propertyTypes[i];
        }

        public float GetFloat (int i) {
            CheckIndex(i);
            return (float) _delegates[i] (_current);
        }

        public Guid GetGuid (int i) {
            CheckIndex(i);
            return (Guid) _delegates[i] (_current);
        }

        public short GetInt16 (int i) {
            CheckIndex(i);
            return (short) _delegates[i] (_current);
        }

        public int GetInt32 (int i) {
            CheckIndex(i);
            return (int) _delegates[i] (_current);
        }

        public long GetInt64 (int i) {
            CheckIndex(i);
            return (long) _delegates[i] (_current);
        }

        public string GetName (int i) {
            CheckIndex(i);
            return _propertyNames[i];
        }

        public int GetOrdinal (string name) {
            CheckName(name);
            return _propertyIndexes[name];
        }

        public DataTable GetSchemaTable () {
            return null;
        }

        public string GetString (int i) {
            CheckIndex(i);
            return (string) _delegates[i] (_current);
        }

        public object GetValue (int i) {
            CheckIndex(i);
            return this [i];
        }

        public int GetValues (object[] values) {
            int i;
            for (i = 0; i < values.Length || i < FieldCount; ++i) {
                values[i] = this [i];
            }
            return i + 1;
        }

        public bool IsDBNull (int i) {
            CheckIndex(i);
            return this [i] == null;
        }

        public bool NextResult () {
            return false;
        }

        public bool Read () {
            if (_list.Count > 0 && _counter < _list.Count) {
                _current = _list[++_counter];
                return true;
            }
            return false;
        }

        private void CheckName(string name){
            if (!_propertyIndexes.ContainsKey(name)){
                throw new IndexOutOfRangeException($"{name} is not a property");
            }
        }

        private void CheckIndex(int i){
            if (i>=_propertyIndexes.Keys.Count){
                throw new IndexOutOfRangeException();
            }
        }
    }
}