using PathFinders.Models;
using PathFinders.Patterns.Builder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Services
{
    public class PackageService
    {
        private readonly IDatabaseService _databaseService;

        public PackageService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddPackage(TravelPackageBuilder builder, string name, decimal price, string destinationName)
        {
            var director = new TravelPackageDirector();
            director.Construct(builder, name, price, destinationName);

            var package = builder.GetPackage();

            // Get or create the destination in the database and set the foreign key
            int destinationId = _databaseService.GetOrCreateDestination(destinationName);
            package.DestinationId = destinationId;

            _databaseService.AddPackage(package);
        }

        public List<TravelPackage> GetPackages()
        {
            DataTable packagesTable = _databaseService.GetPackages();
            var packages = new List<TravelPackage>();

            foreach (DataRow row in packagesTable.Rows)
            {
                packages.Add(new TravelPackage
                {
                    Id = row.Field<int>("ID"),
                    Name = row.Field<string>("Naziv"),
                    Price = row.Field<decimal>("Cena"),
                    Type = row.Field<string>("Tip"),
                    DestinationId = row.Field<int>("DestinacijaID"),
                    Details = row.Field<string>("Detalji")
                });
            }
            return packages;
        }
    }
}
