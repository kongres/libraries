using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Reflection
{
    using System.Reflection;

    public static class TypeUtils
    {
        public static PropertyInfo GetPropertyByName(this Type type, string name, bool isCaseIgnore = true)
        {
            return type.GetProperties().FirstOrDefault(x => string.Equals(x.Name, name, isCaseIgnore ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));
        }
    }
}
