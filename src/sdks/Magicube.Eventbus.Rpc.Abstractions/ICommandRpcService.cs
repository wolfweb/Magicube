using MagicOnion;

namespace Magicube.Eventbus.Rpc {
    public interface ICommandRpcService : IService<ICommandRpcService> {
        UnaryResult ExecuteAsync();
    }
}
