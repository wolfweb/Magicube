using Magicube.Data.Abstractions.ViewModel;
using System;

namespace Magicube.Web.UI.TagHelpers {
    public class MagicubeTagHelperContext {
        public object                     Model      { get; protected set; }
        public Type                       ModelType  { get; protected set; }
        public PropertyComponentContext[] Properties { get; protected set; }
    }
}
