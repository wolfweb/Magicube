using System;

namespace Magicube.Data.Abstractions.EfDbContext {
    public interface IUnitOfWork : IDisposable{
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
