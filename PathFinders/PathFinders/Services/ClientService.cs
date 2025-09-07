using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Models;

namespace PathFinders.Services
{
    public class ClientService
    {
        private readonly IDatabaseService _databaseService;

        public ClientService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddClient(Client client)
        {
            _databaseService.AddClient(client);
        }

        public List<Client> GetClients()
        {
            // Now directly call the database service method that returns a List<Client>
            List<Client> clients = _databaseService.GetClients();
            return clients;
        }

        public List<Client> GetClientsByName(string firstName, string lastName)
        {
            // Now directly call the database service method that returns a List<Client>
            List<Client> clients = _databaseService.GetClientByName(firstName, lastName);
            return clients;
        }

        public void UpdateClient(Client client)
        {
            _databaseService.UpdateClient(client);
        }

        public Client GetClientByPassportNumber(string passportNumber)
        {
            return _databaseService.GetClientByPassportNumber(passportNumber);
        }
    }
}