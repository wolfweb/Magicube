using System;

namespace Magicube.Core {
    public interface INamedService<TService> : IDisposable {
		TService Service { get; }
	}
}
