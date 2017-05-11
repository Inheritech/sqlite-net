using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Attributes {
    /// <summary>
    /// Unique
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : IndexedAttribute {

        /// <summary>
        /// Unique attribute
        /// </summary>
        public override bool Unique
        {
            get { return true; }
            set { /* throw?  */ }
        }
    }
}
