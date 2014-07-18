using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.IO.MzTab
{
    public abstract class MzTabEntity
    {
        public int FieldCount
        {
            get { return Data.Count; }
        }

        protected Dictionary<string, object> Data;

        public string this[string fieldName]
        {
            get { return GetValue(fieldName); }
            set { SetValue(fieldName, value); }
        }

        public abstract string GetValue(string fieldName);
        public abstract void SetValue(string fieldName, string value);

        public virtual T GetValue<T>(string fieldName)
        {
            object o;
            if (!Data.TryGetValue(fieldName, out o))
                return default(T);
            return (T) o;
        }
        
        protected void SetRawValue(string fieldName, object value)
        {
            Data[fieldName] = value;
        }

        protected void SetRawValue<T>(ref List<T> container, int index, T value)
        {
            if (container == null)
            {
                container = new List<T>();
            }

            container.Insert(index - MzTab.IndexBased, value);
        }

        protected void SetRawValue<T>(string fieldName, int index, int index2, T value)
        {
            var list = GetValue<List<T>>(fieldName);

            if (list == null)
            {
                list = new List<T>();
                SetRawValue(fieldName, list);
            }

            list.Insert(index - MzTab.IndexBased, value);
        }

        protected string GetListValue<T>(List<T> list, int index)
        {
            if (list == null)
                return MzTab.NullFieldText;
            index -= MzTab.IndexBased;
            if (index >= list.Count)
                return MzTab.NullFieldText;
            return list[index].ToString();
        }

        public virtual string[] GetOptionalFields()
        {
            return Data.Keys.Where(k => k.StartsWith(MzTab.OptionalColumnPrefix)).ToArray();
        }

        public virtual bool ContainsField(string fieldName)
        {
            return Data.ContainsKey(fieldName);
        }

        public IEnumerable<KeyValuePair<string, object>> GetListValues(string fieldName)
        {
            object o;
            if (!Data.TryGetValue(fieldName, out o))
            {
                yield break;
            }

            var list = o as IEnumerable;
            if (list == null)
                yield break;

            int i = 1;
            foreach (var item in list)
            {
                string expandedName = fieldName.Replace("[]", "[" + i + "]");
                yield return new KeyValuePair<string, object>(expandedName, item);
                i++;
            }
        }

        public string GetOptionalData(string optionalParameter)
        {
            object data;
            if (!Data.TryGetValue(optionalParameter, out data))
                return null;
            return (string)data;
        }

        public virtual IEnumerable<string> GetStringValues(IEnumerable<string> headers)
        {
            return headers.Select(GetValue);
        }

        protected MzTabEntity(int capacity)
        {
            Data = new Dictionary<string, object>(capacity);
        }

        protected MzTabEntity()
        {
            Data = new Dictionary<string, object>();
        }

        protected static IEnumerable<string> GetHeaders<T,T2>(IList<T> data, string fieldName, Func<T, T2> selector) where T : MzTabEntity where T2 : IList
        {
            int indexers = fieldName.Count(c => c.Equals('['));

            switch (indexers)
            {
                case 1:
                {
                    int maxIndex = data.Max(d => selector(d).Count);
                    for (int i = 1; i <= maxIndex; i++)
                    {
                        yield return fieldName.Replace("[]", "[" + i + "]");
                    }
                }
                    break;
                case 2:
                    break;
                default:
                    throw new ArgumentException("Cannot handle more than 2 indexers");
            }
        }

        /// <summary>
        /// Gets the header row data from a list of data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetHeader<T>(IList<T> data) where T : MzTabEntity
        {
            Type tType = typeof(T);

            if (typeof(MzTabPSM) == tType)
            {
                return MzTabPSM.Fields.GetHeader(data.Cast<MzTabPSM>().ToList());
            }

            if (typeof(MzTabProtein) == tType)
            {
                return MzTabProtein.Fields.GetHeader(data.Cast<MzTabProtein>().ToList());
            }

            return null;
        }
    }
}
