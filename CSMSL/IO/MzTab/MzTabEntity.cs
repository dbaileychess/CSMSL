using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.IO.MzTab
{
    public abstract class MzTabEntity
    {
        public int FieldCount { get { return Data.Count; } }

        protected Dictionary<string, object> Data; 

        public object GetValue(string fieldName)
        {
            object o;
            return !Data.TryGetValue(fieldName, out o) ? null : o;
        }

        public T GetValue<T>(string fieldName)
        {
            object o;
            if (!Data.TryGetValue(fieldName, out o))
                return default(T);
            return (T) o;
        }

        public virtual void SetValue(string fieldName, string value)
        {
            Data[fieldName] = value;
        }

        protected void SetRawValue(string fieldName, object value)
        {
            Data[fieldName] = value;
        }

        protected void SetRawValue<T>(string fieldName, int index, T value)
        {
            var list = GetValue<List<T>>(fieldName);

            if (list == null)
            {
                list = new List<T>();
                SetRawValue(fieldName, list);
            }

            list.Insert(index - MzTab.IndexBased, value);
        }

        public virtual string[] GetOptionalFields()
        {
            return Data.Keys.Where(k => k.StartsWith(MzTab.OptionalColumnPrefix)).ToArray();
        }

        public virtual bool ContainsField(string fieldName)
        {
            return Data.ContainsKey(fieldName);
        }
        
        protected MzTabEntity(int capacity)
        {
            Data = new Dictionary<string, object>(capacity);
        }
    }
}
