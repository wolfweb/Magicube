using Magicube.Web.UI;

namespace Magicube.Web.UI.Liquid.Models {
    public class LiquidWidgetModel : IWidget {
        public string Name     { get; set; }
        public string Content  { get; set; }
        public string Settings { get; set; }
    }
}
