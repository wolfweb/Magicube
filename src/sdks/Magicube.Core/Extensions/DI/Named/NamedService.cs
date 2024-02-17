using System;

namespace Magicube.Core{
    public class NamedService<TService, TNamed> : INamedService<TService> where TService : class where TNamed : struct {
		public string Name { get; }
		public TService Service { get; private set; }
		public NamedService(TService service) {
			Service = service;
			Name = typeof(TNamed).Name;
		}
		~NamedService() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected void Dispose(bool disposing) {
			if (disposing) {
				if (Service != null) {
					if (Service is IDisposable disposable) {
						disposable.Dispose();
					}
					Service = null;
				}
			}
		}
	}
}
