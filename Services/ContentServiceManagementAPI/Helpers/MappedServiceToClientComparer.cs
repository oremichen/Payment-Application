using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Helpers
{
     public class MappedServiceToClientComparer : IEqualityComparer<MapServiceToClient>
    {
        public bool Equals(MapServiceToClient x, MapServiceToClient y)
        {
            // Two items are equal if their keys are equal.
            return x.ClientId == y.ClientId;
        }

        public int GetHashCode(MapServiceToClient obj)
        {
            return obj.ClientId.GetHashCode();
        }
    }
}
