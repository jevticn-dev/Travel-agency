using PathFinders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Decorator
{
    public class ReservationServiceDecorator : IPriceable
    {
        private readonly Reservation _reservation;
        private readonly List<Service> _additionalServices;

        public ReservationServiceDecorator(Reservation reservation, List<Service> additionalServices)
        {
            _reservation = reservation;
            _additionalServices = additionalServices;
        }

        public decimal GetTotalPrice()
        {
            // Start with the base price from the wrapped Reservation object
            decimal totalPrice = _reservation.GetTotalPrice();

            // Add the price of each additional service
            totalPrice += _additionalServices.Sum(s => s.ServicePrice);

            return totalPrice;
        }
    }
}
