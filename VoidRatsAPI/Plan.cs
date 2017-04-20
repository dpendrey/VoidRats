using System;
using System.Collections.Generic;
using System.Text;

namespace VoidRatsAPI
{
    public class Plan
    {
        public Dictionary<Resource, double> ResourcesIn;
        public Dictionary<Module, int> ItemsIn;
        public Dictionary<Module, int> ItemsOut;
        public double EnergyRequired;
        public double TimeRequired;
        public List<Ability> AbilitiesRequired;
    }
}
