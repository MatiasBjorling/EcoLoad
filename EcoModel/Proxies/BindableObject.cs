using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;
using NHibernate.Validator.Engine;

namespace EcoManager.Data.Proxies
{
    /// <summary>
    /// Implements the INotifyPropertyChanged interface and 
    /// exposes a RaisePropertyChanged method for derived 
    /// classes to raise the PropertyChange event.  The event 
    /// arguments created by this class are cached to prevent 
    /// managed heap fragmentation.
    /// </summary>
    [Serializable]
    public abstract class BindableObject : INotifyPropertyChanged, IDataErrorInfo
    {
        private static readonly Dictionary<string, PropertyChangedEventArgs> eventArgCache;
        private const string ERROR_MSG = "{0} is not a public property of {1}";
        private static readonly object syncLock = new object();
        private static ValidatorEngine validation;
        private bool isValid;


        static BindableObject()
        {
            eventArgCache = new Dictionary<string, PropertyChangedEventArgs>();
            validation = NHibernate.Validator.Cfg.Environment.SharedEngineProvider.GetEngine();
        }
        
        /// <summary>
        /// Raised when a public property of this object is set.
        /// </summary>
        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns an instance of PropertyChangedEventArgs for 
        /// the specified property name.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to create event args for.
        /// </param>		
        public static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName cannot be null or empty.");

            PropertyChangedEventArgs args;
            lock (syncLock)
            {
                if (!eventArgCache.TryGetValue(propertyName, out args))
                {
                    eventArgCache.Add(propertyName, args = new PropertyChangedEventArgs(propertyName));
                }
            }
            return args;
        }

        /// <summary>
        /// Derived classes can override this method to
        /// execute logic after a property is set. The 
        /// base implementation does nothing.
        /// </summary>
        /// <param name="propertyName">
        /// The property which was changed.
        /// </param>
        protected virtual void AfterPropertyChanged(string propertyName)
        {
        }

        /// <summary>
        /// Attempts to raise the PropertyChanged event, and 
        /// invokes the virtual AfterPropertyChanged method, 
        /// regardless of whether the event was raised or not.
        /// </summary>
        /// <param name="propertyName">
        /// The property which was changed.
        /// </param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            VerifyProperty(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                // Get the cached event args.
                PropertyChangedEventArgs args =
                    GetPropertyChangedEventArgs(propertyName);

                // Raise the PropertyChanged event.
                handler(this, args);
            }

            AfterPropertyChanged(propertyName);
        }


        [Conditional("DEBUG")]
        private void VerifyProperty(string propertyName)
        {
            bool propertyExists = TypeDescriptor.GetProperties(this).Find(propertyName, false) != null;
            if (!propertyExists)
            {
                // The property could not be found,
                // so alert the developer of the problem.

                string msg = string.Format(
                    ERROR_MSG,
                    propertyName,
                    GetType().FullName);

                Debug.Fail(msg);
            }
        }

        public virtual bool IsValid
        {
            get
            {
                var v = validation.Validate(this, new object[0]);
                return v.Length == 0;
            }
        }

        public virtual IEnumerable<string> GetErrors
        {
            get
            {
                return from p in validation.Validate(this, new object[0]) select p.Message ;;
            }
        }
        
        public virtual string this[string propertyName]
        {
            get
            {
                isValid = true;
                var rules = GetInvalidRules(propertyName);
                if (rules != null && rules.Count > 0)
                {
                    isValid = false;
                    return rules[0].Message;
                }

                return null;
            }
        }

        public virtual string Error
        {
            get { return String.Empty; }
        }

        private IList<InvalidValue> GetInvalidRules(string propertyName)
        {
            var type = GetType();

            object obj = GetPropertyValue(type, propertyName);

            return validation.ValidatePropertyValue(type, propertyName, obj);
        }

        private object GetPropertyValue(Type objectType, string properyName)
        {
            return objectType.GetProperty(properyName).GetValue(this, null);
        }
    }
}
