using Portable.Xaml.Markup;

namespace XamlCompiler.Tests.TestClasses
{
    [ContentProperty("Child")]
    public class Window
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
        public Control Child { get; set; }
    }
}
