using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeValue.SuiteValue.UI.Metro.Controls
{
    public class FlexableGridView : GridView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            dynamic viewModel = item;

            element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, viewModel.Width);
            element.SetValue(VariableSizedWrapGrid.RowSpanProperty, viewModel.Height);
            element.SetValue(VerticalContentAlignmentProperty, VerticalAlignment.Stretch);
            element.SetValue(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch); 

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
