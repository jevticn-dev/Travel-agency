using PathFinders.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Services
{
    public class AddOnService
    {
        private readonly IDatabaseService _databaseService;

        public AddOnService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddService(Service service)
        {
            _databaseService.AddService(service);
        }

        public List<Service> GetServices()
        {
            DataTable servicesTable = _databaseService.GetServices();
            var services = new List<Service>();

            foreach (DataRow row in servicesTable.Rows)
            {
                services.Add(new Service
                {
                    Id = row.Field<int>("ID"),
                    ServiceName = row.Field<string>("ServiceName"),
                    ServicePrice = row.Field<decimal>("ServicePrice"),
                    ServiceDescription = row.Field<string>("ServiceDescription")
                });
            }

            return services;
        }

        public void UpdateService(Service service)
        {
            _databaseService.UpdateService(service);
        }

        public void DeleteService(int serviceId)
        {
            _databaseService.DeleteService(serviceId);
        }
    }
}
