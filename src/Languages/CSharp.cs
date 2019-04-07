// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Document.Generator.Languages
{
    public class CSharp : Language
    {
        private const int MaxMethodLineSize = 100;

        public static CSharp Instance = new CSharp();

        public override string Name => "csharp";

        private CSharp() { }

        protected virtual string TypeCategoryOf(Type type)
        {
            if (type.IsDelegate())
                return "delegate";
            if (type.IsClass)
                return "class";
            if (type.IsInterface)
                return "interface";
            if (type.IsEnum)
                return "enum";
            if (type.IsValueType)
                return "struct";

            throw new ArgumentException($"Unexpected type: {type}");
        }

        protected virtual IEnumerable<string> AccessModifiersOf(Type type)
        {
            if (type.IsPublic || type.IsNestedPublic)
                yield return "public";
            if (type.IsNestedFamily || type.IsNestedFamORAssem)
                yield return "protected";
            if (type.IsNestedAssembly || type.IsNestedFamORAssem)
                yield return "internal";
            if (type.IsNotPublic)
                yield return "private";

            if (type.IsUnsafe())
                yield return "unsafe";

            if (type.IsSealed)
            {
                if (type.IsAbstract)
                    yield return "static";
                else if (type.IsClass)
                    yield return "sealed";
            }
            else if (type.IsAbstract && !type.IsInterface)
            {
                yield return "abstract";
            }
        }

        protected virtual IEnumerable<string> AccessModifiersOf(PropertyInfo property)
        {
            var accesors = property.GetAccessors();
            if (accesors.Length == 0)
                return Array.Empty<string>();

            var getter = accesors.FirstOrDefault(a => !typeof(void).Equals(a.ReturnType));
            return AccessModifiersOf(getter ?? accesors[0]);
        }

        protected virtual IEnumerable<string> AccessModifiersOf(EventInfo anEvent)
        {
            var removeModifiers = new HashSet<string>(AccessModifiersOf(anEvent.RemoveMethod));
            return AccessModifiersOf(anEvent.AddMethod).Where(removeModifiers.Contains);
        }

        protected virtual IEnumerable<string> AccessModifiersOf(FieldInfo field)
        {
            if (field.IsPublic)
                yield return "public";
            if (field.IsPrivate)
                yield return "private";
            if (field.IsFamily)
                yield return "protected";
            if (field.IsAssembly)
                yield return "internal";

            if (field.IsLiteral)
                yield return "const";
            else if (field.IsStatic)
                yield return "static";
            if (field.IsVolatile())
                yield return "volatile";
            if (field.IsInitOnly)
                yield return "readonly";
        }

        protected virtual IEnumerable<string> AccessModifiersOf(MethodBase method)
        {
            if (method.IsPublic)
                yield return "public";
            if (method.IsPrivate)
                yield return "private";
            if (method.IsFamily)
                yield return "protected";
            if (method.IsAssembly)
                yield return "internal";

            if (method.IsUnsafe())
                yield return "unsafe";
            if (method.IsStatic)
                yield return "static";
            if (method.IsFinal)
                yield return "sealed";

            if (method.IsAbstract)
                yield return "abstract";
            else if (method.IsVirtual)
            {
                if (method.IsOverride())
                    yield return "override";
                else
                    yield return "virtual";
            }
        }

        protected override void AppendFullName(StringBuilder sb, Type type, bool ignoreParentTypeName = false)
        {
            if (type.IsNullable())
            {
                AppendFullName(sb, Nullable.GetUnderlyingType(type), ignoreParentTypeName);
                sb.Append('?');
                return;
            }

            if (!ignoreParentTypeName && type.IsTypeDefinition && type.IsNested)
            {
                AppendFullName(sb, type.DeclaringType.GetGenericTypeDefinition());
                sb.Append('.');
            }

            if (SystemTypeNames.TryGetValue(type.Name, out var name))
                sb.Append(name);
            else
                sb.Append(type.Name);

            var i = type.Name.LastIndexOf('`');
            if (i != -1)
            {
                sb.Length -= type.Name.Length - i;
                AppendTypeParamameters(sb, type.GetGenericArguments());
            }
        }

        protected override void AppendFullName(StringBuilder sb, PropertyInfo property, bool ignoreParameterNames = false)
        {
            sb.Append(property.Name);

            var indexParameters = property.GetIndexParameters();
            if (indexParameters.Length != 0)
            {
                sb.Append('[');
                AppendParameters(sb, indexParameters, ignoreParameterNames);
                sb.Append(']');
            }
        }

        protected override void AppendFullName(StringBuilder sb, EventInfo anEvent)
        {
            sb.Append(anEvent.Name);
        }

        protected override void AppendFullName(StringBuilder sb, FieldInfo field)
        {
            sb.Append(field.Name);
        }

        protected override void AppendFullName(StringBuilder sb, MethodBase method, bool ignoreParameterNames = false)
        {
            if (method.IsConstructor)
            {
                AppendFullName(sb, method.DeclaringType);
            }
            else if (Operators.TryGetValue(method.Name, out var op))
            {
                if (method.IsTypeCastingOperator())
                {
                    sb.Append(op);
                    sb.Append(" operator ");
                    AppendFullName(sb, (method as MethodInfo).ReturnType);
                }
                else
                {
                    sb.Append("operator ");
                    sb.Append(op);
                }
            }
            else
            {
                sb.Append(method.Name);
            }

            if (method.IsGenericMethodDefinition)
                AppendTypeParamameters(sb, method.GetGenericArguments());

            sb.Append('(');
            if (method.IsExtension())
                sb.Append("this ");
            var offsets = AppendParameters(sb, method.GetParameters(), ignoreParameterNames);
            sb.Append(')');

            if (!ignoreParameterNames && sb.Length > MaxMethodLineSize)
            {
                if (method.IsExtension())
                    offsets[0] -= 5;
                sb.Insert(sb.Length - 1, LineBreak);
                for (var i = offsets.Length - 1; i >= 0; i--)
                {
                    if (sb[offsets[i]] == ' ')
                        sb.Remove(offsets[i], 1);
                    sb.Insert(offsets[i], LineBreakAndIndent);
                }
            }
        }

        protected override void AppendDecleration(StringBuilder sb, Type type)
        {
            AppendAttributes(sb, type.GetCustomeAttributesOrFailSilent());

            foreach (var modifier in AccessModifiersOf(type))
            {
                sb.Append(modifier);
                sb.Append(' ');
            }

            sb.Append(TypeCategoryOf(type));
            sb.Append(' ');

            AppendFullName(sb, type);
            AppendTypeExtends(sb, type);

            if (type.IsGenericTypeDefinition)
                AppendTypeConstrains(sb, type.GetGenericArguments(), sb.Length > MaxMethodLineSize ? LineBreak : LineBreakAndIndent);
        }

        protected override void AppendDecleration(StringBuilder sb, PropertyInfo property)
        {
            AppendAttributes(sb, property.GetCustomeAttributesOrFailSilent());

            var commonModifiers = AccessModifiersOf(property).ToList();
            foreach (var modifier in commonModifiers)
            {
                sb.Append(modifier);
                sb.Append(' ');
            }

            AppendFullName(sb, property.PropertyType);
            sb.Append(' ');

            AppendFullName(sb, property);

            AppendPropertyAccessors(sb, property, commonModifiers);

            if (property.Attributes.HasFlag(PropertyAttributes.HasDefault))
            {
                sb.Append(" = ");
                AppendConstant(sb, property.GetConstantValue(), property.PropertyType);
            }
        }

        protected override void AppendDecleration(StringBuilder sb, EventInfo anEvent)
        {
            AppendAttributes(sb, anEvent.GetCustomeAttributesOrFailSilent());

            foreach (var modifier in AccessModifiersOf(anEvent))
            {
                sb.Append(modifier);
                sb.Append(' ');
            }

            AppendFullName(sb, anEvent.EventHandlerType);
            sb.Append(' ');

            sb.Append(anEvent.Name);
        }


        protected override void AppendDecleration(StringBuilder sb, FieldInfo field)
        {
            AppendAttributes(sb, field.GetCustomeAttributesOrFailSilent());

            foreach (var modifier in AccessModifiersOf(field))
            {
                sb.Append(modifier);
                sb.Append(' ');
            }

            AppendFullName(sb, field.FieldType);
            sb.Append(' ');

            sb.Append(field.Name);
        }

        protected override void AppendDecleration(StringBuilder sb, MethodBase method)
        {
            AppendAttributes(sb, method.GetCustomeAttributesOrFailSilent());

            if (!method.DeclaringType.IsInterface)
            {
                foreach (var modifier in AccessModifiersOf(method))
                {
                    sb.Append(modifier);
                    sb.Append(' ');
                }
            }

            if (!method.IsTypeCastingOperator() && method is MethodInfo info)
            {
                AppendFullName(sb, info.ReturnType);
                sb.Append(' ');
            }

            AppendFullName(sb, method);

            if (method.IsGenericMethod)
                AppendTypeConstrains(sb, method.GetGenericArguments());
        }

        protected virtual void AppendTypeConstrains(StringBuilder sb, Type[] typeParameters, string prefix = LineBreakAndIndent)
        {
            for (var i = 0; i < typeParameters.Length; i++)
                AppendTypeParameterConstrains(sb, typeParameters[i], prefix);
        }

        protected virtual void AppendTypeParameterConstrains(StringBuilder sb, Type typeParameter, string prefix)
        {
            var typeConstrains = typeParameter.GetGenericParameterConstraints();
            var specialConstrains = typeParameter.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;

            if (typeConstrains.Length == 0 && specialConstrains == GenericParameterAttributes.None)
                return;

            sb.Append(prefix);
            sb.Append("where ");
            AppendFullName(sb, typeParameter);
            sb.Append(" : ");

            if (specialConstrains.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
                sb.Append("class, ");
            else if (specialConstrains.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
            {
                sb.Append("struct, ");
                specialConstrains &= ~GenericParameterAttributes.DefaultConstructorConstraint;
            }

            for (var i = 0; i < typeConstrains.Length; i++)
            {
                var typeConstrain = typeConstrains[i];
                if (typeConstrain != typeof(object) && typeConstrain != typeof(ValueType))
                {
                    AppendFullName(sb, typeConstrain);
                    sb.Append(", ");
                }
            }

            if (specialConstrains.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
                sb.Append("new(), ");

            sb.Length -= 2;
        }

        protected virtual void AppendTypeExtends(StringBuilder sb, Type type)
        {
            var anyAppended = false;

            if (type.IsDerivedExplicitly())
            {
                sb.Append(" : ");
                AppendFullName(sb, type.BaseType);
                anyAppended = true;
            }

            if (!type.IsEnum)
            {
                foreach (var theInterface in type.GetInterfaces())
                {
                    sb.Append(anyAppended ? ", " : " : ");
                    AppendFullName(sb, theInterface);
                    anyAppended = true;
                }
            }
        }

        protected virtual void AppendPropertyAccessors(StringBuilder sb, PropertyInfo property, ICollection<string> modifiers)
        {
            var accessors = property.GetAccessors();
            if (accessors.Length != 0)
            {
                sb.Append(" { ");
                foreach (var accessor in accessors)
                {
                    foreach (var modifier in AccessModifiersOf(accessor))
                    {
                        if (!modifiers.Contains(modifier))
                        {
                            sb.Append(modifier);
                            sb.Append(' ');
                        }
                    }
                    sb.Append(typeof(void).Equals(accessor.ReturnType) ? "set" : "get");
                    sb.Append("; ");
                }
                sb.Append("}");
            }
        }

        protected virtual void AppendTypeParamameters(StringBuilder sb, Type[] typeParameters)
        {
            for (var i = 0; i < typeParameters.Length; i++)
            {
                sb.Append(i == 0 ? "<" : ", ");
                AppendTypeParamameter(sb, typeParameters[i]);
            }
            if (typeParameters.Length != 0)
                sb.Append(">");
        }

        protected virtual void AppendTypeParamameter(StringBuilder sb, Type typeParameter)
        {
            if (typeParameter.IsGenericTypeParameter)
            {
                var variance = typeParameter.GenericParameterAttributes & GenericParameterAttributes.VarianceMask;
                if (variance != GenericParameterAttributes.None)
                {
                    if (variance.HasFlag(GenericParameterAttributes.Contravariant))
                        sb.Append("in ");
                    else if (variance.HasFlag(GenericParameterAttributes.Covariant))
                        sb.Append("out ");
                }
            }
            AppendFullName(sb, typeParameter);
        }

        protected virtual int[] AppendParameters(StringBuilder sb, ParameterInfo[] parameters, bool ignoreDetails = false)
        {
            var offsets = new int[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                if (i != 0)
                    sb.Append(", ");
                offsets[i] = sb.Length;
                if (ignoreDetails)
                    AppendFullName(sb, parameters[i].ParameterType);
                else
                    AppendParameter(sb, parameters[i]);
            }
            return offsets;
        }

        protected virtual void AppendParameter(StringBuilder sb, ParameterInfo parameter)
        {
            AppendAttributes(sb, parameter.CustomAttributes, " ");

            if (parameter.ParameterType.IsByRef)
            {
                if (parameter.IsIn)
                    sb.Append("in ");
                else if (parameter.IsOut)
                    sb.Append("out ");
                else
                    sb.Append("ref ");
            }

            AppendFullName(sb, parameter.ParameterType);
            sb.Append(' ');

            sb.Append(parameter.Name);

            if (parameter.HasDefaultValue)
            {
                sb.Append(" = ");
                AppendConstant(sb, parameter.DefaultValue, parameter.ParameterType);
            }
        }

        protected override void AppendAttribute(StringBuilder sb, CustomAttributeData attributeData)
        {
            sb.Append('[');

            AppendFullName(sb, attributeData.AttributeType);
            if (attributeData.AttributeType.Name.EndsWith("Attribute"))
                sb.Length -= 9;

            var hasAnyParam = false;

            foreach (var argument in attributeData.ConstructorArguments)
            {
                if (hasAnyParam)
                {
                    sb.Append(", ");
                }
                else
                {
                    hasAnyParam = true;
                    sb.Append('(');
                }
                AppendAttributeArgument(sb, argument);
            }

            foreach (var argument in attributeData.NamedArguments)
            {
                if (hasAnyParam)
                {
                    sb.Append(", ");
                }
                else
                {
                    hasAnyParam = true;
                    sb.Append('(');
                }
                AppendAttributeArgument(sb, argument);
            }

            if (hasAnyParam)
                sb.Append(')');

            sb.Append(']');
        }

        protected override void AppendLiteral(StringBuilder sb, object value)
        {
            switch (value)
            {
                case null:
                    sb.Append("null");
                    break;
                case bool booleanLiteral:
                    sb.Append(booleanLiteral ? "true" : "false");
                    break;
                case float floatLiteral:
                    sb.Append(floatLiteral.ToString(null, CultureInfo.InvariantCulture));
                    sb.Append('f');
                    break;
                case double doubleLiteral:
                    sb.Append(doubleLiteral.ToString(null, CultureInfo.InvariantCulture));
                    break;
                case decimal decimalLiteral:
                    sb.Append(decimalLiteral.ToString(null, CultureInfo.InvariantCulture));
                    sb.Append('m');
                    break;
                case long longLiteral:
                    sb.Append(longLiteral.ToString(null, CultureInfo.InvariantCulture));
                    sb.Append('L');
                    break;
                case ulong unsignedLongLiteral:
                    sb.Append(unsignedLongLiteral.ToString(null, CultureInfo.InvariantCulture));
                    sb.Append("uL");
                    break;
                case int integerLiteral:
                    sb.Append(integerLiteral.ToString(null, CultureInfo.InvariantCulture));
                    break;
                case uint unsignedIntegerLiteral:
                    sb.Append(unsignedIntegerLiteral.ToString(null, CultureInfo.InvariantCulture));
                    if (unsignedIntegerLiteral > int.MaxValue)
                        sb.Append("u");
                    break;
                case short shortIntegerLiteral:
                    sb.Append(shortIntegerLiteral.ToString(null, CultureInfo.InvariantCulture));
                    break;
                case ushort unsignedShortLiteral:
                    sb.Append(unsignedShortLiteral.ToString(null, CultureInfo.InvariantCulture));
                    if (unsignedShortLiteral > short.MaxValue)
                        sb.Append("u");
                    break;
                case byte byteLiteral:
                    sb.Append(byteLiteral.ToString(null, CultureInfo.InvariantCulture));
                    break;
                case sbyte signeLiterald:
                    sb.Append(signeLiterald.ToString(null, CultureInfo.InvariantCulture));
                    break;
                case char charLiteral:
                    sb.Append('\'');
                    AppendEscapedChar(sb, charLiteral);
                    sb.Append('\'');
                    break;
                case string stringLiteral:
                    sb.Append('"');
                    foreach (var c in stringLiteral)
                        AppendEscapedChar(sb, c, '"');
                    sb.Append('"');
                    break;
                default:
                    throw new ArgumentException($"Unexpected literal value: {value}");
            }
        }

        protected virtual void AppendEscapedChar(StringBuilder sb, char c, char quote = '\'')
        {
            if (c == quote)
            {
                sb.Append('\\');
                sb.Append(quote);
                return;
            }

            const string specialChars = "\\\n\r\t\a\b\f\0";
            const string specialEscapedChars = @"\nrtabf0";
            var i = specialChars.IndexOf(c);
            if (i != -1)
            {
                sb.Append('\\');
                sb.Append(specialEscapedChars[i]);
                return;
            }

            if (c != ' ' && char.IsControl(c) || char.IsWhiteSpace(c))
            {
                if (c < 255)
                {
                    sb.Append("\\x");
                    sb.Append(((int)c).ToString("X2"));
                }
                else
                {
                    sb.Append("\\u");
                    sb.Append(((int)c).ToString("X4"));
                }
                return;
            }

            sb.Append(c);
        }

        protected static readonly IReadOnlyDictionary<string, string> Operators = new Dictionary<string, string>
        {
            ["op_Equality"] = "==",
            ["op_Inequality"] = "!=",
            ["op_GreaterThan"] = ">",
            ["op_LessThan"] = "<",
            ["op_GreaterThanOrEqual"] = ">=",
            ["op_LessThanOrEqual"] = "<=",
            ["op_BitwiseAnd"] = "&",
            ["op_BitwiseOr"] = "|",
            ["op_Addition"] = "+",
            ["op_Subtraction"] = "-",
            ["op_Division"] = "/",
            ["op_Modulus"] = "%",
            ["op_Multiply"] = "*",
            ["op_LeftShift"] = "<<",
            ["op_RightShift"] = ">>",
            ["op_ExclusiveOr"] = "^",
            ["op_UnaryNegation"] = "-",
            ["op_UnaryPlus"] = "+",
            ["op_LogicalNot"] = "!",
            ["op_OnesComplement"] = "~",
            ["op_False"] = "false",
            ["op_True"] = "true",
            ["op_Increment"] = "++",
            ["op_Decrement"] = "--",
            ["op_Implicit"] = "implicit",
            ["op_Explicit"] = "explicit",
        };

        protected static IReadOnlyDictionary<string, string> SystemTypeNames = new Dictionary<string, string>
        {
            ["Void"] = "void",
            ["Boolean"] = "bool",
            ["Char"] = "char",
            ["String"] = "string",
            ["Byte"] = "byte",
            ["SByte"] = "sbyte",
            ["UInt16"] = "ushort",
            ["Int16"] = "short",
            ["UInt32"] = "uint",
            ["Int32"] = "int",
            ["UInt64"] = "ulong",
            ["Int64"] = "long",
            ["Single"] = "float",
            ["Double"] = "double",
            ["Decimal"] = "decimal",
            ["Object"] = "object",
        };
    }
}
