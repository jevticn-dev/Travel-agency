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
    internal class AddReservationCommand : ICommand
    {
        private readonly IDatabaseService databaseService;
        private readonly ReservationMemento reservationMemento; 
        private int newReservationId;

        public AddReservationCommand(IDatabaseService databaseService, Reservation reservation)
        {
            this.databaseService = databaseService;
            this.reservationMemento = reservation.SaveState();
        }

        public void Execute()
        {
            var reservation = new Reservation();
            reservation.Restore(reservationMemento);
            newReservationId = databaseService.AddReservation(reservation);
        }

        public void Redo()
        {
            var reservation = new Reservation();
            reservation.Restore(reservationMemento);
            newReservationId = databaseService.AddReservation(reservation);
        }

        public void Undo()
        {
            if(newReservationId > 0)
            {
                databaseService.DeleteReservation(newReservationId);
            }
        }
    }
}
