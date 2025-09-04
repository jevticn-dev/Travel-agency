using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Builder
{
    public class MountainTravelPackageBuilder : TravelPackageBuilder
    {
        private string _destination;
        private string _transportType;
        private string _accommodationType;
        private List<string> _activities;

        public MountainTravelPackageBuilder(string destination, string transportType, string accommodationType, List<string> activities)
        {
            _destination = destination;
            _transportType = transportType;
            _accommodationType = accommodationType;
            _activities = activities;
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
            _package.Type = "Planine";
        }

        public override void BuildDetails()
        {
            var details = new
            {
                Destinacija = _destination,
                TipPrevoza = _transportType,
                TipSmestaja = _accommodationType,
                DodatneAktivnosti = _activities
            };
            _package.Details = JsonConvert.SerializeObject(details);
        }
    }
}
