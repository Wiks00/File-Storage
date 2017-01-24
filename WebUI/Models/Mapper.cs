using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using BLL.DTO;
using BLL.Interfaces;
using Microsoft.Ajax.Utilities;
using Ninject.Infrastructure.Language;

namespace WebUI.Models
{
    public static class Mapper
    {
        /// <summary>
        /// Convert DtoFolder to json ready format
        /// </summary>
        /// <param name="folder">converting folder</param>
        /// <returns>Json Folder</returns>
        public static JsonTreeFolder ToJsonFolder(this DtoFolder folder)
        {
            if (ReferenceEquals(folder, null))
                return null;

            return new JsonTreeFolder
            {
                id = folder.ID,
                text = folder.Title
            };
        }

        /// <summary>
        /// Convert DtoFile to json ready format
        /// </summary>
        /// <param name="file">converting file</param>
        /// <param name="iconPath">path to image</param>
        /// <returns>Json File</returns>
        public static JsonGridFile ToGridObject(this DtoFile file,string iconPath = "../icons/grid_file1.png")
        {
            if (ReferenceEquals(file, null))
                return null;

            return new JsonGridFile
            {
                id = file.ID,
                data = new []{ iconPath, file.Title, file.FileTypes.First().TypeName, file.DateTime.ToString("D") }
            };
        }

        /// <summary>
        /// Create Json for tree view
        /// </summary>
        /// <param name="service">folder service</param>
        /// <param name="userID">user id</param>
        /// <returns>Json string</returns>
        public static string ToTreeJson(this IFolderService service,long userID)
        {
            JsonTreeFolder jsonArr = new JsonTreeFolder { id = 0 };

            foreach (var folder in service.GetFoldersByPredicate(fldr => fldr.Level == 1 && (fldr.OwnerID == userID)))
            {
                JsonTreeFolder jf = folder.ToJsonFolder();
                AddChlds(service, jf);
                jsonArr.item.Add(jf);
            }

            return new JavaScriptSerializer().Serialize(jsonArr);
        }

        /// <summary>
        /// Create Json for Grid view
        /// </summary>
        /// <param name="folder">folder where we will take files</param>
        /// <returns>Json string</returns>
        public static string ToGridJson(this DtoFolder folder)
        {
            var jsonObj = new { rows = new List<JsonGridFile>() };

            foreach (var file in folder.Files)
            {
                jsonObj.rows.Add(file.ToGridObject());
            }

            return new JavaScriptSerializer().Serialize(jsonObj);
        }

        private static void AddChlds(IFolderService service, JsonTreeFolder folder)
        {
            var chlds = service.GetNextLevelChildNodes(service.GetById(folder.id)).ToList();

            folder.item = new List<JsonTreeFolder>(chlds.Select(item => item.ToJsonFolder()));

            foreach (var chld in folder.item)
            {
                var posibleChlds =
                    service.GetNextLevelChildNodes(service.GetById(chld.id)).Select(item => item.ToJsonFolder());

                if (posibleChlds.Any())
                {
                    AddChlds(service, chld);
                }
            }
        }
    }
}