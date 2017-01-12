using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO;
using ORM;

namespace DAL.Interfaces
{
    public interface IFileRepository : IRepository<DalFile>
    {
    }
}
