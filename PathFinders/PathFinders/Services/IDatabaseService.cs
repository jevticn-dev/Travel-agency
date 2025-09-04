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

        // citanje
        DataTable GetClients();
        DataTable GetPackages();
        DataTable GetReservationsForClient(int clientId);

        // azuriranje
        void UpdateClient(Client client);
        void UpdateReservation(Reservation reservation);

        // brisanje
        void DeleteReservation(int reservationId);

        // ostalo
        DataTable GetAvailableDestinations();
    }
}
