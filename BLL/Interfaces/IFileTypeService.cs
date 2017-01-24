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
        /// Get FileTypes by delegate
        /// </summary>
        /// <param name="func">search delegate</param>
        /// <returns>FileTypes enumeration</returns>
        IEnumerable<DtoFileType> GetFileTypesByPredicate(Expression<Func<DtoFileType, bool>> func);

        /// <summary>
        /// Create new FileType
        /// </summary>
        /// <param name="e">inserting FileType</param>
        /// <returns>new FileType</returns>
        DtoFileType CreateFileType(DtoFileType e);

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
