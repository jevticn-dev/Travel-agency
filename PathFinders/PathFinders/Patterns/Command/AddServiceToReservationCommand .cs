using PathFinders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Models;

namespace PathFinders.Patterns.Command
{
    public class AddServiceToReservationCommand : ICommand
    {
        private readonly IDatabaseService _databaseService;
        private readonly int _reservationId;
        private readonly int _serviceId;

        public AddServiceToReservationCommand(IDatabaseService databaseService, int reservationId, int serviceId)
        {
            _databaseService = databaseService;
            _reservationId = reservationId;
            _serviceId = serviceId;
        }

        public void Execute()
        {
            // Create a new ReservationServiceAssociation object
            var reservationService = new ReservationServiceAssociation
            {
                ReservationId = _reservationId,
                ServiceId = _serviceId
            };

            // Call the database service to add the association
            _databaseService.AddReservationService(reservationService);
        }
    }
}
