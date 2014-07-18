using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.IO.MzTab
{
    public abstract class MzTabEntity
    {
        public int FieldCount
        {
            get { return Data.Count; }
        }

        protected Dictionary<string, object> Data;

        public object this[string fieldName]
        {
            get { return GetValue(fieldName); }
            set { SetRawValue(fieldName, value); }
        }

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

        public virtual IEnumerable<object> GetValues(IEnumerable<string> headers)
        {
            foreach (string header in headers)
            {
                if (header.Contains("["))
                {
                    string condensedFieldName;
                    List<int> indices = MzTab.GetFieldIndicies(header, out condensedFieldName);

                    if (indices.Count > 0)
                    {
                        var list = Data[condensedFieldName] as IList;
                        if (list != null)
                        {
                            int index = indices[0] - MzTab.IndexBased;
                            if (index < list.Count)
                            {
                                yield return list[index];
                                continue;
                            }
                        }
                    }
                   
                    yield return "null";
                    continue;
                }

                object value;
                if (!Data.TryGetValue(header, out value))
                {
                    yield return "null";
                }
                else
                {

                    if (value is Enum)
                    {
                        yield return (int)value;
                        continue;
                    }

                    var list = value as IList;
                    if (list == null)
                    {
                        yield return value;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        bool first = true;
                        foreach (var item in list)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                sb.Append('|');
                            }
                            sb.Append(item);
                        }
                        yield return sb.ToString();
                    }
                }
            }
        }

        protected MzTabEntity(int capacity)
        {
            Data = new Dictionary<string, object>(capacity);
        }
        
        protected static IEnumerable<string> GetHeaders<T>(IList<T> data, string fieldName) where T : MzTabEntity
        {
            int indexers = fieldName.Count(c => c.Equals('['));

            switch (indexers)
            {
                case 1:
                {
                    int maxIndex = data.Max(d => ((IList) d.GetValue(fieldName)).Count);
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
                return MzTabPSM.GetHeader(data.Cast<MzTabPSM>().ToList());
            }

            if (typeof(MzTabProtein) == tType)
            {
                return MzTabProtein.GetHeader(data.Cast<MzTabProtein>().ToList());
            }

            return null;
        }
    }
}
