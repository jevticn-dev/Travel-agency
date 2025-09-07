using PathFinders.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Services
{
    public interface IDatabaseService
    {
        // kreira i vraća vezu sa bazom podataka
        IDbConnection GetConnection();

        // dodavanje
        void AddClient(Client client);
        void AddPackage(TravelPackage package);
        int AddReservation(Reservation reservation);
        void AddService(Service service);
        void AddReservationService(ReservationServiceAssociation reservationService);

        // citanje
        List<Client> GetClients();
        List<Client> GetClientByName(string firstName, string lastName);
        List<TravelPackage> GetPackages();
        List<TravelPackage> GetTravelPackageByType(string type);

        List<Reservation> GetReservationsForClient(int clientId);
        Client GetClientByPassportNumber(string passportNumber);
        int GetOrCreateDestination(string destinationName);
        List<Service> GetServices();
        List<Service> GetServicesForReservation(int reservationId);
        TravelPackage GetPackageById(int packageId);
        Reservation GetReservationById(int reservationId);
        List<Reservation> GetReservationsForPackage(int packageId);

        // azuriranje
        void UpdateClient(Client client);
        int UpdateReservation(Reservation reservation);
        void UpdateService(Service service);
        void UpdatePackage(TravelPackage package); // NOVA METODA

        // brisanje
        void DeleteReservation(int reservationId);
        void DeleteService(int serviceId);
        void DeleteReservationService(int reservationServiceId);
        void DeleteServicesForReservation(int reservationId); // Nova metoda
        void DeletePackage(int packageId); // NOVA METODA

        // ostalo
        List<String> GetAvailableDestinations();
    }
}