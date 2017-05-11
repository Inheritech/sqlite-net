using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Attributes {
    /// <summary>
    /// Store as text
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class StoreAsTextAttribute : Attribute {
    }
}
