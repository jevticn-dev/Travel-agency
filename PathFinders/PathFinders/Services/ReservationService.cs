using PathFinders.Models;
using PathFinders.Patterns.Command;
using PathFinders.Patterns.Decorator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Services
{
    public class ReservationService
    {
        private readonly IDatabaseService _databaseService;

        public ReservationService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public int AddReservation(Reservation reservation)
        {
            return _databaseService.AddReservation(reservation);
        }

        public void AddServiceToReservation(int reservationId, int serviceId)
        {
            var addServiceCommand = new AddServiceToReservationCommand(_databaseService, reservationId, serviceId);
            var invoker = new AddReservationServiceInvoker();
            invoker.SetCommand(addServiceCommand);
            invoker.ExecuteCommand();
        }

        public List<Reservation> GetReservationsForClient(int clientId)
        {
            // Now directly returns a List<Reservation>, no DataTable conversion is needed
            return _databaseService.GetReservationsForClient(clientId);
        }

        public decimal GetTotalPrice(int reservationId)
        {
            var reservation = _databaseService.GetReservationById(reservationId);
            List<Service> services = _databaseService.GetServicesForReservation(reservationId);

            var decorator = new ReservationServiceDecorator(reservation, services);

            return decorator.GetTotalPrice();
        }

        public int UpdateReservation(Reservation reservation)
        {
            return _databaseService.UpdateReservation(reservation);
        }

        public void DeleteReservation(int reservationId)
        {
            // 1. Prvo obrišite sve usluge povezane s tom rezervacijom
            _databaseService.DeleteServicesForReservation(reservationId);

            // 2. Nakon toga, obrišite samu rezervaciju
            _databaseService.DeleteReservation(reservationId);
        }

        public void DeleteReservationsForPackage(int packageId)
        {
            // Pronađite sve rezervacije koje se odnose na ovaj paket
            var reservations = _databaseService.GetReservationsForPackage(packageId);

            // Za svaku rezervaciju, pozovite postojeću metodu za brisanje
            foreach (var reservation in reservations)
            {
                DeleteReservation(reservation.Id); // Ponovo koristimo vašu već implementiranu logiku
            }
        }
    }
}