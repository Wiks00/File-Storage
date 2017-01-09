using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class DalRole : IEntity
    {
        public long ID { get; set; }
        public string Role { get; set; }
    }
}
