using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.DTO.Mapper
{
    public static class Map
    {
        public static Result<T> GetModelResult<T>(List<T> entity, bool status, string message, MetaData metaData = null)
        {
            var result = new Result<T>()
            {
                Data = entity,
                Succeeded = status,
                Message = message,
                MetaData = metaData
            };

            return result;
        }
    }
}
