// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MzTabEntity.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

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

        protected void SetListValue<T>(ref List<T> container, int index, T value)
        {
            if (container == null)
            {
                container = new List<T>();
            }

            container.Insert(index - MzTab.IndexBased, value);
        }

        protected void SetFieldValue<T, T2>(ref List<T> container, int index1, Func<T, List<T2>> selector, T2 value)
        {
            T item = container[index1 - MzTab.IndexBased];
            List<T2> list = selector(item);
            list.Add(value);
            //SetFieldValue(ref _software, indices[0], (sw => sw.Settings), value); return;
        }

        protected void SetListValue<T>(ref MzTabMultipleSet<T> container, int index1, int index2, T value)
        {
            if (container == null)
            {
                container = new MzTabMultipleSet<T>();
            }
            container.SetValue(index1, index2, value);
        }

        protected string GetFieldValue<T, T2>(List<T> list, int index, Func<T, List<T2>> selector, int index2)
        {
            if (list == null)
                return MzTab.NullFieldText;
            index -= MzTab.IndexBased;

            if (index >= list.Count)
                return MzTab.NullFieldText;

            T item = list[index];
            if (item == null)
                return MzTab.NullFieldText;

            List<T2> list2 = selector(item);

            index2 -= MzTab.IndexBased;

            if (index2 >= list2.Count)
                return MzTab.NullFieldText;

            T2 item2 = list2[index2];
            if (item2 == null)
                return MzTab.NullFieldText;

            return item2.ToString();
        }

        protected string GetListValue<T>(List<T> list, int index)
        {
            if (list == null)
                return MzTab.NullFieldText;
            index -= MzTab.IndexBased;

            if (index >= list.Count)
                return MzTab.NullFieldText;

            T item = list[index];
            if (item == null)
                return MzTab.NullFieldText;
            return item.ToString();
        }

        protected string GetListValue<T>(MzTabMultipleSet<T> list, int index1, int index2)
        {
            if (list == null)
                return MzTab.NullFieldText;

            T value;
            if (!list.TryGetValue(index1, index2, out value))
                return MzTab.NullFieldText;

            if (value == null)
                return MzTab.NullFieldText;
            return value.ToString();
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
            if (OptionalData == null)
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

        protected static IEnumerable<string> GetHeaders<T, T2>(IList<T> data, string fieldName, Func<T, MzTabMultipleSet<T2>> selector)
            where T : MzTabEntity
        {
            MzTabMultipleSet<T2> set;
            int maxIndex1 = data.Max(d => (set = selector(d)) == null ? 0 : set.MaxIndex1);
            int maxIndex2 = data.Max(d => (set = selector(d)) == null ? 0 : set.MaxIndex2);

            int index1 = fieldName.IndexOf('[', 0);
            int index2 = fieldName.IndexOf('[', index1 + 1);

            for (int i = 1; i <= maxIndex1; i++)
            {
                for (int j = 1; j <= maxIndex2; j++)
                {
                    string iStr = i.ToString();
                    string temp = fieldName.Insert(index1 + 1, i.ToString());
                    yield return temp.Insert(index2 + 1 + iStr.Length, j.ToString());
                }
            }
        }

        protected static IEnumerable<string> GetHeaders<T, T2>(IList<T> data, string fieldName, Func<T, IList<T2>> selector)
            where T : MzTabEntity
        {
            IList<T2> list;
            int maxIndex = data.Max(d => (list = selector(d)) == null ? 0 : list.Count);
            for (int i = 1; i <= maxIndex; i++)
            {
                yield return fieldName.Replace("[]", "[" + i + "]");
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
            Type tType = typeof (T);

            List<string> headers = new List<string>();

            if (typeof (MzTabPSM) == tType)
            {
                headers.AddRange(MzTabPSM.Fields.GetHeader(data.Cast<MzTabPSM>().ToList()));
            }
            else if (typeof (MzTabProtein) == tType)
            {
                headers.AddRange(MzTabProtein.Fields.GetHeader(data.Cast<MzTabProtein>().ToList()));
            }
            else if (typeof (MzTabPeptide) == tType)
            {
                headers.AddRange(MzTabPeptide.Fields.GetHeader(data.Cast<MzTabPeptide>().ToList()));
            }
            else if (typeof (MzTabSmallMolecule) == tType)
            {
                headers.AddRange(MzTabSmallMolecule.Fields.GetHeader(data.Cast<MzTabSmallMolecule>().ToList()));
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