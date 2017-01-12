using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;

namespace BLL.Services
{
    public class RoleService : IRoleService
    {

        private readonly IUnitOfWork uow;
        private readonly IRoleRepository roleRepository;

        public RoleService(IUnitOfWork uow, IRoleRepository repository)
        {
            this.uow = uow;
            roleRepository = repository;
        }

        public IEnumerable<DtoRole> GetAllRoles()
        {
            return roleRepository.GetAll().Select(item => item.ToDtoRole());
        }

        public DtoRole GetRolesByTitle(string title)
        {
            return
                roleRepository.GetByPredicate(role => role.Role.ToLower().Contains(title.ToLower()))
                    .FirstOrDefault()
                    .ToDtoRole();
        }

        public void CreateRole(DtoRole e)
        {
            roleRepository.Create(e.ToDalRole());
            uow.Commit();
        }

        public void DeleteRole(DtoRole e)
        {
            roleRepository.Delete(e.ToDalRole());
            uow.Commit();
        }

        public void UpdateRole(DtoRole e)
        {
            roleRepository.Update(e.ToDalRole());
            uow.Commit();
        }
    }
}
