using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Portable.Xaml.Markup;

namespace XamlCompiler.Compiler
{
    internal class XamlTypeResolver
    {
        private readonly Dictionary<string, XmlnsDefinition> _namespaces;

        private XamlTypeResolver(IEnumerable<XmlnsDefinition> namespaces)
        {
            _namespaces = namespaces.ToDictionary(d => d.Xmlns);
        }

        public static XamlTypeResolver FromAssemblies(IEnumerable<string> paths)
        {
            var attributes = paths
                .Select(AssemblyDefinition.ReadAssembly)
                .SelectMany(assembly => assembly.CustomAttributes
                    .Where(a => a.AttributeType.FullName == typeof(XmlnsDefinitionAttribute).FullName)
                    .Select(a => new {Assembly = assembly, Attribute = a}));
            var definitions = attributes.Select(x =>
            {
                var arguments = x.Attribute.ConstructorArguments;
                var xmlns = (string) arguments[0].Value;
                var ns = (string) arguments[1].Value;
                return new XmlnsDefinition(xmlns, x.Assembly, ns);
            });

            return new XamlTypeResolver(definitions);
        }

        public TypeReference Resolve(string xmlns, string typeName)
        {
            // TODO[F]: Add type cache.
            var definition = _namespaces[xmlns];
            var types = definition.Assembly.Modules.SelectMany(m => m.Types);
            return types.Single(t => t.FullName == $"{definition.Namespace}.{typeName}");
        }
    }
}
