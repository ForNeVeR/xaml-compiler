using Mono.Cecil;

namespace XamlCompiler.Compiler
{
    internal class XmlnsDefinition
    {
        public string Xmlns { get; }
        public AssemblyDefinition Assembly { get; }
        public string Namespace { get; }

        public XmlnsDefinition(string xmlns, AssemblyDefinition assembly, string ns)
        {
            Xmlns = xmlns;
            Assembly = assembly;
            Namespace = ns;
        }
    }
}
