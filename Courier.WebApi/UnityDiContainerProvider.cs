using System;
using Courier.BusinessLayer;
using Courier.DataLayer;
using Unity;
using Unity.Injection;

namespace Courier.WebApi
{
    public class UnityDiContainerProvider
    {
        public IUnityContainer GetContainer()
        {
            var container = new UnityContainer();

            //container.RegisterType<IMenu, Menu>();
            //container.RegisterType<IMenuItem, MenuItem>();
            //container.RegisterType<IIoHelper, IoHelper>();
            container.RegisterType<IDatabaseManagementService, DatabaseManagementService>();
            container.RegisterType<IUsersService, UsersService>();
            container.RegisterType<IParcelsService, ParcelsService>();
            container.RegisterType<ICarsService, CarsService>();
            container.RegisterType<ICoordinatesService, CoordinatesService>();
            container.RegisterType<ITimeService, TimeService>();
            container.RegisterType<INotificationsService, NotificationsService>();
            container.RegisterType<IShipmentsService, ShipmentsService>();
            container.RegisterType<Func<IParcelsDbContext>>(
                new InjectionFactory(ctx => new Func<IParcelsDbContext>(() => new ParcelsDbContext())));

            return container;
        }
    }
}
