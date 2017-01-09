using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class DalFolder : IEntity
    {
        public DalFolder()
        {
            Files = new HashSet<DalFile>();
            SharedToUsers = new HashSet<DalUser>();
        }

        public long ID { get; set; }
        public DalUser Owner { get; set; }
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public int Level { get; set; }
        public int LeftKey { get; set; }
        public int RightKey { get; set; }
        public ICollection<DalFile> Files { get; set; }
        public ICollection<DalUser> SharedToUsers { get; set; }
    }
}
