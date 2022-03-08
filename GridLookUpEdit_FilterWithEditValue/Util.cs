using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GridLookUpEdit_FilterWithEditValue
{
    public class Util
    {
        /// <summary>
        /// 生成随机字母与数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Str(int length, bool sleep)
        {
            if (sleep)
                System.Threading.Thread.Sleep(3);
            char[] pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = pattern.Length;
            var random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < length; i++)
            {
                int rnd = random.Next(0, n);
                result += pattern[rnd];
            }
            return result;
        }

        /// <summary>
        /// Converts a Generic List into a DataTable
        /// http://www.cnblogs.com/wintersun/archive/2009/03/22/1419061.html
        /// 用反射判断一个类型是否是Nullable同时获取它的根类型
        /// 【解决了Nullable的问题】
        /// </summary>
        /// <param name="list"></param>
        /// <param name="typ"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(IList list, Type typ)
        {
            DataTable dt = new DataTable();

            // Get a list of all the properties on the object
            PropertyInfo[] pi = typ.GetProperties();

            // Loop through each property, and add it as a column to the datatable
            foreach (PropertyInfo p in pi)
            {
                // The the type of the property
                Type columnType = p.PropertyType;

                // We need to check whether the property is NULLABLE
                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                    columnType = p.PropertyType.GetGenericArguments()[0];
                }

                // Add the column definition to the datatable.
                dt.Columns.Add(new DataColumn(p.Name, columnType));
            }

            // For each object in the list, loop through and add the data to the datatable.
            foreach (object obj in list)
            {
                object[] row = new object[pi.Length];
                int i = 0;

                foreach (PropertyInfo p in pi)
                {
                    row[i++] = p.GetValue(obj, null);
                }

                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}
