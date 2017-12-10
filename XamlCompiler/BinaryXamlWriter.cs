using System;
using System.Collections.Generic;
using System.IO;
using Portable.Xaml;

namespace XamlCompiler
{
    public class BinaryXamlWriter : XamlWriter
    {
        private readonly Compiler.XamlCompiler _compiler;

        public BinaryXamlWriter(
            XamlSchemaContext context,
            string name,
            Version version,
            IEnumerable<string> referencedAssemblyNames)
        {
            SchemaContext = context;
            _compiler = new Compiler.XamlCompiler(name, version, referencedAssemblyNames);
        }

        public void WriteAssembly(Stream output) => _compiler.WriteAssembly(output);

        public override XamlSchemaContext SchemaContext { get; }

        public override void WriteEndMember()
        {
        }

        public override void WriteEndObject() => _compiler.OnEndObject();

        public override void WriteGetObject()
        {
            throw new NotImplementedException();
        }

        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
        }

        public override void WriteStartMember(XamlMember xamlMember)
        {
            switch (xamlMember)
            {
                case var x when x == XamlLanguage.Base:
                    break;
                case var x when x == XamlLanguage.Class:
                    _compiler.OnClassAttribute();
                    break;
                default:
                    throw new NotSupportedException($"XAML member {xamlMember} is not supported by the compiler");
            }
        }

        public override void WriteStartObject(XamlType type) => _compiler.OnStartObject(type);
        public override void WriteValue(object value) => _compiler.OnAttributeValue(value);
    }
}
