using System;
using EcoManager.Data.Entities;
using EcoManager.Data.Enums;

namespace EcoManager.Data.Factories
{
    public static class TemporalFactory
    {
        public static TemporalBase GetNewTemporal(TimeTypes timeType)
        {
            TemporalBase tb;
            switch (timeType)
            {
                case TimeTypes.Point:
                    tb = new TemporalPoint();
                    break;
                case TimeTypes.Interval:
                    tb = new TemporalInterval();
                    break;
                case TimeTypes.Length:
                    tb = new TemporalLength();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("timeType");
            }

            return tb;
        }
    }
}
