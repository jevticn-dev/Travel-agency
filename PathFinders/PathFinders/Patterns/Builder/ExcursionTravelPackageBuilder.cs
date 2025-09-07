using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Builder
{
    public class ExcursionTravelPackageBuilder : TravelPackageBuilder
    {
        private string _destination;
        private string _transportType;
        private string _guide;
        private int _durationInDays;

        public ExcursionTravelPackageBuilder(string destination, string transportType, string guide, int durationInDays)
        {
            _destination = destination;
            _transportType = transportType;
            _guide = guide;
            _durationInDays = durationInDays;
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
            _package.Type = "Ekskurzije";
        }

        public override void BuildDetails()
        {
            var details = new
            {
                Destinacija = _destination,
                TipPrevoza = _transportType,
                Vodic = _guide,
                TrajanjeUDanima = _durationInDays
            };
            _package.Details = JsonConvert.SerializeObject(details);
        }

        public override void BuildDestinationName(string destinationName)
        {
            _package.DestinationName = destinationName;
        }
    }
}
