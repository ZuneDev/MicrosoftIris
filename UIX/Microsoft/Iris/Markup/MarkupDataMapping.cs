// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDataMapping
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

#if OPENZUNE
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
#endif

namespace Microsoft.Iris.Markup
{
    internal class MarkupDataMapping
    {
        private string _name;
        private MarkupDataMappingEntry[] _mappings;
        private MarkupDataTypeSchema _targetType;
        private string _provider;
        private object _assemblyDataProviderCookie;

        public MarkupDataMapping(string name) => _name = name;

        public MarkupDataTypeSchema TargetType
        {
            get => _targetType;
            set => _targetType = value;
        }

        public string Provider
        {
            get => _provider;
            set => _provider = value;
        }

        public MarkupDataMappingEntry[] Mappings
        {
            get => _mappings;
            set => _mappings = value;
        }

        public object AssemblyDataProviderCookie
        {
            get => _assemblyDataProviderCookie;
            set => _assemblyDataProviderCookie = value;
        }

        public override string ToString() => $"({_provider}, {_targetType})";

#if OPENZUNE
        /// <summary>
        /// Generates C# code for a plain-old CLR object (POCO) that can
        /// be used to serialize and deserialize the data this mapping
        /// represents.
        /// </summary>
        public string GenerateModelCode()
        {
            CompilationUnitSyntax cu = SyntaxFactory.CompilationUnit()
                .AddUsings
                (
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System"))
                );

            NamespaceDeclarationSyntax localNamespace = SyntaxFactory.NamespaceDeclaration(
                SyntaxFactory.IdentifierName("Zune.Xml"));

            ClassDeclarationSyntax modelClass = SyntaxFactory.ClassDeclaration(TargetType.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            System.Collections.Generic.List<MemberDeclarationSyntax> members = new(Mappings.Length);
            foreach (var mapping in Mappings)
            {
                var propertyType = SyntaxFactory.ParseTypeName(mapping.Property.PropertyType.Name);
                var property = SyntaxFactory.PropertyDeclaration(propertyType, mapping.Property.Name)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                if (mapping.Source != null)
                {
                    string elementName = mapping.Source[1..];
                    var elementNameAttr = SyntaxFactory.AttributeArgument(
                        SyntaxFactory.ParseExpression($"ElementName = {elementName}"));

                    var attributes = SyntaxFactory.AttributeList().AddAttributes
                    (
                        SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("XmlElement"),
                            SyntaxFactory.AttributeArgumentList().AddArguments(elementNameAttr))
                    );

                    property = property.AddAttributeLists(attributes);
                }

                members.Add(property);
            }
            modelClass = modelClass.AddMembers(members.ToArray());

            // Add to file root
            localNamespace = localNamespace.AddMembers(modelClass);
            cu = cu.AddMembers(localNamespace);
            
            // Format file
            AdhocWorkspace cw = new();
            OptionSet options = cw.Options;
            cw.Options.WithChangedOption(CSharpFormattingOptions.IndentBraces, true);
            SyntaxNode formattedNode = Formatter.Format(cu, cw, options);

            // Write to string
            using System.IO.StringWriter writer = new();
            formattedNode.WriteTo(writer);
            return writer.ToString();
        }
#endif
    }
}
