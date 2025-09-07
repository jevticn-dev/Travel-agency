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
    internal class DeleteReservationCommand : ICommand
    {
        private readonly IDatabaseService databaseService;
        private readonly ReservationMemento reservationMemento;
        private int deletedReservationId;

        public DeleteReservationCommand(IDatabaseService databaseService, int reservationId)
        {
            this.databaseService = databaseService;
            var reservationDelete = databaseService.GetReservationById(reservationId);
            reservationMemento = reservationDelete.SaveState();
            deletedReservationId = reservationId;
        }
        public void Execute()
        {
            databaseService.DeleteReservation(deletedReservationId);
        }

        public void Undo()
        {
            var reservation = new Reservation();
            reservation.Restore(reservationMemento);
            deletedReservationId = databaseService.AddReservation(reservation);
        }

        public void Redo()
        {
            if(deletedReservationId > 0)
            {
                databaseService.DeleteReservation(deletedReservationId);
            }
            
        }

        
    }
}
