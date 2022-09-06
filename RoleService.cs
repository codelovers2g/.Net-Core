using RBACDefinition.Business.Logic;
using RBACDefinition.Model.Data;
using RBACDefinition.Model.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RBACDefinition.Business.Service
{
    public class RoleService : BaseService, IRoleService
    {
        private RoleLogic RoleLogic { get; set; }
        private RoleManagerLogic RoleManagerLogic { get; set; }

        #region Initialization
        public RoleService( RBACDefinitionExecutionContext executionContext, RoleLogic roleLogic, RoleManagerLogic roleManagerLogic) : base( executionContext )
        {
            RoleLogic = roleLogic;
            RoleManagerLogic = roleManagerLogic;
        }
        #endregion

        #region IRoleService Implementation
        public async Task<IEnumerable<Role>> GetRoleAllAsync( string tenantUID, CancellationToken cancellationToken = default )
        {
            var res = await WithReadTransaction( async conn => await RoleLogic.GetRoleAllAsync(conn, tenantUID, cancellationToken));
            return res;
        }

        public async Task<IEnumerable<Role>> GetRoleAsync(string tenantUID, IEnumerable<string> lstUIDs, CancellationToken cancellationToken = default )
        {
            var res = await WithReadTransaction( async conn => await RoleLogic.GetRoleAsync(conn, tenantUID, lstUIDs, cancellationToken));
            return res;
        }

        public async Task<Role> GetRoleByUIDAsync(string tenantUID, string uid, CancellationToken cancellationToken = default )
        {
            var res = await WithReadTransaction(async conn => await RoleLogic.GetRoleByUIDAsync(conn, tenantUID, uid, cancellationToken));
            return res;
        }

        public async Task<Dictionary<string, bool>> GetUserPermissionGroupsAsync(string tenantUID, string userUID, CancellationToken cancellationToken = default)
        {
            var res = await WithReadTransaction(async conn => await RoleManagerLogic.GetUserPermissionGroupsAsync(conn, tenantUID, userUID, cancellationToken));
            return res;
        }

        public async Task MergeRoleAsync(IEnumerable<Role> lst, CancellationToken cancellationToken = default)
        {
            await WithWriteTransaction(async (conn, appUser) => await RoleLogic.MergeRoleAsync(conn, lst, appUser, cancellationToken));
        }

        public async Task DeleteRoleAsync( IEnumerable<string> lstUIDs )
        {
            await WithWriteTransaction(async (conn, appUser) => await RoleLogic.DeleteRoleAsync(conn, lstUIDs));
        }
        #endregion
    }
}
