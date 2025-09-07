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
            DataTable clientsTable = _databaseService.GetClients();
            var clients = new List<Client>();

            foreach (DataRow row in clientsTable.Rows)
            {
                // Decrypt the passport number before adding to the list
                string encryptedPassport = row.Field<string>("Broj_pasosa");
                string decryptedPassport = PassportEncryptor.Decrypt(encryptedPassport);

                clients.Add(new Client
                {
                    Id = row.Field<int>("ID"),
                    FirstName = row.Field<string>("Ime"),
                    LastName = row.Field<string>("Prezime"),
                    PassportNumber = decryptedPassport, // Use the decrypted value
                    DateOfBirth = row.Field<DateTime>("Datum_rodjenja"),
                    Email = row.Field<string>("Email_adresa"),
                    PhoneNumber = row.Field<string>("Broj_telefona")
                });
            }

            return clients;
        }

        public List<Client> GetClientsByName(string firstName, string lastName)
        {
            DataTable clientsTable = _databaseService.GetClientByName(firstName, lastName);
            var clients = new List<Client>();

            foreach (DataRow row in clientsTable.Rows)
            {
                // Decrypt the passport number before adding to the list
                string encryptedPassport = row.Field<string>("Broj_pasosa");
                string decryptedPassport = PassportEncryptor.Decrypt(encryptedPassport);

                clients.Add(new Client
                {
                    Id = row.Field<int>("ID"),
                    FirstName = row.Field<string>("Ime"),
                    LastName = row.Field<string>("Prezime"),
                    PassportNumber = decryptedPassport, // Use the decrypted value
                    DateOfBirth = row.Field<DateTime>("Datum_rodjenja"),
                    Email = row.Field<string>("Email_adresa"),
                    PhoneNumber = row.Field<string>("Broj_telefona")
                });
            }

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