using System;
using System.Data;
using EcoManager.Shared.Tools;
using NHibernate;

namespace EcoManager.Data.Management
{
    public static class UnitOfWork
    {
        private static readonly UnitOfWorkFactory unitOfWorkFactory = new UnitOfWorkFactory();

        public static bool IsStarted
        {
            get
            {
                return unitOfWorkFactory.ExistSession();
            }
        }

        public static ISession CurrentSession
        {
            get
            {
                if (unitOfWorkFactory.CurrentSession == null)
                    throw new InvalidOperationException("You are in no unit of work.");

                return unitOfWorkFactory.CurrentSession;
            }
        }
        public static ISession GetSession(string key)
        {
            return unitOfWorkFactory.GetSession(key);
        }

        public static void CommitAndRestartTransaction(string key)
        {
            try
            {
                unitOfWorkFactory.GetSession(key).Transaction.Commit();
                unitOfWorkFactory.GetSession(key).Transaction.Begin();    
            } catch (Exception ex)
            {
                Logger.Message("Der opstod en fejl ved skrivning til databasen. " + ex.Message);
                Logger.Error(ex.ToString());
            }
        }

        public static string GetGeneratedSessionKey
        {
            get { return unitOfWorkFactory.GetGeneratedSessionKey(); }
        }
        public static string GetRandomKey
        {
            get { return unitOfWorkFactory.GetGeneratedSessionKey() + new Random().Next(100000); }
        }
        public static void ClearSecondLevelCache()
        {
            // Clear out the hibernate second level cache
            var roleMap = GetSessionFactory.GetAllCollectionMetadata();
            foreach (String roleName in roleMap.Keys)
            {
                GetSessionFactory.EvictCollection(roleName);
            }

            var entityMap = GetSessionFactory.GetAllClassMetadata();
            foreach (String entityName in entityMap.Keys)
            {
                GetSessionFactory.EvictEntity(entityName);
            }

            GetSessionFactory.EvictQueries();
        }

        public static UnitOfWorkImpl Start()
        {
            lock (lockObject) {
                unitOfWorkFactory.Create();

                return new UnitOfWorkImpl(unitOfWorkFactory.GetGeneratedSessionKey());
            }
        }

        public static UnitOfWorkImpl StartMysql()
        {
            lock (lockObject)
            {
                unitOfWorkFactory.Create(true);

                return new UnitOfWorkImpl(unitOfWorkFactory.GetGeneratedSessionKey());
            }
        }

        public static UnitOfWorkImpl Start(string key)
        {
            return Start(key, IsolationLevel.ReadCommitted);
        }
        public static UnitOfWorkImpl Start(IsolationLevel isolationLevel)
        {
            return Start(GetGeneratedSessionKey, isolationLevel);
        }

        private static Object lockObject = new object();
        public static UnitOfWorkImpl Start(string key, IsolationLevel isolationLevel)
        {
            lock (lockObject)
            {
                unitOfWorkFactory.Create(key, isolationLevel, false);

                return new UnitOfWorkImpl(key);
            }
        }

        public static ISessionFactory GetSessionFactory
        {
            get { return unitOfWorkFactory.GetSessionFactory(); }
        }

        public static void DisposeUnitOfWork(String id)
        {
            unitOfWorkFactory.DisposeUnitOfWork(id);
        }
    }
}
