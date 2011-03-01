using System;
using NHibernate.Proxy;

namespace EcoManager.Data.Proxies
{
    public class ExtendedWithNotifyProxyFactoryFactory : NHibernate.Bytecode.IProxyFactoryFactory
    {

        public IProxyFactory BuildProxyFactory()
        {
            return new DataBindingNotifyPropertyProxyFactory();
        }

        public bool IsInstrumented(Type entityClass)
        {
            return false;
        }

        public IProxyValidator ProxyValidator
        {
            get
            {
                return new DynProxyTypeValidator();
            }
        }
    }
}