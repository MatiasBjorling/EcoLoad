using System;

namespace EcoModel.Entities.Interfaces
{
    public interface IAuditable
    {
        DateTime Oprettet { get; set;}
        DateTime? Rettet { get; set;}
    }
}
