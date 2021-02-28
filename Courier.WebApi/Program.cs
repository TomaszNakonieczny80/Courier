using System;
using Topshelf;
using Unity;

namespace Courier.WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityDiContainerProvider().GetContainer();
            var appHost = container.Resolve<AppHost>();

            var rc = HostFactory.Run(x =>
            {
                x.Service<AppHost>(s =>
                {
                    s.ConstructUsing(sf => appHost);
                    s.WhenStarted(ah => ah.Start());
                    s.WhenStopped(ah => ah.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("WebApiExample.TopShelf service");
                x.SetDisplayName("CM.20201103.WebApiExample.TopShelf");
                x.SetServiceName("CM.20201103.WebApiExample.TopShelf");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
