using PathFinders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Facade
{
    public interface ITravelAgencyFacade
    {
        Task<List<Client>> GetClientsAsync(string? search = null);
        Task AddClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task<Client?> GetClientByPassportNumberAsync(string passportNumber);



        // Paketi
        Task<List<TravelPackage>> GetPackagesAsync(string? search = null);
        Task AddPackageAsync(string name, decimal price, string destinationName, PathFinders.Patterns.Builder.TravelPackageBuilder builder);
        Task UpdatePackageAsync(TravelPackage package);
        Task DeletePackageAsync(int packageId);



        // Usluge (Add-ons)
        Task<List<Service>> GetServicesAsync(string? search = null);
        Task AddServiceAsync(Service service);
        Task UpdateServiceAsync(Service service);
        Task DeleteServiceAsync(int serviceId);



        // Rezervacije
        Task AddReservationAsync(Reservation reservation);
        Task<List<Reservation>> GetReservationsForClientAsync(int clientId);
        Task AddServiceToReservationAsync(int reservationId, int serviceId);
        Task<decimal> GetReservationTotalPriceAsync(int reservationId);
        Task UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(int reservationId);



        // Helper
        Task DeleteReservationsForPackageAsync(int packageId);
    }
}
