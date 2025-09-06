using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Models
{
    public class ReservationServiceAssociation
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int ServiceId { get; set; }
    }
}
