namespace Magicube.Data.Abstractions.ViewModel {
    public interface IViewModel<TKey> { 
        TKey Id { get; }
    }

    public abstract class ViewModel<TKey> : IViewModel<TKey> {
        public abstract TKey Id { get; set; }
    }

    public abstract class ViewModel : ViewModel<long> {
        public override long Id { get; set; }
    }
}
