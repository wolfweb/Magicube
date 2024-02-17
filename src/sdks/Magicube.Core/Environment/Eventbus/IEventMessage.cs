namespace Magicube.Core.Environment.Eventbus {
    public interface IEventMessage {
        string Name        { get; }

        object Sender      { get; }
    }
}
