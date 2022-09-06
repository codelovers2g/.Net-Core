using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using PlatformRuntime.Model.ExternalProvider;
using TenantDefinition.Model.Service;
using TenantDefinition.Model.Data;

namespace PlatformRuntime.Model
{
    public class TenantContextProcessor
    {
        #region ExternalProviders

        private IAuthenticatedUserProvider AuthenticatedUserProvider { get; set; }
        private IContextAttributesProvider ContextAttributesProvider { get; set; }
        private IUserPermissionsProvider UserPermissionsProvider { get; set; }
        private ITenancyResolverService TenancyResolver { get; set; }
        private IApplicationInfoProvider ApplicationInfoProvider { get; set; }

        #endregion

        public TenantContextProcessor(IAuthenticatedUserProvider authenticatedUserProvider, IContextAttributesProvider contextAttributesProvider,
            IUserPermissionsProvider userPermissionsProvider, 
            ITenancyResolverService tenancyResolver, IApplicationInfoProvider applicationInfoProvider)
        {
            
            AuthenticatedUserProvider = authenticatedUserProvider;
            ContextAttributesProvider = contextAttributesProvider;
            UserPermissionsProvider = userPermissionsProvider;
            TenancyResolver = tenancyResolver;
            ApplicationInfoProvider = applicationInfoProvider;
        }

        private async Task<ContextExtended> PrepareContextExtendedAsync(CancellationToken cancellationToken = default)
        {
            ContextExtended res = new ContextExtended();
            res.Requested = await ContextAttributesProvider.GetAsync();
            //Application does not require received context
            res.Application = await ApplicationInfoProvider.GetApplicationInfoAsync(cancellationToken);

            res.Processed = new ContextAttributes();

            //Obtaining UserUID 
            res.Processed.User_UID = AuthenticatedUserProvider.GetUserUID();
            res.Processed.Host = res.Requested.Host; //Is applied without any checks

            if (!string.IsNullOrWhiteSpace(res.Processed.User_UID))
            {
                Func<AccountUser, string, Task> tryAccountAsync = async (accountUser, accountUID) =>
                {
                    accountUser = accountUser == null && !string.IsNullOrWhiteSpace(accountUID)
                                     ? (await TenancyResolver.GetAccountUserAsync(accountUID, res.Processed.User_UID, cancellationToken)) : accountUser;

                    res.Account = !string.IsNullOrWhiteSpace(accountUser?.Account_UID)
                                    ? (await TenancyResolver.GetAccountByUIDAsync(accountUser?.Account_UID, cancellationToken)) : null;
                    
                    if(!string.IsNullOrWhiteSpace(res.Account?.UID))
                    {
                        res.AccountUser = accountUser;
                        res.Processed.Account_UID = res.Account?.UID;
                    }
                };

                Func<TenantUser, string, Task> tryTenantAsync = async (tenantUser, tenantUID) =>
                {
                    tenantUser = tenantUser == null && !string.IsNullOrWhiteSpace(tenantUID)
                                     ? (await TenancyResolver.GetTenantUserAsync(tenantUID, res.Processed.User_UID, cancellationToken)) : tenantUser;

                    res.Tenant = !string.IsNullOrWhiteSpace(tenantUser?.Tenant_UID)
                                    ? (await TenancyResolver.GetTenantByUIDAsync(tenantUser?.Tenant_UID, cancellationToken)) : null;

                    if(!string.IsNullOrWhiteSpace(res.Tenant?.UID))
                    {
                        res.TenantUser = tenantUser;
                        res.Processed.Tenant_UID = res.Tenant?.UID;
                    }
                    await tryAccountAsync(null, res.Tenant.Account_UID);
                };

                if (!string.IsNullOrWhiteSpace(res.Requested.Tenant_UID))
                {
                    await tryTenantAsync(null, res.Requested.Tenant_UID);
                }

                if (res.Tenant == null)
                {
                    IEnumerable<TenantUser> allTenantUsers = await TenancyResolver.GetTenantUsersByUserAsync(res.Processed.User_UID, cancellationToken);
                    foreach (var tenantUser in allTenantUsers)
                    {
                        await tryTenantAsync(tenantUser, null);
                        if (res.Tenant != null)
                        {
                            break;
                        }
                    }
                }

                if (res.Tenant == null)
                {
                    if (!string.IsNullOrWhiteSpace(res.Requested.Account_UID))
                    {
                        await tryAccountAsync(null, res.Requested.Account_UID);
                    }
                    if (res.Account == null)
                    {
                        IEnumerable<AccountUser> allAccountUsers = await TenancyResolver.GetAccountUsersByUserAsync(res.Processed.User_UID, cancellationToken);
                        foreach (var accountUser in allAccountUsers)
                        {
                            await tryAccountAsync(accountUser, null);
                            if (res.Account != null)
                            {
                                break;
                            }
                        }
                    }
                }

                res.DicPermissions = await UserPermissionsProvider.GetUserPermissionGroupsByUserAsync(res.Tenant?.UID, res.Processed.User_UID, cancellationToken);
            }

            return res;
        }

        private ContextExtended ContextExtended { get; set; }
        public async Task<ContextExtended> GetContextExtendedAsync()
        {
            if (ContextExtended == null)
            {
                ContextExtended = await PrepareContextExtendedAsync();
            }
            return ContextExtended;
        }
    }
}
