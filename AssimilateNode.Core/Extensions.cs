using System;
using System.Collections;
using System.Text;

namespace AssimilateNode.Core
{

    /// <summary>
    /// Methods not supplied
    /// </summary>
    public static class Extensions
    {


        /// <summary>
        /// Adds or updates the hashtable based on the Key
        /// </summary>
        /// <param name="hashtable">The Hashtable</param>
        /// <param name="key">The key to apply thew value to.</param>
        /// <param name="value">The value to add or update.</param>
        public static void AddOrUpdate(this Hashtable hashtable, object key, object value)
        {
            if (hashtable.Contains(key))
            {
                hashtable[key] = value;
            }
            else
            {
                hashtable.Add(key, value);
            }
        }

        /// <summary>
        /// Simple string replace function
        /// </summary>
        /// <returns>
        /// The string with replacements applied
        /// </returns>
        /// <param name="str">The String</param>
        /// <param name="unwanted">The token to replace.</param>
        /// <param name="replacement">The value to replace with.</param>
        public static string Replace(this String str, string unwanted, string replacement)
        {
            StringBuilder sb = new StringBuilder(str);
            sb.Replace(unwanted, replacement);
            return sb.ToString();
        }



    }
}
