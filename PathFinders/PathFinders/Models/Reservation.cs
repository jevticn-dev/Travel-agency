using PathFinders.Patterns.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Models
{
    public class Reservation : IPriceable
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int TravelPackageId { get; set; }
        public TravelPackage Package { get; set; }
        public DateTime ReservationDate { get; set; }
        public int NumberOfPeople { get; set; } // Added to store the number of passengers
        public List<Service> AdditionalServices { get; set; } // Added to hold the additional services

        // The base price is the package price
        public decimal GetTotalPrice()
        {
            return Package.Price * NumberOfPeople;
        }

        internal ReservationMemento SaveState()
        {
            // Pozivamo statičku metodu Memento klase
            return ReservationMemento.Create(this);
        }

        internal void Restore(ReservationMemento memento)
        {
            var state = memento.GetState();
            this.Id = state.Id;
            this.ClientId = state.ClientId;
            this.TravelPackageId = state.TravelPackageId;
            this.Package = state.Package;
            this.ReservationDate = state.ReservationDate;
            this.NumberOfPeople = state.NumberOfPeople;
            this.AdditionalServices = state.AdditionalServices;
        }
    }
}
