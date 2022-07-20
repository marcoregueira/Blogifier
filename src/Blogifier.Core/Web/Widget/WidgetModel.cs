namespace Blogifier.Core.Web
{
    public class WidgetModel
    {
        public string Name { get; set; }
        public object Model { get; set; }

        public WidgetModel() { }

        public WidgetModel(string name, object model)
        {
            Name = name;
            Model = model;
        }
    }
}
