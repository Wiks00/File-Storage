using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class DtoFile : IEntity
    {
        public DtoFile()
        {
            FileTypes = new HashSet<DtoFileType>();
        }

        public long ID { get; set; }
        public byte[] Data { get; set; }
        public DtoFolder Folder { get; set; }
        public DateTime DateTime { get; set; }
        public string Title { get; set; }
        public ICollection<DtoFileType> FileTypes { get; set; }
    }
}
