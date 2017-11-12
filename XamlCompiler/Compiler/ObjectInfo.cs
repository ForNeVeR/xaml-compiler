using System;
using Mono.Cecil;
using Portable.Xaml;

namespace XamlCompiler.Compiler
{
    internal class ObjectInfo
    {
        public XamlType BaseType { get; set; }
        public string TypeName { get; set; }

        public TypeDefinition CreateTypeDefinition()
        {
            var baseType = BaseType.UnderlyingType;
            var baseTypeAssembly = baseType.Assembly;
            var baseTypeModule = ModuleDefinition.ReadModule(baseTypeAssembly.Location);
            var baseTypeReference =
                new TypeReference(baseType.Namespace, baseType.Name, baseTypeModule, baseTypeModule);

            var (@namespace, type) = ParseTypeName(TypeName);
            const TypeAttributes attributes = TypeAttributes.Public | TypeAttributes.Class;
            var definition = new TypeDefinition(@namespace, type, attributes, baseTypeReference);

            return definition;
        }

        private (string @namespace, string type) ParseTypeName(string typeName)
        {
            if (typeName == null)
            {
                throw new NotSupportedException($"Type derived from {BaseType} has no name defined");
            }

            var index = typeName.IndexOf(".", StringComparison.InvariantCulture);
            if (index == -1)
            {
                return (null, typeName);
            }

            return (typeName.Substring(0, index), typeName.Substring(index + 1));
        }
    }
}
