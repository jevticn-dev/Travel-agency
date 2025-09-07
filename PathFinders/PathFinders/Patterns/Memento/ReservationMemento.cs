using PathFinders.Models;
using PathFinders.Patterns.Momento;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Memento
{
    internal class ReservationMemento : IMemento<Reservation>
    {
        private readonly Reservation reservationState;

        private ReservationMemento(Reservation state)
        {
            reservationState = new Reservation
            {
                Id = state.Id,
                ClientId = state.ClientId,
                TravelPackageId = state.TravelPackageId,
                Package = state.Package,
                ReservationDate = state.ReservationDate, 
                NumberOfPeople = state.NumberOfPeople,
                AdditionalServices = state.AdditionalServices, 
            };
        }

        public Reservation GetState()
        {
            return reservationState;
        }

        public static ReservationMemento Create(Reservation reservation)
        {
            return new ReservationMemento(reservation);
        }
    }
}
