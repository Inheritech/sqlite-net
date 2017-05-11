using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SQLite.Classes.Attributes;

namespace SQLite.Classes {
    public class EnumCacheInfo {
        public EnumCacheInfo(Type type) {
#if !USE_NEW_REFLECTION_API
            IsEnum = type.IsEnum;
#else
            IsEnum = type.GetTypeInfo().IsEnum;
#endif

            if (IsEnum) {
                // This is a big assumption, but for now support ints only as key, otherwise we still
                // have to pay the price for boxing
                EnumValues = Enum.GetValues(type).Cast<int>().ToDictionary(x => x, x => x.ToString());

#if !USE_NEW_REFLECTION_API
                StoreAsText = type.GetCustomAttribute(typeof(StoreAsTextAttribute), false) != null;
#else
                StoreAsText = type.GetTypeInfo().GetCustomAttribute(typeof(StoreAsTextAttribute), false) != null;
#endif
            }
        }

        public bool IsEnum { get; private set; }

        public bool StoreAsText { get; private set; }

        public Dictionary<int, string> EnumValues { get; private set; }
    }
}
