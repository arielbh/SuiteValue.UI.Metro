using Windows.UI.Xaml;

namespace CodeValue.SuiteValue.UI.Metro.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static object TryFindResource(this FrameworkElement element, object resourceKey)
        {
            object resource;
            var currentElement = element;

            while (currentElement != null)
            {
                resource = currentElement.TryFindResource(resourceKey);
                if (resource != null)
                {
                    return resource;
                }

                currentElement = currentElement.Parent as FrameworkElement;
            }

            Application.Current.Resources.TryGetValue(resourceKey, out resource);
            return resource;
        }
    }
}
