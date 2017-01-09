using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class DtoFileType : IEntity
    {
        public long ID { get; set; }
        public string TypeName { get; set; }
        public string Format { get; set; }
    }
}
