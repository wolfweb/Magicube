namespace Magicube.Versioning.Abstractions {
	public interface IVersioningMessageService {
		string GetMessage(bool isCreate);
	}

    public class NullVersioningMessageService : IVersioningMessageService {
        public string GetMessage(bool isCreate) => string.Empty;
    }
}