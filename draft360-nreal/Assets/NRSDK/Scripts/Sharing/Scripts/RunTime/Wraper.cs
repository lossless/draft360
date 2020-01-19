/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRToolkit.Sharing
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;

    public class Wraper
    {
        public static Dictionary<string, Type> TypeDict = new Dictionary<string, Type>();
        private static Dictionary<Type, Type> m_InternalAttributeDict = new Dictionary<Type, Type>();
        private static Dictionary<Type, Dictionary<string, FieldInfo>> m_TypeFieldsDict = new Dictionary<Type, Dictionary<string, FieldInfo>>();
        private static bool m_IsInitialize = false;

        public static void Initialize()
        {
            if (m_IsInitialize)
            {
                return;
            }
            ScanInternalAttributes();

            var list = GetAllChildClassByBase(typeof(NetworkBehaviour));
            foreach (var item in list)
            {
                var fieldlist = GetAttributeField(item);
                Dictionary<string, FieldInfo> fieldDice = new Dictionary<string, FieldInfo>();
                foreach (var field in fieldlist)
                {
                    fieldDice.Add(field.Name, field);
                }
                m_TypeFieldsDict.Add(item, fieldDice);
            }

            m_IsInitialize = true;
        }

        public static List<SynObject> GetFieldList(object obj)
        {
            Initialize();
            List<FieldInfo> fieldlist = new List<FieldInfo>();
            Dictionary<string, FieldInfo> fieldDict;
            m_TypeFieldsDict.TryGetValue(obj.GetType(), out fieldDict);
            if (fieldDict != null)
            {
                fieldlist.AddRange(fieldDict.Values);
            }
            List<SynObject> fieldobjlist = new List<SynObject>();
            for (int i = 0; i < fieldlist.Count; i++)
            {
                var fieldobj = fieldlist[i].GetValue(obj);
                if (fieldobj != null)
                {
                    fieldobjlist.Add((SynObject)fieldobj);
                }
            }
            if (fieldobjlist.Count == 0)
            {
                return null;
            }
            return fieldobjlist;
        }

        public static object GetFieldValue(string fieldname, object obj)
        {
            Dictionary<string, FieldInfo> fieldDict;
            m_TypeFieldsDict.TryGetValue(obj.GetType(), out fieldDict);

            if (fieldDict != null)
            {
                FieldInfo fieldInfo;
                fieldDict.TryGetValue(fieldname, out fieldInfo);
                if (fieldInfo != null)
                {
                    return fieldInfo.GetValue(obj);
                }
            }

            return null;
        }

        private static void ScanInternalAttributes()
        {
            var types = typeof(Wraper).Assembly.GetTypes();
            foreach (var item in types)
            {
                var basetype = item.BaseType;
                if (basetype != null && basetype.Equals(typeof(SynObject)))
                {
                    m_InternalAttributeDict.Add(item, item);
                }
            }
        }

        private static List<FieldInfo> GetAttributeField(Type type)
        {
            List<FieldInfo> list = new List<FieldInfo>();
            foreach (FieldInfo field in type.GetFields())
            {
                //foreach (var attr in field.GetCustomAttributes())
                //{
                //    if (m_InternalAttributeDict.ContainsKey(attr.GetType()))
                //    {
                //        list.Add(field);
                //    }
                //}

                if (m_InternalAttributeDict.ContainsKey(field.FieldType))
                {
                    list.Add(field);
                }
            }

            return list;
        }

        private static List<Type> GetAllChildClassByBase(Type basetype)
        {
            List<Type> list = new List<Type>();

            var types = Assembly.GetCallingAssembly().GetTypes();
            foreach (var type in types)
            {
                var temp_baseType = type.BaseType;
                while (temp_baseType != null)
                {
                    if (temp_baseType.Equals(basetype))
                    {
                        Type objtype = Type.GetType(type.FullName, true);
                        list.Add(objtype);
                        break;
                    }
                    else
                    {
                        temp_baseType = temp_baseType.BaseType;
                    }
                }
            }

            return list;
        }
    }
}
