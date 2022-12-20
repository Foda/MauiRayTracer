using RayTrace.Materials;

namespace MauiTracer
{
    public class MaterialsDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SolidColorTemplate { get; set; }
        public DataTemplate CheckerboardTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is SolidColorTexture)
                return SolidColorTemplate;
            if (item is Checkerboard)
                return CheckerboardTemplate;

            return SolidColorTemplate;
        }
    }

}
