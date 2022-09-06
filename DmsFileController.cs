using api.REST.Base;
using api.REST.Base.Controllers;
using DMS.Model.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Web;
using Utility.Converters;
using System.Net.Mime;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Utility.WEB.Http;
using Newtonsoft.Json;
using TenantDefinition.Protocol;
using DMS.API.Model.Data;
using DMS.Model.Service;
using DMS.Business.Service;
using DMS.Configuration.Model;
using api.Routing.Attributes;

namespace DMS.API.REST.Controllers
{
    /// <summary>
    /// Etl contraller
    /// </summary>
    [ApiConventionType( typeof( APIConventions ) )]
    [ApiController]
    [Area(DMSConstants.ApiPrefix)]
    public class DmsFileController : APIControllerBase
    {
        #region Constants        
        private const int dms_download_file_size_limit = 2000; //2000Mb
        private const int dms_download_file_count_limit = 100;
        public const int dms_email_file_count_limit = 30;
        public const long dms_email_file_size_limit = 50;
        #endregion

        #region Properties
        IDmsService Service { get; }
        #endregion

        #region Initialization
        public DmsFileController( DmsService service)
        {
            Service = service;
        }
        #endregion

        #region Methods DmsFile/DmsFileComplex

        /// <summary>
        /// Get all files
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of DmsFileComplex</returns>
        [PrefixedHttpGet(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.GetDmsFileComplexAllAsync))]
        public async Task<ActionResult<IEnumerable<DmsFileComplex>>> GetDmsFileComplexAllAsync(CancellationToken cancellationToken)
            => await DefaultGet(async () => await Service.GetDmsFileComplexAllAsync(cancellationToken));

