using System;
using EcoManager.Data.Management;
using EcoModel.Entities.Interfaces;
using NHibernate;
using NHibernate.Type;

namespace EcoManager.Data.Proxies
{
    public class AuditInterceptor : EmptyInterceptor  
    {

        public override object Instantiate(string clazz, EntityMode entityMode, object id)
        {
            if (entityMode == EntityMode.Poco)
            {
                Type type = Type.GetType(clazz);
                if (type != null)
                {
                    var instance = DataBindingFactory.Create(type);
                    UnitOfWork.GetSessionFactory.GetClassMetadata(clazz).SetIdentifier(instance, id, entityMode);
                    return instance;
                }
            }
            return base.Instantiate(clazz, entityMode, id);
        }

        public override string GetEntityName(object entity)
        {
            var markerInterface = entity as DataBindingFactory.IMarkerInterface;
            if (markerInterface != null)
                return markerInterface.TypeName;
            return base.GetEntityName(entity);
        }

        public override bool OnFlushDirty(object entity,
                                          object id,
                                          object[] currentState,
                                          object[] previousState,
                                          string[] propertyNames,
                                          IType[] types)
        {
            if (entity is IAuditable)
            {
                bool changed = false;
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (currentState[i] != null && !currentState[i].Equals(previousState[i]))
                    {
                        changed = true;
                        continue;
                    }
                }

                if (changed)
                {
                    for (int i = 0; i < propertyNames.Length; i++)
                    {
                        if ("Rettet".Equals(propertyNames[i]))
                        {
                            currentState[i] = DateTime.Now;
                            return true;
                        }
                    }
                }
                
            }

            return false;
        }

        public override bool OnSave(object entity,
                                    object id,
                                    object[] state,
                                    string[] propertyNames,
                                    IType[] types)
        {
            if (entity is IAuditable)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if ("Oprettet".Equals(propertyNames[i]))
                    {
                        state[i] = DateTime.Now;
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
