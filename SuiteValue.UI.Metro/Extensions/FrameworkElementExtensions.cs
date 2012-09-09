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
                if(currentElement.Resources.TryGetValue(resourceKey,out resource))
                {
                    return resource;
                }
                currentElement = currentElement.Parent as FrameworkElement;
                return currentElement.TryFindResource(resourceKey);
            }
            return Application.Current.Resources.TryGetValue(resourceKey, out resource)?resource:null;
        }
    }
}
