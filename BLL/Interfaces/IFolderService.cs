using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IFolderService
    {
        /// <summary>
        /// Get all Folders
        /// </summary>
        /// <returns>Folders enumeration</returns>
        IEnumerable<DtoFolder> GetAllFolders();

        /// <summary>
        /// Get Folder by id
        /// </summary>
        /// <param name="key">id of the searching Folder</param>
        /// <returns>Folder</returns>
        DtoFolder GetById(long key);

        /// <summary>
        /// Get Folders that contains titile
        /// </summary>
        /// <param name="title">title of the search Folders</param>
        /// <returns>Folders enumeration</returns>
        IEnumerable<DtoFolder> GetFoldersContainsTitle(string title);

        /// <summary>
        /// Get Folders by delegate
        /// </summary>
        /// <param name="func">search delegate</param>
        /// <returns>Folders enumeration</returns>
        IEnumerable<DtoFolder> GetFoldersByPredicate(Expression<Func<DtoFolder, bool>> func);

        /// <summary>
        /// Create new root Folder for the User
        /// </summary>
        /// <param name="userID">ID of the User</param>
        void CreateRootFolder(long userID);

        /// <summary>
        /// Delete Folder
        /// </summary>
        /// <param name="e">deleting Folder</param>
        void DeleteFolder(DtoFolder e);

        /// <summary>
        /// Update Folder
        /// </summary>
        /// <param name="e">updating Folder</param>
        void UpdateFolder(DtoFolder e);

        /// <summary>
        /// Update Folder titile
        /// </summary>
        /// <param name="newTitle">new Folder title</param>
        /// <param name="id">id of updating Folder</param>
        void UpdateFolderTitle(string newTitle, long id);

        /// <summary>
        /// Move Folder into another path
        /// </summary>
        /// <param name="movingFolder">Folder that would be moved</param>
        /// <param name="toFolder">in what Folder should be moved</param>
        void MoveFolder(DtoFolder movingFolder, DtoFolder toFolder);

        /// <summary>
        /// Add new Folder
        /// </summary>
        /// <param name="parent">in what Folder should be created</param>
        /// <param name="newFolderName">new Folder title</param>
        /// <returns>new Folder</returns>
        DtoFolder AddFolder(DtoFolder parent, string newFolderName);

        /// <summary>
        /// Insert new files in the Folder
        /// </summary>
        /// <param name="folder">Folder , where will be inserted file</param>
        /// <param name="files">inserted file(s)</param>
        void InsertFilesIntoFolder(DtoFolder folder, params DtoFile[] files);


        /// <summary>
        /// Move files into another folder
        /// </summary>
        /// <param name="folder">folder , where will be moveded files</param>
        /// <param name="files">moveded file(s)</param>
        void MoveFilesIntoAnotherFolder(DtoFolder folder, params DtoFile[] files);

        /// <summary>
        /// Share folder to another Users
        /// </summary>
        /// <param name="folder">folder , that will be shared</param>
        /// <param name="users">users which will be granted access</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void ShareFolderToUsers(DtoFolder folder, params DtoUser[] users);

        /// <summary>
        /// Remove access to folder to selected users
        /// </summary>
        /// <param name="folder">for what folder will be removed access</param>
        /// <param name="users">for which users</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void RemoveAccessToFolderToUsers(DtoFolder folder, params DtoUser[] users);

        /// <summary>
        /// Get nex level child Folders of given Folder
        /// </summary>
        /// <param name="folder">investigated Folder</param>
        /// <returns>child Folders enumeration</returns>
        IEnumerable<DtoFolder> GetNextLevelChildNodes(DtoFolder folder);

        /// <summary>
        /// Get all child Folders of given Folder
        /// </summary>
        /// <param name="folder">investigated Folder</param>
        /// <returns>child Folders enumeration</returns>
        IEnumerable<DtoFolder> GetChildNodes(DtoFolder folder);

        /// <summary>
        /// Get parent Folder of given folder
        /// </summary>
        /// <param name="folder">investigated Folder</param>
        /// <returns>parent Folder</returns>
        DtoFolder GetPreviousLevelParentNode(DtoFolder folder);

        /// <summary>
        /// Get neighboring Folders of given Folder
        /// </summary>
        /// <param name="folder">investigated Folder</param>
        /// <returns>neighboring Folders enumeration</returns>
        IEnumerable<DtoFolder> GetNeighboringNodes(DtoFolder folder);
    }
}
