using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OpenOrm.Extensions
{
    public static partial class Extensions
    {
        //public static void CopyTo<T, T2>(this T copyFromObject, T2 copyToObject)
        //{
        //    //BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        //    BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        //    FieldInfo[] fieldsFromObject = copyFromObject.GetType().GetFields(bindFlags);
        //    FieldInfo[] fieldsToObject = copyToObject.GetType().GetFields(bindFlags);

        //    for (int i = 0; i < fieldsFromObject.Length; i++)
        //    {
        //        FieldInfo fieldFrom = copyFromObject.GetType().GetField(fieldsFromObject[i].Name, bindFlags);
        //        if (fieldFrom != null)
        //        {
        //            for (int j = 0; j < fieldsToObject.Length; j++)
        //            {
        //                FieldInfo fieldTo = copyToObject.GetType().GetField(fieldsToObject[j].Name, bindFlags);
        //                if (fieldTo != null)
        //                {
        //                    if (fieldFrom.Name == fieldTo.Name)
        //                    {
        //                        if (fieldFrom.FieldType != fieldTo.FieldType)
        //                        {
        //                            object fromObjectValue = fieldFrom.GetValue(copyFromObject);
        //                            object toObjectValue = Activator.CreateInstance(fieldTo.FieldType);

        //                            if (fromObjectValue.IsList())
        //                            {
        //                                Type innerListItemType = fieldTo.GetBaseType();
        //                                Type listType = typeof(List<>).MakeGenericType(new[] { innerListItemType });
        //                                IList list = (IList)Activator.CreateInstance(listType);

        //                                foreach (object o in fromObjectValue.GetEnumerable())
        //                                {
        //                                    object innerListItem = Activator.CreateInstance(innerListItemType);
        //                                    o.CopyTo(innerListItem);
        //                                    list.Add(innerListItem);
        //                                }

        //                                fieldTo.SetValue(copyToObject, list);
        //                            }
        //                            else
        //                            {
        //                                fromObjectValue.CopyTo(toObjectValue);
        //                                fieldTo.SetValue(copyToObject, toObjectValue);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            fieldTo.SetValue(copyToObject, fieldFrom.GetValue(copyFromObject));
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
