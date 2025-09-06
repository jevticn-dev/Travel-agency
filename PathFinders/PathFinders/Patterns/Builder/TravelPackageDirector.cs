using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Builder
{
    public class TravelPackageDirector
    {
        public void Construct(TravelPackageBuilder builder, string name, decimal price, string destinationName)
        {
            builder.CreateNewPackage();
            builder.BuildName(name);
            builder.BuildPrice(price);
            builder.BuildType();
            builder.BuildDetails();
            builder.BuildDestinationName(destinationName);
        }
    }
}
