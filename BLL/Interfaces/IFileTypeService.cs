using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IFileTypeService
    {
        /// <summary>
        /// Get all FileTypes
        /// </summary>
        /// <returns>FileTypes enumeration</returns>
        IEnumerable<DtoFileType> GetAllFileTypes();

        /// <summary>
        /// Get FileType by id
        /// </summary>
        /// <param name="key">id of the FileType</param>
        /// <returns>FileType</returns>
        DtoFileType GetFileTypeById(long key);

        /// <summary>
        /// Create new FileType
        /// </summary>
        /// <param name="e">inserting FileType</param>
        void CreateFileType(DtoFileType e);

        /// <summary>
        /// Delete FileType
        /// </summary>
        /// <param name="e">deleting FileType</param>
        void DeleteFileType(DtoFileType e);

        /// <summary>
        /// Update FileType
        /// </summary>
        /// <param name="e">updating FileType</param>
        void UpdateFileType(DtoFileType e);
    }
}
