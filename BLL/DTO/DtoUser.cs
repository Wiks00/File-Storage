using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.DTO
{
    public class DtoUser : IEntity
    {
        public DtoUser()
        {
            Folders = new HashSet<DtoFolder>();
            Roles = new HashSet<DtoRole>();
            SharedFolders = new HashSet<DtoFolder>();
        }

        public long ID { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public ICollection<DtoFolder> Folders { get; set; }
        public ICollection<DtoRole> Roles { get; set; }
        public ICollection<DtoFolder> SharedFolders { get; set; }
    }
}
