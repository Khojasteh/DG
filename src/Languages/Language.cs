// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Document.Generator.Languages
{
    public abstract class Language
    {
        public abstract string Name { get; }

        public string DeclerationOf(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case Type typeInfo:
                    return DeclerationOf(typeInfo);
                case MethodBase methodInfo:
                    return DeclerationOf(methodInfo);
                case FieldInfo fieldInfo:
                    return DeclerationOf(fieldInfo);
                case PropertyInfo propertyInfo:
                    return DeclerationOf(propertyInfo);
                case EventInfo eventInfo:
                    return DeclerationOf(eventInfo);
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberInfo));
            }
        }

        public string FullNameOf(Type type, bool ignoreParentTypeName = false)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendFullName(builder, type, ignoreParentTypeName);
                return builder.ToString();
            }
        }

        public string DeclerationOf(Type type)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendDecleration(builder, type);
                return builder.ToString();
            }
        }

        public string FullNameOf(PropertyInfo property, bool ignoreParameterNames = false)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendFullName(builder, property, ignoreParameterNames);
                return builder.ToString();
            }
        }

        public string DeclerationOf(PropertyInfo property)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendDecleration(builder, property);
                return builder.ToString();
            }
        }

        public string FullNameOf(EventInfo anEvent)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendFullName(builder, anEvent);
                return builder.ToString();
            }
        }

        public string DeclerationOf(EventInfo anEvent)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendDecleration(builder, anEvent);
                return builder.ToString();
            }
        }

        public string FullNameOf(FieldInfo field)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendFullName(builder, field);
                return builder.ToString();
            }
        }

        public string DeclerationOf(FieldInfo field)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendDecleration(builder, field);
                return builder.ToString();
            }
        }

        public string FullNameOf(MethodBase method, bool ignoreParameterNames = false)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendFullName(builder, method, ignoreParameterNames);
                return builder.ToString();
            }
        }

        public string DeclerationOf(MethodBase method)
        {
            using (var builder = StringBuilderPool.Acquire())
            {
                AppendDecleration(builder, method);
                return builder.ToString();
            }
        }

        #region Protected Members

        protected virtual void AppendAttributes(StringBuilder sb, IEnumerable<CustomAttributeData> attributes, string postfix = LineBreak)
        {
            if (attributes != null)
            {
                foreach (var attributeData in attributes)
                {
                    if (!SpecialAttributeNamespaces.Contains(attributeData.AttributeType.Namespace))
                    {
                        AppendAttribute(sb, attributeData);
                        sb.Append(postfix);
                    }
                }
            }
        }

        protected virtual void AppendAttributeArgument(StringBuilder sb, CustomAttributeNamedArgument argument)
        {
            sb.Append(argument.MemberName);
            sb.Append(" = ");
            AppendAttributeArgument(sb, argument.TypedValue);
        }

        protected virtual void AppendAttributeArgument(StringBuilder sb, CustomAttributeTypedArgument argument)
        {
            AppendConstant(sb, argument.Value, argument.ArgumentType);
        }

        protected virtual void AppendConstant(StringBuilder sb, object value, Type type)
        {
            if (type.IsEnum)
            {
                sb.Append(type.Name);
                sb.Append('.');
                sb.Append(type.GetEnumName(value));
            }
            else
            {
                AppendLiteral(sb, value);
            }
        }

        protected abstract void AppendFullName(StringBuilder sb, Type type, bool ignoreParentTypeName = false);
        protected abstract void AppendFullName(StringBuilder sb, PropertyInfo property, bool ignoreParameterNames = false);
        protected abstract void AppendFullName(StringBuilder sb, EventInfo anEvent);
        protected abstract void AppendFullName(StringBuilder sb, FieldInfo field);
        protected abstract void AppendFullName(StringBuilder sb, MethodBase method, bool ignoreParameterNames = false);

        protected abstract void AppendDecleration(StringBuilder sb, Type type);
        protected abstract void AppendDecleration(StringBuilder sb, PropertyInfo property);
        protected abstract void AppendDecleration(StringBuilder sb, EventInfo anEvent);
        protected abstract void AppendDecleration(StringBuilder sb, FieldInfo field);
        protected abstract void AppendDecleration(StringBuilder sb, MethodBase method);

        protected abstract void AppendAttribute(StringBuilder sb, CustomAttributeData attributeData);
        protected abstract void AppendLiteral(StringBuilder sb, object value);

        protected const string LineBreak = "\n";
        protected const string LineBreakAndIndent = LineBreak + "    ";

        protected static readonly HashSet<string> SpecialAttributeNamespaces = new HashSet<string>
        {
            "System.Runtime.InteropServices",
            "System.Runtime.CompilerServices",
        };

        #endregion
    }
}
