using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Models
{
    public class TravelPackage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public int? DestinationId { get; set; } // The foreign key stored in the database
        public string DestinationName { get; set; } // Used by the Builder to provide the destination name
        public string Details { get; set; }
    }
}
