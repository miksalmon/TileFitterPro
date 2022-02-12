using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFitter.Models
{
    internal class HeuristicMetrics
    {
        public HeuristicMetrics(int primaryMetric, int secondaryMetric)
        {
            PrimaryMetric = primaryMetric;
            SecondaryMetric = secondaryMetric;
        }

        public int PrimaryMetric { get; set; }

        public int SecondaryMetric { get; set; }

        // Optimization problem so we try to minimize metric prioritizing PrimaryMetric then SecondaryMetric
        public virtual bool IsBetter(HeuristicMetrics otherMetrics)
        {
            return PrimaryMetric < otherMetrics.PrimaryMetric || (PrimaryMetric == otherMetrics.PrimaryMetric && SecondaryMetric < otherMetrics.SecondaryMetric);
        }
    }
}
