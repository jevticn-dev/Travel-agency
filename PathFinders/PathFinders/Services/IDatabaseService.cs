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
        void AddReservation(Reservation reservation);
        void AddService(Service service);
        void AddReservationService(ReservationServiceAssociation reservationService);

        // citanje
        DataTable GetClients();
        DataTable GetPackages();
        DataTable GetReservationsForClient(int clientId);
        Client GetClientByPassportNumber(string passportNumber);
        int GetOrCreateDestination(string destinationName);
        DataTable GetServices();
        List<Service> GetServicesForReservation(int reservationId);
        TravelPackage GetPackageById(int packageId);
        Reservation GetReservationById(int reservationId);
        List<Reservation> GetReservationsForPackage(int packageId);

        // azuriranje
        void UpdateClient(Client client);
        void UpdateReservation(Reservation reservation);
        void UpdateService(Service service);

        void UpdatePackage(TravelPackage package); // NOVA METODA

        // brisanje
        void DeleteReservation(int reservationId);
        void DeleteService(int serviceId);
        void DeleteReservationService(int reservationServiceId);
        void DeleteServicesForReservation(int reservationId); // Nova metoda

        void DeletePackage(int packageId); // NOVA METODA

        // ostalo
        DataTable GetAvailableDestinations();
    }
}