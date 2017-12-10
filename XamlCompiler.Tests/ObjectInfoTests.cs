using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using XamlCompiler.Compiler;
using Xunit;

namespace XamlCompiler.Tests
{
    public class ObjectInfoTests
    {
        private static TypeDefinition CheckTypeCreation(string @namespace, string @class)
        {
            var typeName = string.IsNullOrEmpty(@namespace) ? @class : $"{@namespace}.{@class}";
            var module = ModuleDefinition.CreateModule("foobar", ModuleKind.Dll);
            var baseType = module.ImportReference(typeof(object));
            var objectInfo = new ObjectInfo(baseType) {TypeName = typeName};
            objectInfo.AddTypeDefinition(module);

            return Assert.Single(module.Types, t => t.Namespace == @namespace && t.Name == @class);
        }

        [Fact] public void SimpleTypeDefinitionAddedToContext() => CheckTypeCreation("Foo", "Bar");
        [Fact] public void NamespacelessDefinitionAddedToContext() => CheckTypeCreation("", "Bar");
        [Fact] public void ComplexNamespaceDefinitionAddedToContext() => CheckTypeCreation("Foo.Baz", "Bar");

        [Fact]
        public void GeneratedTypeConstructorCallsBaseClassConstructor()
        {
            var typeDefinition = CheckTypeCreation("Foo", "Bar");
            var ctor = Assert.Single(typeDefinition.GetConstructors());
            var ilProcessor = ctor.Body.GetILProcessor();
            var baseConstructor = typeDefinition.Module.ImportReference(typeof(object).GetConstructor(new Type[0]));

            Assert.Collection(
                ilProcessor.Body.Instructions,
                i => Assert.Equal(OpCodes.Ldarg_0, i.OpCode),
                i => Assert.Equal(baseConstructor.FullName, ((MethodReference)i.Operand).FullName),
                i => Assert.Equal(OpCodes.Ret, i.OpCode));
        }
    }
}
