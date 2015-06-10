using System.IO;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Configuration;
using System.Reflection;
using System.Web.Compilation;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System.Web.Mvc;
using System.Web;
using System.Web.Http;
using System.Configuration;
using Autofac.Integration.Wcf;

namespace Pons.IocContainer
{
    public class AutofacRegister : IocRegister
    {
        public void Register()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            // builder.RegisterModule(new ConfigurationSettingsReader("autofac"));

            var container = builder.Build();


            ServiceLocator.SetLocatorProvider(() =>
            {
                var httpContext = HttpContext.Current;
                if (httpContext != null && httpContext.CurrentHandler is MvcHandler)
                {
                    return new AutofacServiceLocator(AutofacDependencyResolver.Current.RequestLifetimeScope);
                }
                else
                {
                    return new AutofacServiceLocator(container);
                }
            });

            //mvc
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //   return container;

            //wcf host
            AutofacHostFactory.Container = container;

            // Create the depenedency resolver.
            var resolver = new AutofacWebApiDependencyResolver(container);


            // Configure Web API with the dependency resolver.
            // GlobalConfiguration.Configuration.DependencyResolver = resolver;

        }
    }

    public class AutofacModule : Autofac.Module
    {
        public bool ObeySpeedLimit { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            List<Assembly> assList = new List<Assembly>();

            if (HttpContext.Current == null)
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                foreach (string dll in Directory.GetFiles(path, "*.dll"))
                    assList.Add(Assembly.LoadFile(dll));
            }
            else
            {
                assList = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
            }

            //var DALTypes = Assembly.Load("Vancl.TMS.IDAL").GetTypes();
            //var BLLTypes = Assembly.Load("Vancl.TMS.IBLL").GetTypes();
        //    Vancl.TMS.Util.Pager.PagedList<
            //The type 'Vancl.TMS.Util.Pager.PagedList`1[T]' is not assignable to service 'System.Collections.Generic.IList`1'.
            assList.Where(x => x.FullName.Contains("Vancl.TMS")).ToList().ForEach(assembly =>
                {
                    var allTypes = assembly.GetTypes();
                    allTypes.Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType)
                        .ToList().ForEach(x =>
                    {
                        var name = x.Name;
                        var ifs = x.GetInterfaces();
                        if (ifs.Length == 0)
                        { 
                            builder.RegisterType(x).AsSelf().AsImplementedInterfaces().PropertiesAutowired().Named(name, x);
                        }else{
                            ifs.ToList().ForEach(type=>  builder.RegisterType(x).AsSelf().AsImplementedInterfaces().PropertiesAutowired().Named(name, type));
                        }
                    });
                    
           // DpsNotice


                    //     The type 'Vancl.TMS.DAL.Oracle.DbModelInfo' is not assignable to service 'DbModelInfo (System.RuntimeType)'.

                    /*
                    //注册Dao层接口 
                    var daoTypes = allTypes.Where(x =>x.IsClass && !x.IsAbstract && x.GetInterfaces().Any(t => DALTypes.Contains(t))).ToArray();
                    daoTypes.ToList().ForEach(x =>
                    {
                        var name = x.Name;
                        var ifs = x.GetInterfaces();
                        var type = x.GetType();
                        if (ifs.Length > 0) type = ifs[0];
                        builder.RegisterType(x).AsImplementedInterfaces().PropertiesAutowired().Named(name, type);
                    });

                    //注册Service层
                    var serviceTypes = allTypes.Where(x => x.IsClass && !x.IsAbstract && x.GetInterfaces().Any(t => BLLTypes.Contains(t))).ToArray();
                    serviceTypes.ToList().ForEach(x =>
                    {
                        var name = x.Name;
                        var ifs = x.GetInterfaces();
                        var type = x.GetType();
                        if (ifs.Length > 0) type = ifs[0];
                        builder.RegisterType(x).AsImplementedInterfaces().PropertiesAutowired().Named(name, type);
                    });

                    //注册IFormula<T, M>
                    allTypes.Where(x => x.Name.EndsWith("Formula"))
                   .Where(x => x.GetInterfaces().Any(interfaceType =>
                   {
                       if (false == interfaceType.IsGenericType) { return false; }
                       var genericType = interfaceType.GetGenericTypeDefinition();
                       if (genericType == typeof(IFormula<,>)) return true;
                       return false;
                   })).ToList().ForEach(x =>
                   {
                       var name = x.Name;
                       var type = x.GetType();
                       var ifs = x.GetInterfaces();
                       if (ifs.Length > 0) type = ifs[0];
                       builder.RegisterType(x).AsImplementedInterfaces().Named(name, type);
                   });


                    //注册IChecker<T>
                    allTypes.Where(x => x.Name.StartsWith("LimitSubject"))
                   .Where(x => x.GetInterfaces().Any(interfaceType =>
                   {
                       if (false == interfaceType.IsGenericType) { return false; }
                       var genericType = interfaceType.GetGenericTypeDefinition();
                       if (genericType == typeof(IChecker<>)) return true;
                       return false;
                   })).ToList().ForEach(x =>
                   {
                       var name = x.Name;
                       var type = x.GetType();
                       var ifs = x.GetInterfaces();
                       if (ifs.Length > 0) type = ifs[0];
                       builder.RegisterType(x).AsImplementedInterfaces().Named(name, type);
                   });
                    */

                    //注册Controllers
                    builder.RegisterControllers(assembly).PropertiesAutowired();
                    //注册ApiControllers
                    builder.RegisterApiControllers(assembly).PropertiesAutowired();
                });



        }
    }
}
