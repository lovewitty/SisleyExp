using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Reflection;

namespace ExpNewOrd.Common
{
    public partial class ConvertHelper
    {
        /// <summary>  
        /// Represents some utility functions for object-datatable manipulation  
        /// </summary>  
        public sealed class OTConverter
        {

            /// <summary>  
            /// Convert an DataRow to an object  
            /// </summary>  
            /// <typeparam name="T"></typeparam>  
            /// <param name="row"></param>  
            /// <returns></returns>  
            public static T ConvertToObject<T>(DataRow row) where T : new()
            {
                object obj = new T();
                if (row != null)
                {
                    DataTable t = row.Table;
                    GetObject(t.Columns, row, obj);
                }
                if (obj != null && obj is T)
                    return (T)obj;
                else
                    return default(T);

            }

            public static T ConvertToObjectByField<T>(DataRow row) where T : new()
            {
                object obj = new T();
                if (row != null)
                {
                    DataTable t = row.Table;
                    GetObjectByField(t.Columns, row, obj);
                }
                if (obj != null && obj is T)
                    return (T)obj;
                else
                    return default(T);
            }

            /// <summary>  
            /// Convert a data table to an objct list  
            /// </summary>  
            /// <typeparam name="T"></typeparam>  
            /// <param name="t"></param>  
            /// <returns></returns>  
            public static List<T> ConvertTableToObject<T>(DataTable t) where T : new()
            {
                List<T> list = new List<T>();
                foreach (DataRow row in t.Rows)
                {
                    T obj = ConvertToObject<T>(row);
                    list.Add(obj);
                }

                return list;



            }

            public static List<T> ConvertTableToObjectByField<T>(DataTable t) where T : new()
            {
                List<T> list = new List<T>();
                foreach (DataRow row in t.Rows)
                {
                    T obj = ConvertToObjectByField<T>(row);
                    list.Add(obj);
                }

                return list;
            }

            /// <summary>  
            /// Convert object collection to an data table  
            /// </summary>  
            /// <param name="list"></param>  
            /// <returns></returns>  
            public static DataTable GenericListToDataTable(object list)
            {
                DataTable dt = null;
                Type listType = list.GetType();
                if (listType.IsGenericType)
                {
                    //determine the underlying type the List<> contains  
                    Type elementType = listType.GetGenericArguments()[0];

                    //create empty table -- give it a name in case  
                    //it needs to be serialized  
                    dt = new DataTable(elementType.Name + "List");

                    //define the table -- add a column for each public  
                    //property or field  
                    MemberInfo[] miArray = elementType.GetMembers(
                        BindingFlags.Public | BindingFlags.Instance);
                    foreach (MemberInfo mi in miArray)
                    {
                        if (mi.MemberType == MemberTypes.Property)
                        {
                            PropertyInfo pi = mi as PropertyInfo;
                            dt.Columns.Add(pi.Name, pi.PropertyType);
                        }
                        else if (mi.MemberType == MemberTypes.Field)
                        {
                            FieldInfo fi = mi as FieldInfo;
                            dt.Columns.Add(fi.Name, fi.FieldType);
                        }
                    }

                    //populate the table  
                    IList il = list as IList;
                    foreach (object record in il)
                    {
                        int i = 0;
                        object[] fieldValues = new object[dt.Columns.Count];
                        foreach (DataColumn c in dt.Columns)
                        {
                            MemberInfo mi = elementType.GetMember(c.ColumnName)[0];
                            if (mi.MemberType == MemberTypes.Property)
                            {
                                PropertyInfo pi = mi as PropertyInfo;
                                fieldValues[i] = pi.GetValue(record, null);
                            }
                            else if (mi.MemberType == MemberTypes.Field)
                            {
                                FieldInfo fi = mi as FieldInfo;
                                fieldValues[i] = fi.GetValue(record);
                            }
                            i++;
                        }
                        dt.Rows.Add(fieldValues);
                    }
                }
                return dt;
            }


            #region "internal methods"
            private static void GetObject(DataColumnCollection cols, DataRow dr, Object obj)
            {
                Type t = obj.GetType(); //This is used to do the reflection  

                PropertyInfo[] props = t.GetProperties();
                foreach (PropertyInfo pro in props)
                {
                    if (cols.Contains(pro.Name))
                    {
                        pro.SetValue(obj,
                            dr[pro.Name] == DBNull.Value ? null : dr[pro.Name],
                            null);
                    }
                }


                
            }

            private static bool IsNullableType(Type theType)
            {
                return (theType.IsGenericType && theType.
                  GetGenericTypeDefinition().Equals
                  (typeof(Nullable<>)));
            }  

            private static void GetObjectByField(DataColumnCollection cols, DataRow dr, Object obj)
            {
                Type t = obj.GetType(); //This is used to do the reflection  

                FieldInfo[] props = t.GetFields();
                foreach (FieldInfo pro in props)
                {
                    if (cols.Contains(pro.Name))
                    {
                        if (!IsNullableType(pro.FieldType))
                            pro.SetValue(obj,
                                dr[pro.Name] == DBNull.Value ? null : dr[pro.Name]);
                        else//可空类型
                        {
                            pro.SetValue(obj,
                               dr[pro.Name]==null||
                               dr[pro.Name] == DBNull.Value||
                               string.IsNullOrEmpty(dr[pro.Name].ToString()) 
                               ? null :
                                //Convert.ChangeType(dr[pro.Name],pro.FieldType)
                                dr[pro.Name]
                                );
                        }
                    }
                }
            }

            #endregion


        }  
    }
}
