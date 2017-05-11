using System;

namespace SQLite.Classes.Attributes {
    /// <summary>
    /// Autoincrement
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute {
    }
}
