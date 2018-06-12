using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Store.ServiceContracts;
using Store.ServiceContracts.ModelDTOs;
using Microsoft.Practices.Unity;
using Store.Infrastructure;
namespace Store.Service.Wcf
{
    /* 对于WCF中的异常与错误处理我们必须先了解一个概念SOAP Faults，它实际上就是在服务端和客户端之间来传递错误信息的一种载体。

        • 公共语言运行时（CLR）异常无法跨越服务边界
        – 未捕捉异常最多到达服务通道（service channel）
        – 在报告给客户端之前必须要进行序列化
        • 所有的异常都被序列化为SOAP faults
–        基于标准的，可互操作的
    */

    // FaultException可序列化错误详细信息类型。
    // 更多关于 FaultException 参见：（WCF开发之异常与错误处理 ）http://blog.csdn.net/linux7985/article/details/7669511 
    // 或 https://msdn.microsoft.com/zh-cn/library/ms576199.aspx
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class UserService : IUserService
    {
        private readonly IUserService _userServiceImp;

        [InjectionConstructor]
        public UserService()
        {
            this._userServiceImp = ServiceLocator.Instance.GetService<IUserService>();
        }


        #region IUservice Methods
        public IList<UserDto> CreateUsers(List<UserDto> userDtos)
        {
            try
            {
                return this._userServiceImp.CreateUsers(userDtos);
            }
            catch (Exception e)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(e), FaultData.CreateFaultReason(e));
            }
        }
        public bool ValidateUser(string userName, string password)
        {
            //return this._userServiceImp.ValidateUser(userName,password);
            try
            {
                return _userServiceImp.ValidateUser(userName, password);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public UserDto GetUserByKey(Guid id)
        {
            try
            {
                return _userServiceImp.GetUserByKey(id);
            }
            catch (Exception e)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(e), FaultData.CreateFaultReason(e));
            }
        }

        public UserDto GetUserByEmail(string email)
        {
            try
            {
                return _userServiceImp.GetUserByEmail(email);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public UserDto GetUserByName(string userName)
        {
            try
            {
                return _userServiceImp.GetUserByName(userName);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public bool DisableUser(UserDto userDto)
        {
            try
            {
                return _userServiceImp.DisableUser(userDto);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public bool EnableUser(UserDto userDto)
        {
            try
            {
                return _userServiceImp.EnableUser(userDto);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }
        public void DeleteUsers(List<UserDto> userDtos)
        {
            try
            {
                _userServiceImp.DeleteUsers(userDtos);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public RoleDto GetRoleByUserName(string userName)
        {
            try
            {
                return _userServiceImp.GetRoleByUserName(userName);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }
        #endregion


        //public void DoWork()
        //{
        //}
    }
}
