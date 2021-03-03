using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.Users.Dto;
using ThemePark.ApplicationDto.Users;
using ThemePark.Core.Agencies;
using ThemePark.Core.Authorization.Users;
using ThemePark.Core.Users;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Users
{
    /// <summary>
    /// Interface IUserAppService
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService"/>
    public interface IUserAppService : IApplicationService, IUserInfoService
    {
        #region Methods

        /// <summary>
        /// ����ϵͳ�û�
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task.</returns>
        Task<Result> AddSysUserAsync(CreateUserInput input);

        /// <summary>
        /// ����û�ϲ�ò˵��б�
        /// </summary>
        /// <returns></returns>
        Task<Result> AddUserPreferMenuAsnyc(string menuName, string permissionName);

        /// <summary>
        /// ɾ���û�ϲ�ò˵��б�
        /// </summary>
        /// <returns></returns>
        Task<Result> DeleteUserPreferMenuAsnyc(string entityName, string permissionName);

        /// <summary>
        /// ��ȡ�������û���Ϣ
        /// </summary>
        Task<TDto> GetAgencyUserAsync<TDto>(IQuery<AgencyUser> query);

        /// <summary>
        /// ����Id��ȡϵͳ�û�
        /// </summary>
        /// <typeparam name="TDto">The type of the return.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;TDto&gt;.</returns>
        Task<TDto> GetSysUserByIdAsync<TDto>(long id);

        /// <summary>
        /// ɾ��ϵͳ�û�
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> DeleteSysUserAsync(long id);

        /// <summary>
        /// ɾ���������û�
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> DeleteAgencyUserAsync(long id);

        /// <summary>
        /// ϵͳ�û���ҳ��ѯ����
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        Task<PageResult<TDto>> GetSysUsersByPageAsync<TDto>(IPageQuery<SysUser> pageQuery);

        /// <summary>
        /// ϵͳ�û���ҳ��ѯ��������������Ȩ�޹��ˣ�ֻ��ʾ��ǰ�û�ͬ�����¼�����δ��������Ȩ�޵��û�
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        Task<PageResult<TDto>> GetDataPermissionSysUsersByPageAsync<TDto>(IPageQuery<SysUser> pageQuery);

        /// <summary>
        /// �����û���ҳ��ѯ����
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="pageQuery">The page query.</param>
        /// <returns>Task&lt;PageResult&lt;TDto&gt;&gt;.</returns>
        Task<PageResult<TDto>> GetAllUsersByPageAsync<TDto>(IPageQuery<User> pageQuery);

        /// <summary>
        /// ����������ȡ�û�ϲ�ò˵��б�
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetUserPreferMenusListAsnyc<TDto>(IQuery<UserPreferMenu> query);
        
        /// <summary>
        /// Removes from role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>Task.</returns>
        Task RemoveFromRole(long userId, string roleName);

        /// <summary>
        /// �޸�����
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdatePasswordAsync(UpdatePasswordInput input);

        /// <summary>
        /// �޸�����
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> ResetPasswordAsync(ResetPasswordInput input);

        /// <summary>
        /// �޸��ֻ���
        /// </summary>
        Task<Result> UpdatePhoneByOldPhoneAsync(long userId, UpdatePhoneInput input);

        /// <summary>
        /// �޸��û���Ϣ
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdateUserAsync(ApplicationDto.Users.UpdateUserInput input);

        /// <summary>
        /// �޸��û�����Ȩ��
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdateUserDataPermissionAsync(UpdateUserDataPermissionInput input);

        /// <summary>
        /// �޸��û���ɫ
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdateUserRolesAsync(UpdateUserRoleInput input);

        /// <summary>
        /// ���տͻ����û��޸����루����ͬ����
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> UpdatePasswordDatasync(ChangePasswordDto input);

        #endregion Methods
    }
}