        /// <summary>
        /// Get file by UID
        /// </summary>
        /// <param name="uid">File UID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>DmsFileComplex</returns>
        [PrefixedHttpGet(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.GetDmsFileComplexByUIDAsync), withParamUID:true)]
        public async Task<ActionResult<DmsFileComplex>> GetDmsFileComplexByUIDAsync(string uid, CancellationToken cancellationToken)
            => await DefaultGet(async () => await Service.GetDmsFileComplexByUIDAsync(uid, cancellationToken));


        /// <summary>
        /// Get files by UIDs
        /// </summary>
        /// <param name="lstUIDs">Collection of file UIDs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of DmsFileComplex</returns>
        [PrefixedHttpGet(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.GetDmsFileComplexAsync))]
        public async Task<ActionResult<IEnumerable<DmsFileComplex>>> GetDmsFileComplexAsync(IEnumerable<string> lstUIDs, CancellationToken cancellationToken)
            => await DefaultGet(async () => await Service.GetDmsFileComplexAsync(lstUIDs, cancellationToken));


        /// <summary>
        /// Get files by objects files are attached
        /// </summary>
        /// <param name="lstUIDs">Collection of object UIDs files are attached to</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of DmsFileComplex</returns>
        [PrefixedHttpGet(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.GetDmsFileComplexByAttachedToAsync))]
        public async Task<ActionResult<IEnumerable<DmsFileComplex>>> GetDmsFileComplexByAttachedToAsync(IEnumerable<string> lstUIDs, CancellationToken cancellationToken = default)
           => await DefaultGet(async () => await Service.GetDmsFileComplexByAttachedToAsync(lstUIDs, cancellationToken));


        /// <summary>
        /// Get files count by objects files are attached
        /// </summary>
        /// <param name="lstUIDs">Collection of object UIDs files are attached to</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        [PrefixedHttpGet(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.GetDmsFileCountByAttachedToAsync))]
        public async Task<ActionResult<Dictionary<string, int>>> GetDmsFileCountByAttachedToAsync(IEnumerable<string> lstUIDs, CancellationToken cancellationToken = default)
           => await DefaultGet(async () => await Service.GetDmsFileCountByAttachedToAsync(lstUIDs, cancellationToken));


        /// <summary>
        /// Merge (insert new or update existing) Files
        /// </summary>
        /// <param name="lstDmsFile">Collection of DmsFile</param>
        /// <returns></returns>
        [PrefixedHttpPost(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.MergeDmsFileAsync))]
        public async Task<IActionResult> PostDmsFile(DmsFile[] lstDmsFile)
            => await DefaultPost(async () => await Service.MergeDmsFileAsync(lstDmsFile));


        /// <summary>
        /// Update existing FileComplexes (Allowed to edit columns only)
        /// </summary>
        /// <param name="lstDmsFileComplex">Collection of DmsFileComplex</param>
        /// <returns></returns>
        [PrefixedHttpPost(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.MergeAllowedToEditFieldsFileComplexAsync))]
        public async Task<IActionResult> MergeAllowedToEditFieldsFileComplexAsync(DmsFileComplex[] lstDmsFileComplex)
            => await DefaultPost(async () => await Service.MergeAllowedToEditFieldsFileComplexAsync(lstDmsFileComplex));


        /// <summary>
        /// Delete files by UIDs
        /// </summary>
        /// <param name="lstUIDs">Collection of file UIDs</param>
        /// <returns></returns>
        [PrefixedHttpDelete(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.DeleteDmsFileWithReferencesAsync))]
        public async Task<IActionResult> DeleteDmsFileWithReferencesAsync(IEnumerable<string> lstUIDs)
            => await DefaultDelete(async () => await Service.DeleteDmsFileWithReferencesAsync(lstUIDs));


        /// <summary>
        /// Delete files by objects files are attached to
        /// </summary>
        /// <param name="lstUIDs">Collection of object UIDs files are attached to</param>
        /// <returns></returns>
        [PrefixedHttpDelete(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.DeleteDmsFileWithReferencesByAttachedToAsync))]
        public async Task<IActionResult> DeleteDmsFileWithReferencesByAttachedToAsync(IEnumerable<string> lstUIDs)
            => await DefaultDelete(async () => await Service.DeleteDmsFileWithReferencesByAttachedToAsync(lstUIDs));

          

        #endregion

        #region DmsFileVersion

        /// <summary>
        /// Get file version by UID
        /// </summary>
        /// <param name="uid">File version UID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>DmsFileVersion</returns>
        [PrefixedHttpGet(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.GetDmsFileVersionByUIDAsync))]
        public async Task<ActionResult<DmsFileVersion>> GetDmsFileVersionByUIDAsync(string uid, CancellationToken cancellationToken)
            => await DefaultGet(async () => await Service.GetDmsFileVersionByUIDAsync(uid, cancellationToken));


        /// <summary>
        /// Get file versions by UIDs
        /// </summary>
        /// <param name="lstUIDs">Collection of file version UIDs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of DmsFileVersion</returns>
        [PrefixedHttpGet(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.GetDmsFileVersionAsync))]
        public async Task<ActionResult<IEnumerable<DmsFileVersion>>> GetDmsFileVersionAsync(IEnumerable<string> lstUIDs, CancellationToken cancellationToken)
            => await DefaultGet(async () => await Service.GetDmsFileVersionAsync(lstUIDs, cancellationToken));


        /// <summary>
        /// Get file versions by file UIDs
        /// </summary>
        /// <param name="lstUIDs">Collection of file UIDs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>DmsFileVersion</returns>
        [PrefixedHttpGet(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.GetDmsFileVersionByFileAsync))]
        public async Task<ActionResult<IEnumerable<DmsFileVersion>>> GetDmsFileVersionByFileAsync(IEnumerable<string> lstUIDs, CancellationToken cancellationToken)
            => await DefaultGet(async () => await Service.GetDmsFileVersionByFileAsync(lstUIDs, cancellationToken));


        /// <summary>
        /// Merge (insert new or update existing) file versions
        /// </summary>
        /// <param name="lstDmsFileVersion">Collection of DmsFileVersion</param>
        /// <returns></returns>
        [PrefixedHttpPost(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.MergeAllowedToEditFieldsFileVersionAsync))]
        public async Task<IActionResult> MergeAllowedToEditFieldsFileVersionAsync(DmsFileVersion[] lstDmsFileVersion)
            => await DefaultPost(async () => await Service.MergeAllowedToEditFieldsFileVersionAsync(lstDmsFileVersion));


        /// <summary>
        /// Delete file versions by UIDs
        /// </summary>
        /// <param name="lstUIDs">Collection of file version UIDs</param>
        /// <returns></returns>
        [PrefixedHttpDelete(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.DeleteDmsFileVersionAsync))]
        public async Task<IActionResult> DeleteDmsFileVersionAsync(IEnumerable<string> lstUIDs)
            => await DefaultDelete(async () => await Service.DeleteDmsFileVersionAsync(lstUIDs));

        /// <summary>
        /// Delete file versions by file UIDs
        /// </summary>
        /// <param name="lstUIDs">Collection of file UIDs</param>
        /// <returns></returns>
        [PrefixedHttpDelete(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.DeleteDmsFileVersionByFileAsync))]
        public async Task<IActionResult> DeleteDmsFileVersionByFileAsync(IEnumerable<string> lstUIDs)
            => await DefaultDelete(async () => await Service.DeleteDmsFileVersionByFileAsync(lstUIDs));

        #endregion

        #region DmsFileStream


        /// <summary>
        /// Download file data by file version UID
        /// </summary>
        /// <param name="fileVersionUID">File version UID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream of data</returns>
        [PrefixedHttpGet(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.DownloadFileDataByFileVersionAsync))]
        public async Task<ActionResult<Stream>> DownloadFileDataByFileVersionAsync(string fileVersionUID, CancellationToken cancellationToken = default)
        {
            var res = await Service.DownloadFileDataByFileVersionAsync(fileVersionUID, cancellationToken);
            return res;
        }

        /// <summary>
        /// Download files data (zip) by file version UIDs
        /// </summary>
        /// <param name="fileVersionUIDs">File version UIDs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream of files data archive</returns>
        [PrefixedHttpGet(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.DownloadFilesDataByFileVersionsAsync))]
        public async Task<IActionResult> DownloadFilesDataByFileVersionsAsync( IEnumerable<string> fileVersionUIDs, CancellationToken cancellationToken = default )
		{
            await ValidateDownloadFilesDataByFileVersionsAsync( fileVersionUIDs, cancellationToken);
            if( !ModelState.IsValid )
			{
                return ValidationProblem();
            }

            Response.ContentType = MediaTypeNames.Application.Octet;
            Response.Headers.Add( "Content-Disposition", "attachment; filename=\"documents.zip\"" );            
            await Service.DownloadFilesDataByFileVersionsAsync( Response.Body, fileVersionUIDs, cancellationToken );
            return new EmptyResult();

            async Task ValidateDownloadFilesDataByFileVersionsAsync( IEnumerable<string> fileVersionUIDs, CancellationToken cancellationToken )
            {
                var lstErrors = new List<(string Key, string Message)>();

                void setError( string msg )
                {
                    lstErrors.Add( (Key: string.Empty, Message: msg) );
                }

                if ( fileVersionUIDs.Count() > dms_download_file_count_limit)
                {
                    setError( $"Maximum number of files to download exceeded. File count limit: {dms_download_file_count_limit}" );
                }

                if ( !lstErrors.Any() )
                {
                    var files = await Service.GetDmsFileVersionAsync( fileVersionUIDs, cancellationToken );
                    if ( files.Sum( x => x.FileSize ?? 0 ) > dms_download_file_size_limit * 1024 * 1024)
                    {
                        setError( $"Maximum size of files to download exceeded. Summarized file size limit (MB): {dms_download_file_size_limit}" );
                    }
                }

                if ( lstErrors.Any() )
                {
                    foreach ( var (Key, Message) in lstErrors )
                    {
                        ModelState.AddModelError( Key, Message );
                    }
                }
            }
        }


        /// <summary>
        /// Upload new file data
        /// </summary>
        /// <returns></returns>
        [PrefixedHttpPost(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.UploadFileDataAsync))]
        public async Task<IActionResult> UploadFileDataAsync()
        {
            return await DefaultPost(
               async () =>
             {
                 DmsUploadFileMeta fileMeta = null;
                 Stream stream = null;

                 if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                 {
                     throw new Exception("UploadNewFileDataAsync wrong content type");
                 }
                                  
                 var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType));
                 var reader = new MultipartReader(boundary, HttpContext.Request.Body);

                 var section = await reader.ReadNextSectionAsync();
                 
                 while (section != null)
                 {
                     if (section.Headers.ContainsKey(FileUploadSectionMark.FileStreamMetaDataMark))
                     {
                         using (StreamReader streamReader = new StreamReader(section.Body))
                         {
                             var strJson = await streamReader.ReadToEndAsync();
                             fileMeta = JsonConvert.DeserializeObject<DmsUploadFileMeta>(strJson);
                         }
                     }
                     else if (section.Headers.ContainsKey(FileUploadSectionMark.FileStreamMark))
                     {
                         stream = section.Body;
                         
                         if (stream == null || fileMeta == null)
                         {
                             throw new Exception("UploadNewFileDataAsync: File stream or file meta data is not received");
                         }
                         await Service.UploadFileDataAsync(stream, fileMeta);
                         break;
                     }
                     section = await reader.ReadNextSectionAsync();
                 }
             } );
        }


        /// <summary>
        /// Upload new file data
        /// </summary>
        /// <returns></returns>
        [PrefixedHttpPost(DMSConstants.ApiPrefix, nameof(IDmsService), nameof(IDmsService.SendFilesByEmailAsync))]
        public async Task<IActionResult> SendFilesByEmailAsync1(SendFilesByEmailParameters sendFilesByEmailParameters, CancellationToken cancellationToken = default)
        {
            var lstErrors = new List<(string Key, string Message)>();

            void setError(string msg)
            {
                lstErrors.Add((Key: string.Empty, Message: msg));
            }

            if (sendFilesByEmailParameters.UIDs.Count() > dms_email_file_count_limit)
            {
                setError($"Maximum number of files to Email exceeded. File count limit: {dms_email_file_count_limit}");
            }

            if (!lstErrors.Any())
            {
                var files = await Service.GetDmsFileVersionAsync(sendFilesByEmailParameters.UIDs, cancellationToken);
                if (files.Sum(x => x.FileSize ?? 0) > dms_email_file_size_limit * 1024 * 1024)
                {
                    setError($"Maximum size of files to Email exceeded. Summarized file size limit (MB): {dms_email_file_size_limit}");
                }
            }

            if (lstErrors.Any())
            {
                foreach (var (Key, Message) in lstErrors)
                {
                    ModelState.AddModelError(Key, Message);
                }
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            await Service.SendFilesByEmailAsync(sendFilesByEmailParameters, cancellationToken);

            return Ok();            
        }
        #endregion
    }


    
}
