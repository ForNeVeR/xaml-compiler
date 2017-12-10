using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Portable.Xaml;
using XamlCompiler.Tests.TestClasses;
using Xunit;

namespace XamlCompiler.Tests
{
    public class XamlCompilerTest
    {
        private static T ActivateInstanceFromXaml<T>(string xaml, string typeName)
        {
            var referencedAssembly = typeof(T).Assembly;
            var path = new[] {new Uri(referencedAssembly.CodeBase).LocalPath};
            var context = new XamlSchemaContext();
            using (var xamlReader = new StringReader(xaml))
            using (var reader = new XamlXmlReader(xamlReader, context))
            using (var writer = new BinaryXamlWriter(context, "test", new Version(), path))
            using (var stream = new MemoryStream())
            {
                XamlServices.Transform(reader, writer);

                writer.WriteAssembly(stream);
                var bytes = stream.ToArray();
                var assembly = Assembly.Load(bytes);
                var type = assembly.ExportedTypes.Single(t => t.Name == typeName);
                return (T) Activator.CreateInstance(type);
            }
        }

        [Fact]
        public void SimpleClassIsDefined()
        {
            const string xaml = @"<Window xmlns=""test.fornever.me""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        x:Class=""TestClass"" />";

            var window = ActivateInstanceFromXaml<Window>(xaml, "TestClass");
            Assert.Equal("TestClass", window.GetType().Name);
        }

        [Fact(Skip = "Not yet implemented")]
        public void PropertiesAreSetFromXaml()
        {
            const string xaml = @"<Window xmlns=""test.fornever.me""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        x:Class=""TestClass""
        StringProperty=""TestString""
        IntProperty=""42"">
</Window>";

            var window = ActivateInstanceFromXaml<Window>(xaml, "TestClass");
            Assert.Equal("TestString", window.StringProperty);
            Assert.Equal(42, window.IntProperty);
        }

        [Fact(Skip = "Not yet implemented")]
        public void ChildIsCreatedFromXaml()
        {
            const string xaml = @"<Window xmlns=""test.fornever.me""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        x:Class=""TestClass""
        StringProperty=""TestString""
        IntProperty=""42"">
    <Control x:Name=""ChildControl"" />
</Window>";

            var window = ActivateInstanceFromXaml<Window>(xaml, "TestClass");
            var child = window.Child;
            Assert.NotNull(child);
        }
    }
}
