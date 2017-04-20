using System;
using System.Collections.Generic;
using System.Text;

namespace VoidRatsAPI
{
    class Recipie
    {
        public Dictionary<Resource, double> Ingreddients;
        public Resource OutputResource;
        public double OutputQty;
        public double ProductionEnergy;
        public double ProductionTime;
        public List<int> RequiresAbilities;
    }
}
