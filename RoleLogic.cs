using RBACDefinition.Model.Data;
using RBACDefinition.Persistence.SQLServer.DapperDataAccess;
using RBACDefinition.Persistence.SQLServer.DataAccessContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RBACDefinition.Business.Logic
{
    public class RoleLogic
    {
        #region Properties
        IRoleDA RoleDA { get; }
        #endregion

        #region Initialization
        public RoleLogic(IRoleDA roleDA )
        {
            RoleDA = roleDA;
        }
        #endregion

        #region Methods
        public async Task<IEnumerable<Role>> GetRoleAllAsync(IDbConnection conn, string tenantUID, CancellationToken cancellationToken = default )
        {
            var res = await RoleDA.GetAllAsync(conn, tenantUID, cancellationToken);
            return res.IfNullValue();
        }

        public async Task<IEnumerable<Role>> GetRoleAsync(IDbConnection conn, string tenantUID, IEnumerable<string> lstUIDs,  CancellationToken cancellationToken = default )
        {
            var res = await RoleDA.GetAsync(conn, tenantUID, lstUIDs, cancellationToken);
            return res.IfNullValue();
        }

        public async Task<Role> GetRoleByUIDAsync(IDbConnection conn,string tenantUID, string uid,  CancellationToken cancellationToken = default )
        {
            var res = await RoleDA.GetByUIDAsync(conn, tenantUID, uid, cancellationToken);
            return res;
        }

        public async Task MergeRoleAsync(IDbConnection conn, IEnumerable<Role> lst, string appUser, CancellationToken cancellationToken = default)
        {
            if (lst?.Count() > 0)
            {
                UtilsForLogic.ApplyEditedStateToRows(lst, appUser);
                await RoleDA.MergeAsync(conn, lst, cancellationToken);
            }
        }
        public async Task DeleteRoleAsync(IDbConnection conn, IEnumerable<string> lstUIDs)
        {
            if (lstUIDs?.Count() > 0)
            {
                await RoleDA.DeleteAsync(conn, lstUIDs);
            }
        }
        #endregion
    }
}
