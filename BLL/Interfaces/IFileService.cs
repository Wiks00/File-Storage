﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Get all Files from database
        /// </summary>
        /// <returns>Files enumeration</returns>
        IEnumerable<DtoFile> GetAllFiles();

        /// <summary>
        /// Get File by id
        /// </summary>
        /// <param name="key">id of the File</param>
        /// <returns>Files</returns>
        DtoFile GetFileById(long key);

        /// <summary>
        /// Get Fiels by delegate
        /// </summary>
        /// <param name="func">search delegate</param>
        /// <returns>Fiels enumeration</returns>
        IEnumerable<DtoFile> GetFielsByPredicate(Expression<Func<DtoFile, bool>> func);

        /// <summary>
        /// Create new Files
        /// </summary>
        /// <param name="e">inserting File</param>
        /// <returns>new File</returns>
        DtoFile CreateFile(DtoFile e);

        /// <summary>
        /// Delete File
        /// </summary>
        /// <param name="e">deleting File</param>
        void DeleteFile(DtoFile e);

        /// <summary>
        /// Update File
        /// </summary>
        /// <param name="e">updating File</param>
        void UpdateFile(DtoFile e);


        /// <summary>
        /// Update File titile
        /// </summary>
        /// <param name="newTitle">new File title</param>
        /// <param name="id">id of updating File</param>
        void UpdateFileTitle(string newTitle,long id);
    }
}
