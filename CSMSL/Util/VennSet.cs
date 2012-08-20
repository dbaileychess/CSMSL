using System;
using System.Collections.Generic;

namespace CSMSL.Util
{
    public class VennSet<T> : HashSet<T> where T : IEquatable<T>
    {
        private string _name;

        public VennSet(IEnumerable<T> items, string name = "")
            : base(items)
        {
            Name = name;
        }

        public VennSet(string name = "")
            : base()
        {
            Name = name;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void Add(IEnumerable<T> items)
        {
            this.UnionWith(items);
        }

        public override string ToString()
        {
            return string.Format("{0} Count = {1:G0}", Name, Count);
        }
    }
}