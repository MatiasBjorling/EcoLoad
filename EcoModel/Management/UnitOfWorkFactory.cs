using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Linq;
using EcoManager.Data.Proxies;
using EcoManager.Shared.DataAccess;
using EcoManager.Shared.Tools;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Driver;
//using NHibernate.Spatial.Dialect;
using NHibernate.Spatial.Dialect;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Event;

namespace EcoManager.Data.Management
{
    public class UnitOfWorkFactory
    {
        private static readonly Dictionary<String, ISession> sessions = new Dictionary<String, ISession>();

        private ISessionFactory sessionFactory;
        private ISessionFactory sessionMysqlFactory;
        private Configuration configuration;
        private Configuration configurationMysql;

        internal UnitOfWorkFactory()
        { }

        public void Create()
        {
            Create(Thread.CurrentThread.ManagedThreadId.ToString(), IsolationLevel.ReadCommitted, false);
        }

        public void Create(bool useMysql)
        {
            Create(Thread.CurrentThread.ManagedThreadId.ToString(), IsolationLevel.ReadCommitted, useMysql);
        }

        public void Create(IsolationLevel isolationLevel)
        {
            OpenSession(Thread.CurrentThread.ManagedThreadId.ToString(), isolationLevel, false);
        }
        public void Create(string key, IsolationLevel isolationLevel, bool useMysql)
        {
            if (sessions.ContainsKey(key))
                throw new InvalidOperationException("Thread already have an open unit of work.");
            
            OpenSession(key, isolationLevel, useMysql);
        }

        public ISessionFactory SessionFactory(bool useMysql)
        {
            if (useMysql)
            {
                
                if (sessionMysqlFactory == null)
                    sessionMysqlFactory = ConfigurationForMysql.BuildSessionFactory();
                return sessionMysqlFactory;
            }
            else
            {
                if (sessionFactory == null)
                    sessionFactory = Configuration.BuildSessionFactory();
                return sessionFactory;
            }
        }

        public ISession CurrentSession
        {
            get { return GetSession(Thread.CurrentThread.ManagedThreadId.ToString()); }
        }

        public bool ExistSession()
        {
            return ExistSession(Thread.CurrentThread.ManagedThreadId.ToString());
        }

        public bool ExistSession(string key)
        {
            return sessions.ContainsKey(key);
        }

        public ISession GetSession(string key)
        {
            if (!sessions.ContainsKey(key))
                throw new InvalidOperationException("You are not in a unit of work.");

            return sessions[key];
        }

        public ISessionFactory GetMysqlSessionFactory()
        {
            return SessionFactory(true);
        }

        public ISessionFactory GetSessionFactory()
        {
            return SessionFactory(false);
        }

        public void DisposeUnitOfWork(String id)
        {
            // Close the session 
            ISession session = sessions[id];

            try
            {
                if (session.Transaction.IsActive)
                    session.Transaction.Commit();
                
                session.Close();

            } catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            
            // and dispose
            sessions[id] = null;
            sessions.Remove(id);
            session.Dispose();
        }

        public string GetGeneratedSessionKey()
        {
            return Thread.CurrentThread.ManagedThreadId.ToString();
        }

        private void OpenSession(string key, IsolationLevel isolationLevel, bool useMysql)
        {
            ISession session = CreateSession(useMysql);
            session.Transaction.Begin(isolationLevel);
            sessions[key] = session;
        }

        private ISession CreateSession(bool useMysql)
        {
            return SessionFactory(useMysql).OpenSession();
        }

