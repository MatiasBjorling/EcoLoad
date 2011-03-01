using System;

namespace EcoManager.Data.Management
{
    public class UnitOfWorkImpl : IDisposable
    {
        public String Id { get; private set; } 

        public UnitOfWorkImpl(String Id)
        {
            this.Id = Id;
        }

        private UnitOfWorkImpl()
        {
        }

        public void Dispose()
        {
            UnitOfWork.DisposeUnitOfWork(Id);
        }
    }
}
