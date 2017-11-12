using System;
using System.Diagnostics;
using System.IO;
using Mono.Cecil;
using Portable.Xaml;

namespace XamlCompiler
{
    public class BinaryXamlWriter : XamlWriter
    {
        private readonly ModuleDefinition _module;

        private XamlType _baseType;
        private TypeDefinition _currentType;

        public BinaryXamlWriter(XamlSchemaContext context, string name, Version version)
        {
            SchemaContext = context;

            var assemblyName = new AssemblyNameDefinition(name, version);
            _module = CreateModule(assemblyName);
        }

        public void WriteAssembly(Stream output)
        {
            throw new NotImplementedException();
        }

        private static ModuleDefinition CreateModule(AssemblyNameDefinition assemblyName)
        {
            var assembly = AssemblyDefinition.CreateAssembly(
                assemblyName,
                assemblyName.Name,
                ModuleKind.Dll);
            return assembly.MainModule;
        }

        public override XamlSchemaContext SchemaContext { get; }

        public override void WriteEndMember()
        {
            throw new NotImplementedException();
        }

        public override void WriteEndObject()
        {
            throw new NotImplementedException();
        }

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
                case var x when x == XamlLanguage.Class:
                    Debug.Assert(_currentType == null);
                    _currentType = new TypeDefinition("", "", TypeAttributes.Class);
                    break;
                default:
                    throw new NotSupportedException($"XAML member {xamlMember} is not supported by the compiler");
            }
        }

        public override void WriteStartObject(XamlType type)
        {
            Debug.Assert(_baseType == null);
            _baseType = type;
        }

        public override void WriteValue(object value)
        {
            throw new NotImplementedException();
        }
    }
}