        #region Database configuration setup
        public Configuration Configuration
        {
            get
            {
                if (configuration == null)
                {

                    var fluentConfiguration =
                        Fluently.Configure()
                            .Database(
                                Sql2008Configuration.MsSql2008.ConnectionString(SQLConn.StrConn)
                                    .Cache(c => c
                                                    .UseQueryCache()
                                                    .ProviderClass<NHibernate.Caches.SysCache.SysCacheProvider>()
                                                    )
                                    .AdoNetBatchSize(1000)
                            )

                            .Mappings(m => m
                                               .FluentMappings.AddFromAssemblyOf<UnitOfWorkFactory>()
                                               .Conventions.Setup(GetConventions())
                            .ExportTo("D:\\tmp")
                            )

                            .ExposeConfiguration(config => config
                                .SetInterceptor(new AuditInterceptor())
                                .SetProperty(NHibernate.Cfg.Environment.ProxyFactoryFactoryClass, typeof(ExtendedWithNotifyProxyFactoryFactory).AssemblyQualifiedName)
                            );


                    NHibernate.Validator.Cfg.Environment.SharedEngineProvider = new NHibernateSharedEngineProvider();

                    NHibernate.Validator.Cfg.Loquacious.FluentConfiguration validatorConfig = new NHibernate.Validator.Cfg.Loquacious.FluentConfiguration();

                    validatorConfig.Register(
                        Assembly.GetAssembly(typeof(UnitOfWorkFactory)).GetTypes().Where(
                            t =>
                                t.Namespace != null &&
                                t.Namespace.Equals("EcoManager.Entities"))
                                .ToList())
                                .SetMessageInterpolator<ConventionMessageInterpolator>()
                                .SetCustomResourceManager("EcoManager.Properties.ValidationTexts", Assembly.GetAssembly(typeof(UnitOfWorkFactory)))
                                .SetDefaultValidatorMode(ValidatorMode.UseAttribute)
                                .IntegrateWithNHibernate
                                .ApplyingDDLConstraints()
                                .And.RegisteringListeners();


                    ValidatorEngine ve = NHibernate.Validator.Cfg.Environment.SharedEngineProvider.GetEngine();
                    ve.Configure(validatorConfig);

                    configuration = fluentConfiguration.BuildConfiguration();
                    // Update the configuration with our validation rules.

                    ValidatorInitializer.Initialize(configuration, ve);
                }

                return configuration;
            }
        }

        public Configuration ConfigurationForMysql
        {
            get
            {
                if (configurationMysql == null)
                {

                    var fluentConfiguration =
                        Fluently.Configure()
                            .Database(
                            MySQLConfiguration.Standard
                            .Driver("NHibernate.Driver.MySqlDataDriver")
                            .ConnectionString(SQLConn.StrConnMysql)
                            .Dialect("NHibernate.Dialect.MySQLDialect")
                            .Cache(c=>c.UseQueryCache()))
                            .Mappings(m => m
                                               .FluentMappings.AddFromAssemblyOf<UnitOfWorkFactory>()
                                               .Conventions.Setup(GetConventions())
                            .ExportTo("D:\\tmp")
                            )

                            .ExposeConfiguration(config => config
                                .SetInterceptor(new AuditInterceptor())
                                .SetProperty(NHibernate.Cfg.Environment.ProxyFactoryFactoryClass, typeof(ExtendedWithNotifyProxyFactoryFactory).AssemblyQualifiedName)
                            );


                    NHibernate.Validator.Cfg.Environment.SharedEngineProvider = new NHibernateSharedEngineProvider();

                    NHibernate.Validator.Cfg.Loquacious.FluentConfiguration validatorConfig = new NHibernate.Validator.Cfg.Loquacious.FluentConfiguration();

                    validatorConfig.Register(
                        Assembly.GetAssembly(typeof(UnitOfWorkFactory)).GetTypes().Where(
                            t =>
                                t.Namespace != null &&
                                t.Namespace.Equals("EcoManager.Entities"))
                                .ToList())
                                .SetMessageInterpolator<ConventionMessageInterpolator>()
                                .SetCustomResourceManager("EcoManager.Properties.ValidationTexts", Assembly.GetAssembly(typeof(UnitOfWorkFactory)))
                                .SetDefaultValidatorMode(ValidatorMode.UseAttribute)
                                .IntegrateWithNHibernate
                                .ApplyingDDLConstraints()
                                .And.RegisteringListeners();


                    ValidatorEngine ve = NHibernate.Validator.Cfg.Environment.SharedEngineProvider.GetEngine();
                    ve.Configure(validatorConfig);

                    configuration = fluentConfiguration.BuildConfiguration();
                    // Update the configuration with our validation rules.

                    ValidatorInitializer.Initialize(configuration, ve);
                }

                return configuration;
            }
        }

        private class EnumConvention : IUserTypeConvention
        {
            public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
            {
                
                criteria.Expect(x => x.Property.PropertyType.IsEnum ||
                    (x.Property.PropertyType.IsGenericType &&
                     x.Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                     x.Property.PropertyType.GetGenericArguments()[0].IsEnum)
                    );
            }

            public void Apply(IPropertyInstance target)
            {
                target.CustomType(target.Property.PropertyType);
            }
        }

        private static Action<IConventionFinder> GetConventions()
        {
            return c => c.Add<EnumConvention>();
        }

        public class Sql2008Configuration : PersistenceConfiguration<Sql2008Configuration, MsSqlConnectionStringBuilder>
        {
            public Sql2008Configuration()
            {
                Driver<SqlClientDriver>();
            }

            public static Sql2008Configuration MsSql2008
            {
                get { return new Sql2008Configuration().Dialect<MsSql2008GeographyDialect>(); }
            }
        }


        #endregion
    }

    
}
