using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class DtoFolder : IEntity
    {
        public DtoFolder()
        {
            Files = new HashSet<DtoFile>();
            SharedToUsers = new HashSet<long>();
        }

        public long ID { get; set; }
        public long OwnerID { get; set; }
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public int Level { get; set; }
        public int LeftKey { get; set; }
        public int RightKey { get; set; }
        public ICollection<DtoFile> Files { get; set; }
        public ICollection<long> SharedToUsers { get; set; }
    }
}
