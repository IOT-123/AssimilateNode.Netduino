using System;
using Microsoft.SPOT;
using System.Collections;
using System.Text;

namespace AssimilateNode.Core
{

    /// <summary>
    /// Methods not supplied
    /// </summary>
    public static class Extensions
    {


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

        public static string Replace(this String str, string unwanted, string replacement)
        {
            StringBuilder sb = new StringBuilder(str);
            sb.Replace(unwanted, replacement);
            return sb.ToString();
        }



    }
}
