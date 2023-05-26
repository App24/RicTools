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
        public static IEnumerable<MethodInfo> GetMethodsRecursive(this Type type, BindingFlags bindingFlags)
        {
            IEnumerable<MethodInfo> methods = type.GetMethods(bindingFlags);

            if (type.BaseType != null)
            {
                methods = methods.Concat(type.BaseType.GetMethodsRecursive(bindingFlags));
            }

            return methods;
        }

        public static MethodInfo GetMethodRecursive(this Type type, string name, BindingFlags bindingFlags)
        {
            var method = type.GetMethod(name, bindingFlags);

            if (method == null && type.BaseType != null)
            {
                method = type.BaseType.GetMethodRecursive(name, bindingFlags);
            }

            return method;
        }

        public static IEnumerable<FieldInfo> GetFieldsRecursive(this Type type, BindingFlags bindingFlags)
        {
            IEnumerable<FieldInfo> methods = type.GetFields(bindingFlags);

            if (type.BaseType != null)
            {
                methods = methods.Concat(type.BaseType.GetFieldsRecursive(bindingFlags));
            }

            return methods;
        }

        public static FieldInfo GetFieldRecursive(this Type type, string name, BindingFlags bindingFlags)
        {
            var method = type.GetField(name, bindingFlags);

            if (method == null && type.BaseType != null)
            {
                method = type.BaseType.GetFieldRecursive(name, bindingFlags);
            }

            return method;
        }
    }
}
