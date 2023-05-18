using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RicUtils.Utilities
{
    public static class ReflectionUtilities
    {
        public static IEnumerable<MethodInfo> GetRecursiveMethods(this Type type, BindingFlags bindingFlags)
        {
            IEnumerable<MethodInfo> methods = type.GetMethods(bindingFlags);

            if (type.BaseType != null)
            {
                methods = methods.Concat(type.BaseType.GetRecursiveMethods(bindingFlags));
            }

            return methods;
        }

        public static MethodInfo GetRecursiveMethod(this Type type, string name, BindingFlags bindingFlags)
        {
            var method = type.GetMethod(name, bindingFlags);

            if (method == null && type.BaseType != null)
            {
                method = type.BaseType.GetRecursiveMethod(name, bindingFlags);
            }

            return method;
        }

        public static IEnumerable<FieldInfo> GetRecursiveFields(this Type type, BindingFlags bindingFlags)
        {
            IEnumerable<FieldInfo> methods = type.GetFields(bindingFlags);

            if (type.BaseType != null)
            {
                methods = methods.Concat(type.BaseType.GetRecursiveFields(bindingFlags));
            }

            return methods;
        }

        public static FieldInfo GetRecursiveField(this Type type, string name, BindingFlags bindingFlags)
        {
            var method = type.GetField(name, bindingFlags);

            if (method == null && type.BaseType != null)
            {
                method = type.BaseType.GetRecursiveField(name, bindingFlags);
            }

            return method;
        }
    }
}
