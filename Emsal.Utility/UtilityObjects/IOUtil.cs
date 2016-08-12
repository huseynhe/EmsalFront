using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Emsal.Utility.UtilityObjects
{

    public class IOUtil
    {

        public static string GetObjValue(object obj)
        {

            string objValue = String.Empty;
            string objNewValue = String.Empty;
            if (obj != null)
            {
                Type t = obj.GetType();
                if (!t.IsArray)
                {
                    object obj1 = obj;
                    PropertyInfo[] pinfo = obj.GetType().GetProperties();

                    foreach (PropertyInfo property in pinfo)
                    {
                        object newObj = obj1.GetType().GetProperty(property.Name).GetValue(obj1, null);

                        if (property.PropertyType.FullName.Substring(0, 6) == "System")
                        {

                            if (newObj != null)
                            {
                                Type t1 = newObj.GetType();
                                if (t1.IsArray)
                                {
                                    objValue = objValue + property.Name + ": " + GetPrimitiveArrayValue(newObj);
                                }
                                else
                                {
                                    objValue = objValue + "  " + (property.Name + ": " + property.GetValue(obj1, null)).ToString();
                                }
                            }
                        }
                        else
                        {
                            objValue = objValue + property.Name + ": " + GetObjValue(newObj);

                        }

                    }
                    objNewValue = t.Name.ToString() + "( " + objValue + " )" + " ";

                }
                else if (t.IsArray)
                {
                    if (t.FullName.Substring(0, 6) == "System")
                    {

                        objValue = objValue + ": " + GetPrimitiveArrayValue(obj);
                    }
                    else
                    {
                        object[] arr = obj as object[];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (arr[i] != null)
                            {
                                PropertyInfo[] pinfo = arr.GetValue(i).GetType().GetProperties();
                                foreach (PropertyInfo property in pinfo)
                                {
                                    object newObj = arr.GetValue(i).GetType().GetProperty(property.Name).GetValue(arr.GetValue(i), null);

                                    if (property.PropertyType.FullName.Substring(0, 6) == "System")
                                    {
                                        if (newObj != null)
                                        {
                                            Type t1 = newObj.GetType();
                                            if (t1.IsArray)
                                            {
                                                objValue = objValue + property.Name + ": " + GetPrimitiveArrayValue(newObj);
                                            }
                                            else
                                            {
                                                objValue = objValue + "  " + (property.Name + ": " + property.GetValue(arr.GetValue(i), null)).ToString();
                                            }
                                        }

                                    }
                                    else
                                    {

                                        objValue = objValue + " " + property.Name + " : " + GetObjValue(newObj);

                                    }

                                }
                            }
                        }

                    }

                    objNewValue = t.Name.ToString() + "( " + objValue + " )" + " ";
                }

                return objNewValue;
            }
            else
            {
                objNewValue = " null ";
                return objNewValue;
            }
        }




        //private static string GetObjectValue(object obj)
        //{


        //    string objValue = String.Empty;
        //    string objNewValue = String.Empty;
        //    PropertyInfo[] pinfo = obj.GetType().GetProperties();
        //    Type t = obj.GetType();
        //    foreach (PropertyInfo property in pinfo)
        //    {
        //        objValue = objValue + "  " + (property.Name + ": " + property.GetValue(obj, null)).ToString();
        //        // string a = obj.GetType().InvokeMember(property.Name, BindingFlags.GetProperty, null, obj, null).ToString();

        //    }
        //    objNewValue = t.Name.ToString() + "( " + objValue + " )" + " ";
        //    return objNewValue;
        //}

        public static string GetPVValue(string variableName, object obj)
        {

            object objValue = String.Empty;
            if (obj == null)
            {
                objValue = variableName + ": " + "null" + " ";
            }
            else
            {
                objValue = variableName + ": " + obj + " ";
            }

            return objValue.ToString();
        }
        private static string GetPrimitiveArrayValue(object data)
        {
            string outputValue = String.Empty;

            try
            {

                Array vals = (Array)(data);

                for (int i = 0; i < vals.Length; i++)
                {
                    outputValue = outputValue + " " + vals.GetValue(i).ToString();
                }
                return outputValue;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void TestMethod<T>(T[] array)
        {

            for (int i = 0; i < array.Count(); i++)
            {
                array[i] = (T)Activator.CreateInstance<T>();
            }

        }
        public static string GetFunctionRequestID()
        {



            DateTime time = DateTime.Now;
            string guid = time.ToString("yyyy") + time.ToString("MM") + time.ToString("dd") + time.ToString("HH") + time.ToString("mm") + time.ToString("ss") + time.ToString("fff");
            System.Threading.Thread.Sleep(1);
            return guid;
        }
        public static Int64 GetGuid()
        {



            DateTime time = DateTime.Now;
            string guid = time.ToString("yyyy") + time.ToString("MM") + time.ToString("dd") + time.ToString("HH") + time.ToString("mm") + time.ToString("ss") + time.ToString("fff");
            System.Threading.Thread.Sleep(1);

            return Int64.Parse(guid);
        }
        public static int GetLineNumber()
        {
            int lineNumber = (new System.Diagnostics.StackFrame()).GetFileLineNumber();
            return lineNumber;
        }
        public static ChannelEnum ParseChannel(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                return (ChannelEnum)Enum.Parse(typeof(ChannelEnum), value, true);
            }
            else
            {
                return (ChannelEnum)Enum.Parse(typeof(ChannelEnum), "0", true);
            }

        }


    }
}
