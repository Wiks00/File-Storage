using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO;
using ORM;

namespace DAL.Interfaces
{
    public interface IFolderRepository : IRepository<Folders>
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
        void Move(Folders movingFolder, Folders toFolder);

        /// <summary>
        /// Add new folder
        /// </summary>
        /// <param name="parent">in what folder should be created</param>
        /// <param name="newFolderName">new folder title</param>
        void Add(Folders parent, string newFolderName);

        /// <summary>
        /// Insert new files in the folder
        /// </summary>
        /// <param name="folder">folder , where will be inserted files</param>
        /// <param name="files">inserted file(s)</param>
        void InsertFiles(Folders folder, params Files[] files);

        /// <summary>
        /// Move files into another folder
        /// </summary>
        /// <param name="newFolder">folder , where will be moveded files</param>
        /// <param name="files">moveded file(s)</param>
        void MoveFiles(Folders newFolder, params Files[] files);

        /// <summary>
        /// Get child folders of given folder
        /// </summary>
        /// <param name="folder">investigated folder</param>
        /// <returns>child folder enumeration</returns>
        IEnumerable<Folders> GetNextLevelChildNodes(Folders folder);

        /// <summary>
        /// Get parent folder of given folder
        /// </summary>
        /// <param name="folder">investigated folder</param>
        /// <returns>parent folder</returns>
        Folders GetPreviousLevelParentNode(Folders folder);

        /// <summary>
        /// Get neighboring folders of given folder
        /// </summary>
        /// <param name="folder">investigated folder</param>
        /// <returns>neighboring folder enumeration</returns>
        IEnumerable<Folders> GetNeighboringNodes(Folders folder);
    }
}
