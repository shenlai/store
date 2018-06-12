using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Store.ServiceContracts;
using Store.Domain.Repositories;
using Store.ServiceContracts.ModelDTOs;
using Store.Domain.Model;
using AutoMapper;
using Store.Domain.Services;
using Store.Domain.Specifications;

namespace Store.Application.ServiceImplementations
{
    public class UserServiceImpl : ApplicationService,IUserService
    {
        public readonly IUserRepository _userReposity;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        //private readonly IDomainService _domainService;

        public UserServiceImpl(IRepositoryContext repositoryContext, 
            IUserRepository userRepository, 
            IShoppingCartRepository shoppingCartRepository, 
            //IDomainService domainService, 
            IRoleRepository roleRepository, 
            IUserRoleRepository userRoleRepository)
            : base(repositoryContext)
        {
            _userReposity = userRepository;
            _shoppingCartRepository = shoppingCartRepository;
            //_domainService = domainService;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        #region IUservice Methods

        public IList<UserDto> CreateUsers(List<UserDto> userDtos)
        {
            if (userDtos == null)
                throw new ArgumentNullException("userDtos");
            return PerformCreateObjects<List<UserDto>, UserDto, User>(userDtos,
                _userReposity,
                dto =>
                {
                    if (dto.RegisteredDate == null)
                        dto.RegisteredDate = DateTime.Now;
                },
                ar =>
                {
                    var shoppingCart = ar.CreateShoppingCart();
                    _shoppingCartRepository.Add(shoppingCart);
                });
        }
        public bool ValidateUser(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            return this._userReposity.CheckPassword(userName, password);
        }

        public UserDto GetUserByKey(Guid id)
        {
            var user = this._userReposity.GetByKey(id);
            var userDto = Mapper.Map<User, UserDto>(user);
            return userDto;
        }


        public UserDto GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("email");
            var user = this._userReposity.GetByExpression(u => u.Email == email);
            var userDto = Mapper.Map<User, UserDto>(user);
            return new UserDto();
        }

        public UserDto GetUserByName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("userName");
            var user = this._userReposity.GetByExpression(u => u.UserName == userName);
            var userDto = Mapper.Map<User, UserDto>(user);
            return userDto;
        }

        public bool DisableUser(UserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentException("userDto");
            User user;
            if (!IsEmptyGuidString(userDto.Id))
                user = this._userReposity.GetByKey(new Guid(userDto.Id));
            else if (!string.IsNullOrEmpty(userDto.UserName))
                user = this._userReposity.GetByExpression(u => u.UserName == userDto.UserName);
            else if (!string.IsNullOrEmpty(userDto.Email))
                user = this._userReposity.GetByExpression(u => u.Email == userDto.Email);
            else
                throw new ArgumentNullException("userDto", "Either ID, UserName or Email should be specified.");
            user.Disable();
            this._userReposity.Update(user);
            RepositoryContext.Commit();
            return user.IsDisabled;
        }

        public bool EnableUser(UserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException("userDto");
            User user;
            if (!IsEmptyGuidString(userDto.Id))
                user = _userReposity.GetByKey(new Guid(userDto.Id));
            else if (!string.IsNullOrEmpty(userDto.UserName))
                user = _userReposity.GetByExpression(u => u.UserName == userDto.UserName);
            else if (!string.IsNullOrEmpty(userDto.Email))
                user = _userReposity.GetByExpression(u => u.Email == userDto.Email);
            else
                throw new ArgumentNullException("userDto", "Either ID, UserName or Email should be specified.");
            user.Enable();
            _userReposity.Update(user);
            RepositoryContext.Commit();
            return user.IsDisabled;
        }


        public void DeleteUsers(List<UserDto> userDtos)
        {
            if (userDtos == null)
                throw new ArgumentNullException("userDtos");
            foreach (var userDto in userDtos)
            {
                User user = null;
                if (!IsEmptyGuidString(userDto.Id))
                    user = _userReposity.GetByKey(new Guid(userDto.Id));
                else if (!string.IsNullOrEmpty(userDto.UserName))
                    user = _userReposity.GetByExpression(u => u.UserName == userDto.UserName);
                else if (!string.IsNullOrEmpty(userDto.Email))
                    user = _userReposity.GetByExpression(u => u.Email == userDto.Email);
                else
                    throw new ArgumentNullException("userDtos", "Either ID, UserName or Email should be specified.");
                var userRole = _userRoleRepository.GetBySpecification(Specification<UserRole>.Eval(ur => ur.UserId == user.Id));
                if (userRole != null)
                    _userRoleRepository.Remove(userRole);
                _userReposity.Remove(user);
            }       
            RepositoryContext.Commit();
        }


        //IList<UserDto> UpdateUsers(List<UserDto> userDataObjects);

        public RoleDto GetRoleByUserName(string userName)
        {
            if(string.IsNullOrEmpty(userName))
                throw new ArgumentException("userName"); 
            var user = _userReposity.GetByExpression(u=>u.UserName==userName);
            var role = _userRoleRepository.GetRoleForUser(user);
            return Mapper.Map<Role, RoleDto>(role);
        }
        #endregion

    }
}