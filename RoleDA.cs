using Dapper;
using RBACDefinition.Model.Data;
using RBACDefinition.Persistence.SQLServer.DataAccessContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace RBACDefinition.Persistence.SQLServer.DapperDataAccess
{
    public class RoleDA : BaseDapperDA<Role>, IRoleDA
    {
        #region Properties
        protected override string ScriptSelect => $"SELECT * FROM {TableName} WHERE 1=1";
        #endregion

        #region Initialization
        public RoleDA() : base("rbac_Role")
        {
        }
        #endregion

        public async Task<IEnumerable<Role>> GetAllAsync(IDbConnection conn, string tenantUID, CancellationToken cancellationToken)
        {
            var script = $"SELECT * FROM {TableName} WHERE 1=1 and {nameof(Role.Tenant_UID)} = @tenantUID";
            var param = new { tenantUID = tenantUID };

            return await CallSelectAsync(conn, script, param, cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetAsync(IDbConnection conn, string tenantUID, IEnumerable<string> lstUIDs, CancellationToken cancellationToken)
        {
            return await PerformInChunks(lstUIDs, async chunk => await conn.QueryAsync<Role>(new CommandDefinition($"{ScriptSelect} and {nameof(Role.Tenant_UID)} = @tenantUID and { nameof(Role.UID) } in @values",
                new { values = chunk, tenantUID = tenantUID }, cancellationToken: cancellationToken)));
        }

        public async Task<Role> GetByUIDAsync(IDbConnection conn, string tenantUID, string uid, CancellationToken cancellationToken)
        {
            var script = $"SELECT * FROM {TableName} WHERE 1=1 and {nameof(Role.Tenant_UID)} = @tenantUID and {nameof(Role.UID)} = @uid";
            var param = new { tenantUID = tenantUID, uid = uid };
            
            return await CallSelectFirstAsync(conn, script, param, cancellationToken);
        }

        public async Task MergeAsync(IDbConnection conn, IEnumerable<Role> lst, CancellationToken cancellationToken)
            => await CallMergeAsync(conn, lst, cancellationToken);

        public async Task DeleteAsync(IDbConnection conn, IEnumerable<string> lstUIDs)
            => await CallDeleteAsync(conn, lstUIDs);
    }
}
