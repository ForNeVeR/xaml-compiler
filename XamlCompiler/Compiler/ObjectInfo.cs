using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Portable.Xaml;

namespace XamlCompiler.Compiler
{
    internal class ObjectInfo
    {
        public XamlType BaseType { get; }
        public string TypeName { get; set; }

        public ObjectInfo(XamlType baseType)
        {
            BaseType = baseType;
        }

        public void AddTypeDefinition(ModuleDefinition module)
        {
            var baseType = BaseType.UnderlyingType;
            var baseTypeReference = module.ImportReference(baseType);

            var (@namespace, type) = ParseTypeName(TypeName);
            const TypeAttributes attributes = TypeAttributes.Public | TypeAttributes.Class;
            var definition = new TypeDefinition(@namespace, type, attributes, baseTypeReference);
            AddConstructor(module, definition);

            module.Types.Add(definition);
        }

        private void AddConstructor(ModuleDefinition module, TypeDefinition definition)
        {
            const MethodAttributes attributes = MethodAttributes.Public
                                                | MethodAttributes.HideBySig
                                                | MethodAttributes.SpecialName
                                                | MethodAttributes.RTSpecialName;
            var ctor = new MethodDefinition(".ctor", attributes, module.TypeSystem.Void);
            var ilProcessor = ctor.Body.GetILProcessor();

            var baseTypeConstructor = definition.BaseType.Resolve().GetConstructors()
                .Single(c => c.Parameters.Count == 0);
            ilProcessor.Append(ilProcessor.Create(OpCodes.Ldarg_0));
            ilProcessor.Append(ilProcessor.Create(OpCodes.Call, module.ImportReference(baseTypeConstructor)));
            ilProcessor.Append(ilProcessor.Create(OpCodes.Ret));

            definition.Methods.Add(ctor);
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
