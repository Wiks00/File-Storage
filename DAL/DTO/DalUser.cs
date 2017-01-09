using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class DalUser : IEntity
    {
        public DalUser()
        {
            Folders = new HashSet<DalFolder>();
            Roles = new HashSet<DalRole>();
            SharedFolders = new HashSet<DalFolder>();
        }

        public long ID { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public ICollection<DalFolder> Folders { get; set; }
        public ICollection<DalRole> Roles { get; set; }
        public ICollection<DalFolder> SharedFolders { get; set; }
    }
}
