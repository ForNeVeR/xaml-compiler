using System;
using System.Reflection;
using Portable.Xaml.Markup;
using XamlCompiler.Compiler;
using XamlCompiler.Tests.TestClasses;
using Xunit;

namespace XamlCompiler.Tests
{
    public class XamlTypeResolverTests
    {
        [Fact]
        public void TypeResolverResolvesAType()
        {
            var referencedAssembly = typeof(Window).Assembly;
            var path = new[] {new Uri(referencedAssembly.CodeBase).LocalPath};
            var resolver = XamlTypeResolver.FromAssemblies(path);

            var attribute = (XmlnsDefinitionAttribute) referencedAssembly.GetCustomAttribute(
                typeof(XmlnsDefinitionAttribute));
            Assert.Equal(typeof(Window).Namespace, attribute.ClrNamespace);
            var type = resolver.Resolve(attribute.XmlNamespace, nameof(Window));

            Assert.Equal(typeof(Window).FullName, type.FullName);
        }
    }
}
