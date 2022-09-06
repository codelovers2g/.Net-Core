using System;
using System.Collections.Generic;
using System.Text;
using RBACDefinition.Model.Common;

namespace RBACDefinition.Model.Data
{
    /// <summary>
	/// Role definition
	/// </summary>
    public class Role : IModelRecord
    {
        #region Mandatory Fields
        /// <summary>
        /// Record numeric ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Record UID (Unique Identifier)
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// Database record comments
        /// </summary>
        public string RecordComment { get; set; }

        /// <summary>
        /// Database record level security
        /// </summary>
        public string RecordLevelSecurity { get; set; }

        /// <summary>
        /// Database record creation datetime
        /// </summary>
        public DateTime? RecordCreated { get; set; }

        /// <summary>
        /// Database record modification datetime
        /// </summary>
        public DateTime? RecordModified { get; set; }

        /// <summary>
        /// Database record modified by SQL user
        /// </summary>
        public string RecordModifiedBySQLUser { get; set; }

        /// <summary>
        /// Database record modified by application user
        /// </summary>
        public string RecordModifiedByAppUser { get; set; }

        /// <summary>
        /// Database record row version
        /// </summary>
        public byte[] RecordRowVersion { get; set; }
        #endregion

        #region Table fields
        /// <summary>
        /// Tenant UID
        /// </summary>
        public string Tenant_UID { get; set; }

        /// <summary>
        /// Role code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Role name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Role description
        /// </summary>
        public string Description { get; set; }
        #endregion

        #region Static
        public static Role GetDefaultedNew()
        {
            var result = new Role();

            return result;
        }
        #endregion
    }
    public static class RoleExtension
    {
        public static string GetTitleRepresentation( this Role obj )
        {
            string res = obj.DisplayName;
            if (string.IsNullOrWhiteSpace(res))
            {
                res = obj.Code;
            }
            if (string.IsNullOrWhiteSpace(res))
            {
                res = obj.UID;
            }
            return res ?? string.Empty;
        }
    }
}
