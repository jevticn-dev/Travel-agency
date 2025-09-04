using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Builder
{
    public class CruiseTravelPackageBuilder : TravelPackageBuilder
    {
        private string _ship;
        private string _route;
        private DateTime _departureDate;
        private string _cabinType;

        public CruiseTravelPackageBuilder(string ship, string route, DateTime departureDate, string cabinType)
        {
            _ship = ship;
            _route = route;
            _departureDate = departureDate;
            _cabinType = cabinType;
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
            _package.Type = "Krstarenja";
        }

        public override void BuildDetails()
        {
            var details = new
            {
                Brod = _ship,
                Ruta = _route,
                DatumPolaska = _departureDate,
                TipKabine = _cabinType
            };
            _package.Details = JsonConvert.SerializeObject(details);
        }
    }
}
