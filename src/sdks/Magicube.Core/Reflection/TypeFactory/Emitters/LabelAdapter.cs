namespace Magicube.Core.Reflection.Emitters {
    internal class LabelAdapter : ILabel, IAdaptedLabel {
        internal LabelAdapter(string name, object label = null) {
            Name  = name;
            Label = label;
        }

        public object Label { get; set; }

        public string Name { get; set; }
    }
}