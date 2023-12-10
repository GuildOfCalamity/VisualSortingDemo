using Microsoft.UI.Xaml.Markup;

namespace VisualSortingItems
{
    // This idea was copied from:
    // https://devblogs.microsoft.com/ifdef-windows/use-a-custom-resource-markup-extension-to-succeed-at-ui-string-globalization/

    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class ResourceString : MarkupExtension
    {
        public string Name { get; set; } = string.Empty;

        protected override object ProvideValue()
        {
            string value = AppResourceManager.GetInstance.GetString(Name);
            return value;
        }
    }
}
