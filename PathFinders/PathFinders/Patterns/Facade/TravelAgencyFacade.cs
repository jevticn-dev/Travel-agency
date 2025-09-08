using PathFinders.Models;
using PathFinders.Patterns.Builder;
using PathFinders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Facade
{
    internal class TravelAgencyFacade : ITravelAgencyFacade
    {
        private readonly ClientService _clients;
        private readonly PackageService _packages;
        private readonly AddOnService _addons;
        private readonly ReservationService _reservations;

        private readonly IDatabaseService _db;

        public TravelAgencyFacade(IDatabaseService dbService)
        {
            _db = dbService;
            _clients = new ClientService(_db);
            _reservations = new ReservationService(_db);
            _packages = new PackageService(_db, _reservations);
            _addons = new AddOnService(_db);
        }

        // Klijenti 
        public Task<List<Client>> GetClientsAsync(string? search = null)
        {
            return Task.FromResult(_clients.GetClients());
        }

        public Task<List<Client>> GetClientsByName(string firstName, string lastName)
        {
            return Task.FromResult(_clients.GetClientsByName(firstName,lastName));
        }

        public Task<Client?> GetClientByPassportNumberAsync(string passportNumber)
        {
            return Task.FromResult(_clients.GetClientByPassportNumber(passportNumber));
        }
           

        public Task AddClientAsync(Client client)
        {
            _clients.AddClient(client);
            return Task.CompletedTask;
        }

        public Task UpdateClientAsync(Client client)
        {
            _clients.UpdateClient(client);
            return Task.CompletedTask;
        }

        // Paketi

        public Task<List<TravelPackage>> GetPackagesAsync(string? search = null)
        {
            return Task.FromResult(_packages.GetPackages());
        }
        public List<TravelPackage> GetPackagesByType(string type)
        {
            return _packages.GetPackagesByType(type);
        }
        public Task AddPackageAsync(string name, decimal price, string destinationName, TravelPackageBuilder builder)
        {
            _packages.AddPackage(builder, name, price, destinationName);
            return Task.CompletedTask;
        }

        public Task UpdatePackageAsync(TravelPackage package)
        {
            _packages.UpdatePackage(package);
            return Task.CompletedTask;
        }

        public Task DeletePackageAsync(int packageId)
        {
            _packages.DeletePackage(packageId);
            return Task.CompletedTask;
        }

        // Rezervacije

        public Task<List<Reservation>> GetReservationsForClientAsync(int clientId)
        {
            return Task.FromResult(_reservations.GetReservationsForClient(clientId));
        }

        public Task AddReservationAsync(Reservation reservation)
        {
            _reservations.AddReservation(reservation);
            return Task.CompletedTask;
        }

        public Task AddServiceToReservationAsync(int reservationId, int serviceId)
        {
            _reservations.AddServiceToReservation(reservationId, serviceId);
            return Task.CompletedTask;
        }

        public Task<decimal> GetReservationTotalPriceAsync(int reservationId)
            => Task.FromResult(_reservations.GetTotalPrice(reservationId));

        public Task UpdateReservationAsync(Reservation reservation)
        {
            _reservations.UpdateReservation(reservation);
            return Task.CompletedTask;
        }

        public Task DeleteReservationAsync(int reservationId)
        {
            _reservations.DeleteReservation(reservationId);
            return Task.CompletedTask;
        }

        public Task DeleteReservationsForPackageAsync(int packageId)
        {
            _reservations.DeleteReservationsForPackage(packageId);
            return Task.CompletedTask;
        }


        //Usluge

        public Task<List<Service>> GetServicesAsync(string? search = null)
        {
            return Task.FromResult(_addons.GetServices());
        }

        public Task AddServiceAsync(Service service)
        {
            _addons.AddService(service);
            return Task.CompletedTask;
        }

        public Task UpdateServiceAsync(Service service)
        {
            _addons.UpdateService(service);
            return Task.CompletedTask;
        }

        public Task DeleteServiceAsync(int serviceId)
        {
            _addons.DeleteService(serviceId);
            return Task.CompletedTask;
        }


    }
}
