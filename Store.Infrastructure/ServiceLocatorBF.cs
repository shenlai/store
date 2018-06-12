using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure
{
    // 服务定位器的实现(备注：配置文件读取Unity 失败，转至Application实现注入)
    public class ServiceLocatorBF:IServiceProvider
    {
        //private readonly IUnityContainer _container;
        private static IUnityContainer _container;
        private static ServiceLocatorBF _instance = new ServiceLocatorBF();

        static ServiceLocatorBF()
        {
            //var unityConfig = AppDomain.CurrentDomain.BaseDirectory + @"UnityConfig\Unity.config";
            var unityConfig = @"F:\Project\Store\Store.Application.Test\UnityConfig\Unity.config";

            var fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = unityConfig };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var unitySection = (UnityConfigurationSection)configuration.GetSection("unity");
            _container.LoadConfiguration(unitySection);

            //_container = new UnityContainer();
            //var unitySection = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            //_container.LoadConfiguration();
            int x = 0;
        }

        //private static ServiceLocator Instance
        public static ServiceLocatorBF Instance
        {
            get {return _instance;}
        }

        #region Public Methods

        public T GetService<T>()
        {
            return _container.Resolve<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _container.ResolveAll<T>();
        }

        public T GetService<T>(object overridedArguments)
        {
            var overrides = GetParameterOverrides(overridedArguments);
            return _container.Resolve<T>(overrides.ToArray());
        }

        public object GetService(Type serviceType, object overridedArguments)
        {
            var overrides = GetParameterOverrides(overridedArguments);
            return _container.Resolve(serviceType, overrides.ToArray());
        }

        #endregion

        #region Private Methods

        private IEnumerable<ParameterOverride> GetParameterOverrides(object overridedArguments)
        {
            var overrides = new List<ParameterOverride>();
            var argumentsType = overridedArguments.GetType();
            argumentsType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToList()
                .ForEach(property =>
                {
                    var propertyValue = property.GetValue(overridedArguments, null);
                    var propertyName = property.Name;
                    overrides.Add(new ParameterOverride(propertyName, propertyValue));
                });
            return overrides;
        }
        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        #endregion


    }
}
