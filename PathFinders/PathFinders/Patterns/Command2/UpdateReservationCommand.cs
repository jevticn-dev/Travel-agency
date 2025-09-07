using PathFinders.Models;
using PathFinders.Patterns.Memento;
using PathFinders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Command2
{
    internal class UpdateReservationCommand : ICommand
    {
        private readonly IDatabaseService databaseService;
        private readonly ReservationMemento reservationMementoBefore;
        private readonly ReservationMemento reservationMementoAfter;
        
        public UpdateReservationCommand(IDatabaseService db, int id, int clientId, int travelPackageId, TravelPackage package, DateTime reservationDate, int numberOfPeople, List<Service> additionalServices)
        {
            this.databaseService = db;
            var current = databaseService.GetReservationById(id);
            reservationMementoBefore = current.SaveState();

            var updated = new Reservation
            {
                Id = id,
                ClientId = clientId,
                TravelPackageId = travelPackageId,
                Package = package,
                ReservationDate = reservationDate,
                NumberOfPeople = numberOfPeople,
                AdditionalServices = additionalServices
            };
            reservationMementoAfter = updated.SaveState();
        }

        public void Execute()
        {
            var updated = new Reservation();
            updated.Restore(reservationMementoAfter);
            databaseService.UpdateReservation(updated);
        }

        public void Redo()
        {
            var after = new Reservation();
            after.Restore(reservationMementoAfter);
            databaseService.UpdateReservation(after);
        }

        public void Undo()
        {
            var before = new Reservation();
            before.Restore(reservationMementoBefore);
            databaseService.UpdateReservation(before);
        }
    }
}
