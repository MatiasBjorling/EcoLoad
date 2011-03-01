using System;
using EcoManager.Data.Enums;

namespace EcoManager.Data.Entities
{
    public class SchemaColumn
    {
        public virtual int Id { get; set; }
        public virtual SchemaInfo Schema { get; set; }
        public virtual string Name { get; set; }
        public virtual string OrigName { get; set; }
        public virtual int ColOrder { get; set; }
        public virtual StorageTypes Type { get; set; }
        public virtual int GroupId { get; set; }

        

        // Spatial
        public virtual GeographyNames SpatialGeoType { get; set; }

        // Temporal
        public virtual TimeTypes TemporalType { get; set; }
        public virtual TimeEndings TemporalEndingType { get; set; }
        public virtual String DateFormat { get; set; }

        public virtual DateTime ValidBegin { get; set; }
        public virtual DateTime? ValidEnd { get; set; }

    }
}
