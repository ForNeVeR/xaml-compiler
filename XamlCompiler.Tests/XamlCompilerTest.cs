using System;
using System.IO;
using Portable.Xaml;
using XamlCompiler.Tests.TestClasses;
using Xunit;

namespace XamlCompiler.Tests
{
    public class XamlCompilerTest
    {
        [Fact]
        public void ObjectWriterGeneratesInstanceOfRightType()
        {
            const string xaml = @"<Window xmlns=""test.fornever.me""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        x:Class=""TestClass""
        StringProperty=""TestString""
        IntProperty=""42"">
    <Control x:Name=""ChildControl"" />
</Window>";

            var context = new XamlSchemaContext(new[] {typeof(Window).Assembly});
            using (var xamlReader = new StringReader(xaml))
            using (var reader = new XamlXmlReader(xamlReader, context))
            using (var writer = new XamlObjectWriter(context))
            {
                XamlServices.Transform(reader, writer);
                Assert.Equal(typeof(Window), writer.Result.GetType());
            }
        }
    }
}
