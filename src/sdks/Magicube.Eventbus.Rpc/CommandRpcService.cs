using MagicOnion;
using MagicOnion.Server;
using System;

namespace Magicube.Eventbus.Rpc {
    public class CommandRpcService : ServiceBase<ICommandRpcService>, ICommandRpcService {
        public UnaryResult ExecuteAsync() {
            throw new NotImplementedException();
        }
    }
}
