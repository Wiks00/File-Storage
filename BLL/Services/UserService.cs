using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;

namespace BLL.Services
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork uow;
        private readonly IUserRepository userRepository;

        public UserService(IUnitOfWork uow, IUserRepository repository)
        {
            this.uow = uow;
            userRepository = repository;
        }

        public IEnumerable<DtoUser> GetAllUsers()
        {
            return userRepository.GetAll().Select(item => item.ToDtoUser());
        }

        public DtoUser GetUserById(long key)
        {
            return userRepository.GetById(key).ToDtoUser();
        }

        public void CreateUser(DtoUser e)
        {
            userRepository.Create(e.ToDalUser());
            uow.Commit();
        }

        public void DeleteUser(DtoUser e)
        {
            userRepository.Delete(e.ToDalUser());
            uow.Commit();
        }

        public void UpdateUser(DtoUser e)
        {
            userRepository.Update(e.ToDalUser());
            uow.Commit();
        }

        public void ChangePassword(DtoUser e)
        {
            var entityUser = GetUserById(e.ID);

            if (!ReferenceEquals(e.OldPassword, null) && !ReferenceEquals(e.NewPassword, null)
                && e.NewPassword == e.ConfirmPassword && Crypto.VerifyHashedPassword(entityUser.Password, e.OldPassword))
            {
                userRepository.Update(user => user.ID == e.ID ,user => user.Password ,Crypto.HashPassword(e.NewPassword));
                uow.Commit();
            }
        }

        public void ChangeLogin(string newLogin, long id)
        {
            userRepository.Update(user => user.ID == id , user => user.Login , newLogin);
            uow.Commit();
        }

        public void ChangeEmail(string newEmail, long id)
        {
            userRepository.Update(user => user.ID == id, user => user.Email, newEmail);
            uow.Commit();
        }

        public bool IsFreeEmail(string email)
        {
            return !userRepository.GetByPredicate(user => user.Email == email).Any();
        }

        public bool IsFreeLogin(string login)
        {
            return !userRepository.GetByPredicate(user => user.Email == login).Any();
        }
    }
}
