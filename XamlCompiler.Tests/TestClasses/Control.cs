using Portable.Xaml.Markup;

namespace XamlCompiler.Tests.TestClasses
{
    [RuntimeNameProperty("Name")]
    public class Control
    {
        public string Name { get; set; }
    }
}
