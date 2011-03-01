using System;
using System.ComponentModel;
using System.Threading;
using Castle.DynamicProxy;
using EcoManager.Shared.Tools;
using NHibernate.ByteCode.Castle;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using System.Collections;
using NHibernate.Proxy;
using NHibernate;
using LazyInitializer = NHibernate.ByteCode.Castle.LazyInitializer;

namespace EcoManager.Data.Proxies
{
    // We override this class implementation to handle the proxy creation ourselves. We 
    // need to specify our own Interceptor when a method of our entity is called.
    public class DataBindingNotifyPropertyProxyFactory : ProxyFactory
    {
        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            // If it is not a proxy for a class do what you usually did.
            if (!IsClassProxy) return base.GetProxy(id, session);

            try
            {
                LazyInitializer initializer = new DataBindingInterceptor(EntityName, PersistentClass, id,
                           GetIdentifierMethod, SetIdentifierMethod, ComponentIdType, session);

                // Add to the list of the interfaces that the proxy class will support the INotifyPropertyChanged interface.
                // This is only needed in the case when we need to cast our proxy object as INotifyPropertyChanged interface.
                ArrayList list = new ArrayList(Interfaces);
                list.Add(typeof(INotifyPropertyChanged));
                System.Type[] interfaces = (System.Type[])list.ToArray(typeof(System.Type));

                //We create the proxy
                object generatedProxy = DefaultProxyGenerator.CreateClassProxy(PersistentClass, interfaces, initializer);

                initializer._constructed = true;
                return (INHibernateProxy)generatedProxy;
            }
            catch (Exception e)
            {
                Logger.Error("Creating a proxy instance failed", e);
                throw new HibernateException("Creating a proxy instance failed", e);
            }
        }
    }
    public class DataBindingInterceptor : LazyInitializer
    {
        private const string ERROR_MSG = "{0} is not a public property of {1}";

        private PropertyChangedEventHandler subscribers = delegate { };

        public DataBindingInterceptor(String EntityName, Type persistentClass, object id, MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType aType, ISessionImplementor session)
            : base(EntityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, aType, session)
        {
        }

        public override void Intercept(IInvocation invocation)
        {

            // WPF will call a method named add_PropertyChanged to subscribe itself to the property changed events of 
            // the given entity. The method to call is stored in invocation.Arguments[0]. We get this and add it to the 
            // proxy subscriber list.
            if (invocation.Method.Name.Contains("PropertyChanged"))
            {
                PropertyChangedEventHandler propertyChangedEventHandler = (PropertyChangedEventHandler)invocation.Arguments[0];
                if (invocation.Method.Name.StartsWith("add_"))
                {
                    subscribers += propertyChangedEventHandler;
                }
                else
                {
                    subscribers -= propertyChangedEventHandler;
                }
            }

            // Here we call the actual method of the entity
            base.Intercept(invocation);

            // If the method that was called was actually a proeprty setter (set_Line1 for example) we generate the 
            // PropertyChanged event for the property but with event generator the proxy. This must do the trick.
            if (invocation.Method.Name.StartsWith("set_"))
            {
                subscribers(invocation.InvocationTarget, new PropertyChangedEventArgs(invocation.Method.Name.Substring(4)));
            }
        }
    }
}
