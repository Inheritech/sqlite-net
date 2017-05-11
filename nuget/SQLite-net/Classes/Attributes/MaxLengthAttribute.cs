using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Attributes {

    /// <summary>
    /// Max length
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxLengthAttribute : Attribute {
        /// <summary>
        /// Max length
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length">Length</param>
        public MaxLengthAttribute(int length) {
            Value = length;
        }
    }
}
