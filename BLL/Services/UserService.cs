﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.DTO;
using DAL.Interfaces;
using static BLL.Mappers.BllMapper;

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
            => userRepository.GetAll().Select(item => item.ToDtoUser());

        public DtoUser GetUserById(long key) 
            => userRepository.GetById(key).ToDtoUser();

        public IEnumerable<DtoUser> GetUserByPredicate(Expression<Func<DtoUser, bool>> func) 
            => userRepository.GetByPredicate(Convert<DtoUser, DalUser>(func)).Select(item => item.ToDtoUser());

        public DtoUser CreateUser(DtoUser e)
        {
            var user = userRepository.Create(e.ToDalUser());
            uow.Commit();

            return user.ToDtoUser();
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
            => !userRepository.GetByPredicate(user => user.Email == email).Any();

        public bool IsFreeLogin(string login) 
            => !userRepository.GetByPredicate(user => user.Login == login).Any();
    }
}
