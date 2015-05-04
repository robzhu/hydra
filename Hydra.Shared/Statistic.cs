using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydra.Shared
{
    public class Statistic
    {
        public int LifetimeSampleSize { get; set; }
        public double LifetimeAverage { get; set; }
        public double LifetimeMax { get; set; }
        public double LifetimeMin { get; set; }

        public int RunningSampleSize { get; set; }
        public double RunningAverage { get; set; }
        public double RunningMax { get; set; }
        public double RunningMin { get; set; }
        public double RunningStandardDeviation { get; set; }
        private List<double> RunningSamples = new List<double>();

        public double LastSample { get; set; }

        public Statistic()
        {
            RunningSampleSize = 10;
            LifetimeMin = -1;
        }

        public void AddSample( double sample )
        {
            RecalculateLifetimeValues( sample );
            RecalculateRunningValues( sample );
            LastSample = sample;
        }

        private void RecalculateLifetimeValues( double sample )
        {
            var previousTotal = LifetimeAverage * LifetimeSampleSize;
            LifetimeSampleSize++;
            LifetimeAverage = ( previousTotal + sample ) / LifetimeSampleSize;

            if( sample > LifetimeMax ) LifetimeMax = sample;

            if( LifetimeMin == -1 ) LifetimeMin = sample;
            if( sample < LifetimeMin ) LifetimeMin = sample;
        }

        private void RecalculateRunningValues( double sample )
        {
            if( RunningSamples.Count == RunningSampleSize )
            {
                RunningSamples.RemoveAt( 0 );
            }
            RunningSamples.Add( sample );
            RunningAverage = RunningSamples.Average();
            RunningMax = RunningSamples.Max();
            RunningMin = RunningSamples.Min();

            double sumOfSquaresOfDifferences = RunningSamples.Select( val => ( val - RunningAverage ) * ( val - RunningAverage ) ).Sum();
            RunningStandardDeviation = Math.Sqrt( sumOfSquaresOfDifferences / RunningSamples.Count );
        }
    }
}
