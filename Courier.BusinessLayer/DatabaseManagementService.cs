using System;
using System.Collections.Generic;
using System.Text;
using Courier.DataLayer;

namespace Courier.BusinessLayer
{
    public interface IDatabaseManagementService
    {
        void EnsureDatabaseCreation();
    }
    public class DatabaseManagementService : IDatabaseManagementService
    {
        public void EnsureDatabaseCreation()
        {
            using (var context = new ParcelsDbContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
