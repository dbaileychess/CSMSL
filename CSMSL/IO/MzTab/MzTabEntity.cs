using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.IO.MzTab
{
    public abstract class MzTabEntity
    {
        public Dictionary<string, object> OptionalData;

        public string this[string fieldName]
        {
            get { return GetValue(fieldName); }
            set { SetValue(fieldName, value); }
        }

        public abstract string GetValue(string fieldName);
        public abstract void SetValue(string fieldName, string value);

        protected void SetRawValue<T>(ref List<T> container, int index, T value)
        {
            if (container == null)
            {
                container = new List<T>();
            }

            container.Insert(index - MzTab.IndexBased, value);
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
        
        public IEnumerable<KeyValuePair<string, object>> GetListValues(string fieldName)
        {
            object o;
            if (!OptionalData.TryGetValue(fieldName, out o))
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

        public void SetOptionalData(string optionalField, string value)
        {
            if (OptionalData == null)
                OptionalData = new Dictionary<string, object>();

            OptionalData[optionalField] = value;
        }

        public string GetOptionalData(string optionalField)
        {
            if(OptionalData == null)
                return MzTab.NullFieldText;
            object data;
            if (!OptionalData.TryGetValue(optionalField, out data))
                return MzTab.NullFieldText;
            return data.ToString();
        }

        public virtual IEnumerable<string> GetStringValues(IEnumerable<string> headers)
        {
            return headers.Select(GetValue);
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

            List<string> headers = new List<string>();

            if (typeof(MzTabPSM) == tType)
            {
                headers.AddRange(MzTabPSM.Fields.GetHeader(data.Cast<MzTabPSM>().ToList()));
            } else if (typeof(MzTabProtein) == tType)
            {
                headers.AddRange(MzTabProtein.Fields.GetHeader(data.Cast<MzTabProtein>().ToList()));
            }

            // Optional Parameters
            HashSet<string> optionalFields = new HashSet<string>();
            foreach (string field in data.Where(d => d.OptionalData != null).SelectMany(d => d.OptionalData.Keys))
            {
                optionalFields.Add(field);
            }
            headers.AddRange(optionalFields);


            return headers;
        }
    }
}
