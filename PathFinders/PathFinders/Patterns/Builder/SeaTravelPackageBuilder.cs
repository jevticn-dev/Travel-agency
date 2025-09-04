using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace PathFinders.Patterns.Builder
{
    public class SeaTravelPackageBuilder : TravelPackageBuilder
    {
        private string _destination;
        private string _transportType;
        private string _accommodationType;

        public SeaTravelPackageBuilder(string destination, string transportType, string accommodationType)
        {
            _destination = destination;
            _transportType = transportType;
            _accommodationType = accommodationType;
        }

        public override void BuildName(string name)
        {
            _package.Name = name;
        }

        public override void BuildPrice(decimal price)
        {
            _package.Price = price;
        }

        public override void BuildType()
        {
            _package.Type = "More";
        }

        public override void BuildDetails()
        {
            // detalji se serijalizuju u JSON string
            var details = new
            {
                Destinacija = _destination,
                TipPrevoza = _transportType,
                TipSmestaja = _accommodationType
            };
            _package.Details = JsonConvert.SerializeObject(details);
        }
    }
}
