using Document.Generator.Formatters;
using Document.Generator.Helpers;
using Document.Generator.Languages;
using Document.Generator.Models.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Document.Generator.Models.Clr
{
    public abstract class ClrMember : ClrItem, ICRef, IEquatable<ClrMember>
    {
        public ClrMember(ClrType owner, MemberInfo memberInfo)
        {
            Owner = owner;
            Info = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));
            CRef = Info.GetCRef();
            Name = Info.GetDisplayName();
        }

        public ClrType Owner { get; }

        public MemberInfo Info { get; }

        public virtual string CRef { get; }

        public override string Name { get; }

        public abstract MemberCategory Kind { get; }

        public virtual string BaseName => Info.Name;

        public override string Title => $"{Owner.Name}.{Name} {Kind}";

        public virtual string GetSyntax(Language language) => language.DeclerationOf(Info);

        public override string GetDocFile(OutputContext context)
        {
            return context.ToFileName($"{Owner.Info.FullName}.{Info.Name}");
        }

        public override void Write(DocumentFormatter output, OutputContext context)
        {
            output.Header(1, Title);
            WriteInfoBox(output, context);
            WriteSummary(output, context);

            var syntax = GetSyntax(context.Language);
            if (!string.IsNullOrEmpty(syntax))
            {
                output.Header(2, "Syntax");
                output.Code(syntax, context.Language.Name);
            }

            WriteDetails(2, output, context);
        }

        public override void WriteSummaryLine(DocumentFormatter output, OutputContext context)
        {
            var summary = context.Document.Of(this)?.Summaries.FirstOrDefault();
            if (summary != null)
                output.Xml(summary);
            else
                Log.WarnMisisngDoc(this);
        }

        public virtual void WriteSummary(DocumentFormatter output, OutputContext context)
        {
            output.Xml(context.Document.Of(this)?.Summaries);
        }

        public virtual void WriteDetails(int level, DocumentFormatter output, OutputContext context)
        {
            var references = GetReferences().Distinct();
            if (context.Document.Members.TryGetValue(CRef, out var documentation))
            {
                WriteSection(output, level, "Events", "Event", documentation.Events);
                WriteSection(output, level, "Exceptions", "Exception", documentation.Exceptions);
                WriteSection(output, level, "Examples", documentation.Examples);
                WriteSection(output, level, "Remarks", documentation.Remarks);
                WriteSection(output, level, "Security", "Permission", documentation.Permissions);
                WriteSection(output, level, "See Also", references.Concat(documentation.SeeAlso));
                WriteSection(output, level, "Revision History", documentation.RevisionHistories);
            }
            else
            {
                WriteSection(output, level, "See Also", references);
            }
        }

        public virtual void WriteTypeParametersSection(int level, DocumentFormatter output, OutputContext context, IEnumerable<Type> typeParameters)
        {
            if (typeParameters == null || !typeParameters.Any())
                return;

            var doc = context.Document.Of(this);

            output.Header(level, "Type Parameters");
            output.DefinitionList(typeParameters,
                typeParameter => output.Text(typeParameter.Name, TextStyles.Teletype),
                typeParameter => output.Section(() => 
                {
                    if (WriteTypeParamDoc(doc, typeParameter.Name))
                        return;

                    if (!typeParameter.IsGenericMethodParameter)
                    {
                        for (var parent = Owner; parent != null; parent = parent.Owner)
                        {
                            if (WriteTypeParamDoc(context.Document.Of(parent), typeParameter.Name))
                                return;
                        }
                    }

                    Log.WarnMisisngTypeParameterDoc(this, typeParameter.Name);
                }));

            bool WriteTypeParamDoc(XmlMember ownerDoc, string paramName)
            {
                if (ownerDoc != null && ownerDoc.TypeParameters.TryGetValue(paramName, out var paramDoc))
                {
                    output.Xml(paramDoc);
                    return true;
                }

                return false;
            }
        }

        public virtual void WriteParametersSection(int level, DocumentFormatter output, OutputContext context, IEnumerable<ParameterInfo> parameters)
        {
            if (parameters == null || !parameters.Any())
                return;

            var doc = context.Document.Of(this);

            output.Header(level, "Parameters");
            output.DefinitionList(parameters,
                parameter =>
                {
                    output.Text(parameter.Name, TextStyles.Teletype);
                    output.Text(": ");
                    if (parameter.ParameterType.IsGenericParameter)
                        output.Text(parameter.ParameterType.GetDisplayName(), TextStyles.Emphasize);
                    else
                        output.LinkCRef(parameter.ParameterType.GetCRef(), parameter.ParameterType.GetDisplayName());
                },
                parameter => output.Section(() => doc?.Parameters.For(parameter.Name, output.Xml, name => Log.WarnMisisngParameterDoc(this, name))));
        }

        public virtual void WriteReturnValueSection(int level, DocumentFormatter output, OutputContext context, Type returnType)
        {
            if (returnType == null || returnType == typeof(void))
                return;

            var doc = context.Document.Of(this);

            if (doc != null && doc.Returns.Count == 0)
                Log.WarnMisisngReturnDoc(this);

            output.Header(level, "Return Value");
            output.DefinitionList(new[] { returnType },
                type => output.LinkCRef(type.GetCRef(), type.GetDisplayName()),
                type => output.Xml(doc?.Returns));
        }

        public virtual void WriteInfoBox(DocumentFormatter output, OutputContext context)
        {
            output.Quote(GetInfoBoxWriters(context), info =>
            {
                output.Text(info.Label);
                output.Text(": ");
                info.Writer(output);
            });
        }

        protected virtual IEnumerable<(string Label, Action<DocumentFormatter> Writer)> GetInfoBoxWriters(OutputContext context)
        {
            return Owner.GetInfoBoxWriters(context).Take(2);
        }

        protected virtual IEnumerable<ICRef> GetReferences()
        {
            foreach (var reference in Owner.GetReferences())
                yield return reference;
            yield return Owner;
        }

        public bool Equals(ClrMember other)
        {
            return (other != null) && (other.CRef == CRef);
        }

    }
}
