using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class DtoRole : IEntity
    {
        public long ID { get; set; }
        public string Role { get; set; }
    }
}
