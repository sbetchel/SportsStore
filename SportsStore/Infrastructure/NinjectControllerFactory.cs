using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Modules;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using SportsStore.Domain.Services;

namespace SportsStore.Infrastructure
{
    public class NinjectControllerFactory : System.Web.Mvc.DefaultControllerFactory
    {
        // A ninject "kernel" is the thing that can supply object instances.
        private IKernel kernel = new StandardKernel(new SportsStoreServices());

        // asp.net calls this to get the controller for each request
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                return null;
            return (IController)kernel.Get(controllerType);
        }
    }

    class SportsStoreServices : NinjectModule
    {
        public override void Load()
        {
            Bind<IProductsRepository>()
                .To<SqlProductsRepository>()
                .WithConstructorArgument("connectionString", System.Configuration.ConfigurationManager.ConnectionStrings["AppDb"].ConnectionString);

            Bind<IOrderSubmitter>()
                .To<EmailOrderSubmitter>()
                .WithConstructorArgument("mailTo", System.Configuration.ConfigurationManager.AppSettings["EmailOrderSubmitter.MailTo"]);
        }
    }
}