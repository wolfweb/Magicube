using System;

namespace Magicube.Data.Abstractions.EfDbContext {
    public interface IUnitOfWork : IDisposable{
        IUnitOfWorkScoped BeginTransaction();
    }

    public interface IUnitOfWorkScoped : IDisposable {
        void Rollback();
    }
}
