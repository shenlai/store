using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure
{
    // 服务定位器的实现(备注：配置文件读取Unity 失败，转至Application实现注入,后又转至该实现)
    [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
    public class ServiceLocator:IServiceProvider
    {
        private readonly IUnityContainer _container;
        private static ServiceLocator _instance = new ServiceLocator();

        private ServiceLocator()
        {
            _container = new UnityContainer();

            /**************Unity加载配置文件的两种方式**********************/
            /*【1】显示加载指定的配置文件，通过ExeConfigurationFileMap指定文件路径：*/
            //var file = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            var unityConfig = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Unity\Unity.config";
            var fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = unityConfig };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var unitySection = (UnityConfigurationSection)configuration.GetSection("unity");
            _container.LoadConfiguration(unitySection);

            /*【2】1、当前AppDomain的配置文件（App.config或Web.config，通过AppDomain.CurrentDomain.SetupInformation.ConfigurationFile获得）：*/
            //try
            //{
            //    _container.LoadConfiguration();
            //}
            //catch (Exception)
            //{

            //    throw;
            //}

        }

        public static ServiceLocator Instance
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
