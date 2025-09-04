using PathFinders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Patterns.Builder
{
    public abstract class TravelPackageBuilder
    {
        protected TravelPackage _package;

        public TravelPackage GetPackage()
        {
            return _package;
        }

        public void CreateNewPackage()
        {
            _package = new TravelPackage();
        }

        public abstract void BuildName(string name);
        public abstract void BuildPrice(decimal price);
        public abstract void BuildType();
        public abstract void BuildDetails();
    }
}
