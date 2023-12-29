using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LMMCFeedbacks.Runtime;
using UnityEngine;
using static System.Type;

namespace LMMCFeedbacks
{
    public static class ReflectionUtility
    {
        public static List<Type> FindClassesImplementing<T>()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            return types.Where(type => !type.IsInterface&&typeof(T).IsAssignableFrom(type)).ToList();
        }

        public static IFeedback CopyFeedback(this IFeedback feedback)
        {
            Type type = feedback.GetType();
            var constructorInfo = type.GetConstructor(EmptyTypes);
            IFeedback clone = (IFeedback)constructorInfo.Invoke(EmptyTypes);
            clone.IsActive = feedback.IsActive;
            
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .SelectMany(x => x.CustomAttributes
                    .Where(t => t.AttributeType == typeof(SerializeField))
                    .Select(_ => x));
            
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsClass)
                {
                    field.SetValue(clone, field.GetValue(feedback));    
                }
                else
                {
                    var constructor = field.FieldType.GetConstructor(new [] { field.FieldType });
                    if (constructor != null)
                    {
                        var a = constructor.Invoke(new []{field.GetValue(feedback)});
                        field.SetValue(clone,a);
                        continue;
                    }
                    if(field.CustomAttributes.Any(x=>x.AttributeType == typeof(NoCopyAttribute)))
                    {
                        field.SetValue(clone, Activator.CreateInstance(field.FieldType));
                        continue;
                    }
                    field.SetValue(clone, field.GetValue(feedback));
                }
            }
            return clone;
        }
    }
}