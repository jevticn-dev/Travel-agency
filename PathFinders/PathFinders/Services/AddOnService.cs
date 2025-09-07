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
            // Now directly returns a List<Service>, no DataTable conversion is needed
            return _databaseService.GetServices();
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