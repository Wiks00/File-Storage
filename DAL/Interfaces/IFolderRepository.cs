using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO;

namespace DAL.Interfaces
{
    public interface IFolderRepository : IRepository<DalFolder>
    {
        /// <summary>
        /// Create root folder for new User (only one per user)
        /// </summary>
        /// <param name="userID">For what user create root folder</param>
        /// <exception cref="ArgumentException"></exception>
        void CreateRoot(long userID);

        /// <summary>
        /// Move folder into another path
        /// </summary>
        /// <param name="movingFolder">folder that would be moved</param>
        /// <param name="toFolder">in what folder should be moved</param>
        /// <exception cref="ArgumentNullException"></exception>
        void Move(DalFolder movingFolder, DalFolder toFolder);

        /// <summary>
        /// Add new folder
        /// </summary>
        /// <param name="prnt">in what folder should be created</param>
        /// <param name="newFolderName">new folder title</param>
        /// <returns>new folder</returns>
        /// <exception cref="ArgumentNullException"></exception>
        DalFolder Add(DalFolder prnt, string newFolderName);

        /// <summary>
        /// Insert new files in the folder
        /// </summary>
        /// <param name="fldr">folder , where will be inserted files</param>
        /// <param name="fls">inserted file(s)</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void InsertFiles(DalFolder fldr, params DalFile[] fls);

        /// <summary>
        /// Move files into another folder
        /// </summary>
        /// <param name="newFldr">folder , where will be moveded files</param>
        /// <param name="fls">moveded file(s)</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void MoveFiles(DalFolder newFldr, params DalFile[] fls);

        /// <summary>
        /// Share folder to another Users
        /// </summary>
        /// <param name="fldr">folder , that will be shared</param>
        /// <param name="usrs">users which will be granted access</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void ShareFolderTo(DalFolder fldr, params DalUser[] usrs);

        /// <summary>
        /// Remove access to folder to selected users
        /// </summary>
        /// <param name="fldr">for what folder will be removed access</param>
        /// <param name="usrs">for which users</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void RemoveAccessToFolder(DalFolder fldr, params DalUser[] usrs);

        /// <summary>
        /// Get next level child folders of given folder
        /// </summary>
        /// <param name="fldr">investigated folder</param>
        /// <returns>child folder enumeration</returns>
        /// <exception cref="ArgumentNullException"></exception>
        IEnumerable<DalFolder> GetNextLevelChildNodes(DalFolder fldr);

        /// <summary>
        /// Get all child folders of given folder
        /// </summary>
        /// <param name="fldr">investigated folder</param>
        /// <returns>child folder enumeration</returns>
        /// <exception cref="ArgumentNullException"></exception>
        IEnumerable<DalFolder> GetChildNodes(DalFolder fldr);

        /// <summary>
        /// Get parent folder of given folder
        /// </summary>
        /// <param name="fldr">investigated folder</param>
        /// <returns>parent folder</returns>
        /// <exception cref="ArgumentNullException"></exception>
        DalFolder GetPreviousLevelParentNode(DalFolder fldr);

        /// <summary>
        /// Get neighboring folders of given folder
        /// </summary>
        /// <param name="fldr">investigated folder</param>
        /// <returns>neighboring folder enumeration</returns>
        /// <exception cref="ArgumentNullException"></exception>
        IEnumerable<DalFolder> GetNeighboringNodes(DalFolder fldr);
    }
}
