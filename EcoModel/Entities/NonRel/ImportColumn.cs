using System;
using EcoManager.Data.Enums;

namespace EcoManager.Data.Entities.NonRel
{
    public class ImportColumn 
    {
        public Int32 ColumnNr { get; set; }
        public String Name { get; set; }
        public String OrigName { get; set; }
        public StorageTypes StorageType { get; set; }
        public int GroupId { get; set; }
        
        // Spatial
        public GeographyNames SpatialGeoType { get; set; }

        // Temporal
        public TimeTypes TemporalType { get; set; }
        public TimeEndings TemporalEndingType { get; set; }
        public String DateFormat { get; set; }

        public ImportColumn(int columnNr, string name)
        {
            ColumnNr = columnNr;
            Name = name;
            OrigName = name;
        }
    }
}
