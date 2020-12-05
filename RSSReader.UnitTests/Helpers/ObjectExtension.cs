using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.UnitTests.Helpers
{
    static class ObjectExtension
    {
        /// <summary>
        /// Returns property via reflection
        /// </summary>
        public static object GetProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj);
        }
    }
}
