using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class DalFile : IEntity
    {
        public DalFile()
        {
            FileTypes = new HashSet<DalFileType>();
        }

        public long ID { get; set; }
        public byte[] Data { get; set; }
        public long folderID { get; set; }
        public DateTime DateTime { get; set; }
        public string Title { get; set; }
        public ICollection<DalFileType> FileTypes { get; set; }
    }
}
