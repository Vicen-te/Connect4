using System.Collections.Generic;

namespace AI
{
    public class Average
    {
        private readonly List<double> _nodesValue = new();
        
        private double AverageOfValues()
        {
            double sum = 0;
            foreach (var i in _nodesValue) sum += i;
            return sum / _nodesValue.Count;
        }

        public double Value => AverageOfValues();
        public void Add(double value)
        {
            _nodesValue.Add(value);
        }
    }
}