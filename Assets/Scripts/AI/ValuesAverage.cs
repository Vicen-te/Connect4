using System.Collections.Generic;
using System.Linq;

namespace AI
{
    public class Average
    {
        private readonly List<double> nodesValue = new();
        
        private double AverageOfValues()
        {
            double sum = 0;
            foreach (var i in nodesValue) sum += i;
            return sum / nodesValue.Count;
        }

        public double Value => AverageOfValues();
        public void Add(double value)
        {
            nodesValue.Add(value);
        }
    }
}