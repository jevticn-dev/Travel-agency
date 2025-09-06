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

        public void AddReservation(Reservation reservation)
        {
            _databaseService.AddReservation(reservation);
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
            DataTable reservationsTable = _databaseService.GetReservationsForClient(clientId);
            var reservations = new List<Reservation>();

            foreach (DataRow row in reservationsTable.Rows)
            {
                var reservation = new Reservation
                {
                    Id = row.Field<int>("ID"),
                    ClientId = row.Field<int>("KlijentID"),
                    TravelPackageId = row.Field<int>("PaketID"),
                    ReservationDate = row.Field<DateTime>("Datum_rezervacije"),
                    NumberOfPeople = row.Field<int>("Broj_osoba")
                };

                // Fetch and populate the related objects
                reservation.Package = _databaseService.GetPackageById(reservation.TravelPackageId);
                reservation.AdditionalServices = _databaseService.GetServicesForReservation(reservation.Id);

                reservations.Add(reservation);
            }
            return reservations;
        }

        public decimal GetTotalPrice(int reservationId)
        {
            var reservation = _databaseService.GetReservationById(reservationId);
            List<Service> services = _databaseService.GetServicesForReservation(reservationId);

            var decorator = new ReservationServiceDecorator(reservation, services);

            return decorator.GetTotalPrice();
        }

        public void UpdateReservation(Reservation reservation)
        {
            _databaseService.UpdateReservation(reservation);
        }

        public void DeleteReservation(int reservationId)
        {
            // 1. Prvo obrišite sve usluge povezane s tom rezervacijom
            _databaseService.DeleteServicesForReservation(reservationId);

            // 2. Nakon toga, obrišite samu rezervaciju
            _databaseService.DeleteReservation(reservationId);
        }
    }
}
