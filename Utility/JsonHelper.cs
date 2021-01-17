﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class JsonHelper
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonToT<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static T JsonToT<T>(this string json,Type type)
        {
            var instance = Activator.CreateInstance(type);
            JsonConvert.PopulateObject(json,instance);
            return (T)instance;
        }
    }



    public static class ObjectExtension
    {
        private static readonly Dictionary<Type, Func<object, object>> ConvertDictionary = new Dictionary<Type, Func<object, object>>();
        static ObjectExtension()
        {
            ConvertDictionary.Add(typeof(bool), WrapValueConvert(Convert.ToBoolean));
            ConvertDictionary.Add(typeof(bool?), WrapValueConvert(Convert.ToBoolean));
            ConvertDictionary.Add(typeof(int), WrapValueConvert(Convert.ToInt32));
            ConvertDictionary.Add(typeof(int?), WrapValueConvert(Convert.ToInt32));
            ConvertDictionary.Add(typeof(long), WrapValueConvert(Convert.ToInt64));
            ConvertDictionary.Add(typeof(long?), WrapValueConvert(Convert.ToInt64));
            ConvertDictionary.Add(typeof(short), WrapValueConvert(Convert.ToInt16));
            ConvertDictionary.Add(typeof(short?), WrapValueConvert(Convert.ToInt16));
            ConvertDictionary.Add(typeof(double), WrapValueConvert(Convert.ToDouble));
            ConvertDictionary.Add(typeof(double?), WrapValueConvert(Convert.ToDouble));
            ConvertDictionary.Add(typeof(float), WrapValueConvert(Convert.ToSingle));
            ConvertDictionary.Add(typeof(float?), WrapValueConvert(Convert.ToSingle));
            ConvertDictionary.Add(typeof(Guid), m => Guid.Parse(m.ToString()) as object);
            ConvertDictionary.Add(typeof(Guid?), m => Guid.Parse(m.ToString()) as object);
            ConvertDictionary.Add(typeof(string), Convert.ToString);
            ConvertDictionary.Add(typeof(DateTime), WrapValueConvert(Convert.ToDateTime));
            ConvertDictionary.Add(typeof(DateTime?), WrapValueConvert(Convert.ToDateTime));
        }
        /// <summary>
        /// 转换为
        /// </summary>
        public static object ConvertTo(this object inputObj, Type targetType)
        {
            if (inputObj == null)
            {
                if (targetType.IsValueType) throw new Exception($"不能将null转换为{targetType.Name}");
                return null;
            }
            if (inputObj.GetType() == targetType || targetType.IsInstanceOfType(inputObj))
            {
                return inputObj;
            }
            if (ConvertDictionary.ContainsKey(targetType))
            {
                return ConvertDictionary[targetType](inputObj);
            }
            try
            {
                return Convert.ChangeType(inputObj, targetType);
            }
            catch (Exception ex)
            {
                throw new Exception($"未实现到{targetType.Name}的转换方法", ex);
            }
        }
        #region 私有方法
        /// <summary>
        /// 包装值转换
        /// </summary>
        private static Func<object, object> WrapValueConvert<T>(Func<object, T> input) where T : struct
        {
            return i =>
            {
                if (i == null || i is DBNull) return null;
                return input(i);
            };
        }
        #endregion
    }
}
