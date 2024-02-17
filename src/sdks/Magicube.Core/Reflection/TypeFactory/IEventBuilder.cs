namespace Magicube.Core.Reflection {
    using System.Reflection;
    using System.Reflection.Emit;

    public interface IEventBuilder {
        EventAttributes EventAttributes { get; }

        IEventBuilder SpecialName();

        IEventBuilder RTSpecialName();

        EventBuilder Define();
    }
}