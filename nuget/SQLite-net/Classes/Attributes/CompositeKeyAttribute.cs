using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Attributes {

    /// <summary>
    /// Composite key
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CompositeKeyAttribute : Attribute {
    }
}
