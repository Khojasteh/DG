// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Document.Generator.Helpers
{
    public static class ReflectionExtensions
    {
        public static readonly IReadOnlyDictionary<string, string> OperatorNames = new Dictionary<string, string>
        {
            ["op_Equality"] = "Equality",
            ["op_Inequality"] = "Inequality",
            ["op_GreaterThan"] = "Greater Than",
            ["op_LessThan"] = "Less Than",
            ["op_GreaterThanOrEqual"] = "Greater Than Or Equal",
            ["op_LessThanOrEqual"] = "Less Than Or Equal",
            ["op_BitwiseAnd"] = "Bitwise And",
            ["op_BitwiseOr"] = "Bitwise Or",
            ["op_Addition"] = "Addition",
            ["op_Subtraction"] = "Subtraction",
            ["op_Division"] = "Division",
            ["op_Modulus"] = "Modulus",
            ["op_Multiply"] = "Multiply",
            ["op_LeftShift"] = "Left Shift",
            ["op_RightShift"] = "Right Shift",
            ["op_ExclusiveOr"] = "Exclusive Or",
            ["op_UnaryNegation"] = "Unary Negation",
            ["op_UnaryPlus"] = "Unary Plus",
            ["op_LogicalNot"] = "Logical Not",
            ["op_OnesComplement"] = "Ones Complement",
            ["op_False"] = "Absolute False",
            ["op_True"] = "Absolute True",
            ["op_Increment"] = "Increment",
            ["op_Decrement"] = "Decrement",
            ["op_Implicit"] = "Implicit",
            ["op_Explicit"] = "Explicit",
        };

        public static IEnumerable<CustomAttributeData> GetCustomeAttributesOrFailSilent(this MemberInfo memberInfo)
        {
            try
            {
                return memberInfo.GetCustomAttributesData();
            }
            catch
            {
                return Enumerable.Empty<CustomAttributeData>();
            }
        }

        public static string GetCRef(this MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case Type typeInfo:
                    return typeInfo.GetCRef();
                case MethodBase methodInfo:
                    return methodInfo.GetCRef();
                case FieldInfo fieldInfo:
                    return fieldInfo.GetCRef();
                case PropertyInfo propertyInfo:
                    return propertyInfo.GetCRef();
                case EventInfo eventInfo:
                    return eventInfo.GetCRef();
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberInfo));
            }
        }

        public static string GetDisplayName(this MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case Type typeInfo:
                    return typeInfo.GetDisplayName();
                case MethodBase methodInfo:
                    return methodInfo.GetDisplayName();
                case FieldInfo fieldInfo:
                    return fieldInfo.GetDisplayName();
                case PropertyInfo propertyInfo:
                    return propertyInfo.GetDisplayName();
                case EventInfo eventInfo:
                    return eventInfo.GetDisplayName();
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberInfo));
            }
        }

        #region Type

        public static bool IsVisible(this Type type)
        {
            if (type.IsNotPublic)
                return false;

            if (type.IsNested)
                return type.DeclaringType.IsVisible();

            return true;
        }

        public static bool IsDelegate(this Type type)
        {
            return type.Equals(typeof(Delegate)) || type.IsSubclassOf(typeof(Delegate));
        }

        public static bool IsUnsafe(this Type type)
        {
            const BindingFlags AllFields =
                BindingFlags.GetField | BindingFlags.SetField |
                BindingFlags.Static | BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic;

            return type.GetFields(AllFields).Any(f => f.FieldType.IsPointer);
        }

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsDerivedExplicitly(this Type type)
        {
            return (type.IsClass || type.IsInterface) && (type.BaseType != null) && (type.BaseType != typeof(object));
        }

        public static string GetRefName(this Type type)
        {
            if (type.IsGenericParameter)
                return type.Name;
            if (type.FullName != null)
                return Referencify(type.FullName);

            return (type.DeclaringType?.GetRefName() ?? type.Namespace) + '.' + Referencify(type.Name);

            string Referencify(string name)
            {
                int i = name.IndexOf('[');
                while (i != -1)
                {
                    var k = name.IndexOf(']', i + 1);
                    while (k + 1 < name.Length && name[k + 1] == ']')
                        k++;
                    name = name.Remove(i, k - i + 1);
                    i = name.IndexOf('[', i);
                }

                return name.Replace('+', '.');
            }
        }

        public static string GetDisplayName(this Type type)
        {
            return CSharp.Instance.FullNameOf(type);
        }

        public static string GetCRef(this Type type)
        {
            return "T:" + type.GetRefName();
        }

        public static IEnumerable<Type> GetAncestors(this Type type)
        {
            var hierarchy = new Stack<Type>();
            var parent = type.BaseType;
            while (parent != null)
            {
                hierarchy.Push(parent);
                parent = parent.BaseType;
            }
            return hierarchy;
        }

        #endregion

        #region Property

        public static bool IsVisible(this PropertyInfo property)
        {
            return property.GetAccessors().Any(accessor => accessor.IsVisible()) && !property.IsSpecialName;
        }

        public static bool IsIndexer(this PropertyInfo property)
        {
            return property.GetIndexParameters().Length != 0;
        }

        public static string GetDisplayName(this PropertyInfo property)
        {
            return CSharp.Instance.FullNameOf(property, ignoreParameterNames: true);
        }

        public static string GetCRef(this PropertyInfo property)
        {
            return "P:" + property.DeclaringType.GetRefName() + '.' + property.Name;
        }

        #endregion

        #region Event

        public static bool IsVisible(this EventInfo anEvent)
        {
            return (anEvent.AddMethod.IsVisible() || anEvent.RemoveMethod.IsVisible()) && !anEvent.IsSpecialName;
        }

        public static string GetDisplayName(this EventInfo anEvent)
        {
            return anEvent.Name;
        }

        public static string GetCRef(this EventInfo anEvent)
        {
            return "E:" + anEvent.DeclaringType.GetRefName() + '.' + anEvent.Name;
        }

        #endregion

        #region Field

        public static bool IsVisible(this FieldInfo field)
        {
            return !field.IsPrivate && !field.IsSpecialName;
        }

        public static bool IsVolatile(this FieldInfo field)
        {
            return field.GetRequiredCustomModifiers().Any(type => typeof(IsVolatile).Equals(type));
        }

        public static string GetDisplayName(this FieldInfo field)
        {
            return field.Name;
        }

        public static string GetCRef(this FieldInfo field)
        {
            return "F:" + field.DeclaringType.GetRefName() + '.' + field.Name;
        }

        #endregion

        #region Method

        public static bool IsVisible(this MethodBase method)
        {
            return !method.IsPrivate && (method.IsPublic || method.IsFamily);
        }

        public static bool IsExtension(this MethodBase method)
        {
            return method.IsDefined(typeof(ExtensionAttribute), false);
        }

        public static bool IsAsync(this MethodBase method)
        {
            return method.IsDefined(typeof(AsyncStateMachineAttribute), false);
        }

        public static bool IsOverride(this MethodBase method)
        {
            return (method as MethodInfo)?.GetBaseDefinition().DeclaringType != method.DeclaringType;
        }

        public static bool IsUnsafe(this MethodBase method)
        {
            return (method is MethodInfo info && info.ReturnType.IsPointer)
                || method.GetParameters().Any(p => p.ParameterType.IsPointer);
        }

        public static bool IsOperator(this MethodBase method)
        {
            return method.IsSpecialName && OperatorNames.ContainsKey(method.Name);
        }

        public static bool IsTypeCastingOperator(this MethodBase method)
        {
            return (method.Name == "op_Implicit") || (method.Name == "op_Explicit");
        }

        public static string GetDisplayName(this MethodBase method)
        {
            if (OperatorNames.TryGetValue(method.Name, out var operatorName))
            {
                if (!method.IsTypeCastingOperator())
                    return operatorName;

                var sourceType = method.GetParameters()[0].ParameterType;
                var targetType = ((MethodInfo)method).ReturnType;
                return $"{operatorName} {sourceType.GetDisplayName()} to {targetType.GetDisplayName()}";
            }

            return CSharp.Instance.FullNameOf(method, ignoreParameterNames: true);
        }

        public static string GetCRef(this MethodBase method)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendTo(builder);
                return builder.ToString();
            }

            void AppendTo(StringBuilder sb)
            {
                var parameters = method.GetParameters();
                sb.Append("M:");
                sb.Append(method.DeclaringType.GetRefName());
                sb.Append('.');
                sb.Append(method.IsConstructor ? "#ctor" : method.Name);
                if (method.IsGenericMethod)
                {
                    sb.Append("``");
                    sb.Append(method.GetGenericArguments().Length);
                }
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i == 0)
                        sb.Append('(');
                    else
                        sb.Append(',');
                    AppendType(sb, parameters[i].ParameterType);
                }
                if (parameters.Length != 0)
                    sb.Append(')');
                if (method.IsTypeCastingOperator())
                {
                    sb.Append('~');
                    AppendType(sb, (method as MethodInfo).ReturnType);
                }
            }

            void AppendType(StringBuilder sb, Type type, Type nestedType = null)
            {
                if (type.IsGenericParameter)
                    AppendTypeParameter(sb, type);
                else
                {
                    if (type.IsNested)
                        AppendType(sb, type.DeclaringType, type);
                    else
                        sb.Append(type.Namespace);
                    sb.Append('.');
                    sb.Append(type.Name);
                    if (type.IsGenericType && sb[sb.Length - 2] == '`')
                    {
                        sb.Length -= 2;
                        AppendTypeArguments(sb, type.IsNested && type.IsTypeDefinition && nestedType != null ? nestedType : type);
                    }
                }
            }

            void AppendTypeArguments(StringBuilder sb, Type type)
            {
                var anyArgs = false;
                foreach (var typeArgument in type.GetGenericArguments())
                {
                    if (!typeArgument.IsGenericTypeParameter || (type.IsGenericTypeDefinition || !type.IsNested))
                    {
                        sb.Append(anyArgs ? ',' : '{');
                        AppendType(sb, typeArgument);
                        anyArgs = true;
                    }
                }
                if (anyArgs)
                    sb.Append('}');
            }

            void AppendTypeParameter(StringBuilder sb, Type typeParameter)
            {
                sb.Append('`');
                if (typeParameter.IsGenericMethodParameter)
                    sb.Append('`');
                sb.Append(typeParameter.GenericParameterPosition);
            }
        }

        #endregion

        #region Enum

        public static string GetCRef(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            return $"F:{enumType.GetRefName()}.{enumType.GetEnumName(enumValue)}";
        }

        #endregion
    }
}
