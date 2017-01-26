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
        /// Convert DtoFolder to json ready format
        /// </summary>
        /// <param name="folder">converting folder</param>
        /// <param name="type">type of fodler</param>
        /// <returns>Json File</returns>
        public static JsonGridFile ToGridObject(this DtoFolder folder, string type)
        {
            if (ReferenceEquals(folder, null))
                return null;

            return new JsonGridFile
            {
                id = folder.ID,
                data = new[] { SetIconPath(type), folder.Title, "-", folder.DateTime.ToString("D") }
            };
        }

        /// <summary>
        /// Convert DtoFile to json ready format
        /// </summary>
        /// <param name="file">converting file</param>
        /// <param name="type">file type</param>
        /// <returns>Json File</returns>
        public static JsonGridFile ToGridObject(this DtoFile file,string type)
        {
            if (ReferenceEquals(file, null))
                return null;

            return new JsonGridFile
            {
                id = file.ID,
                data = new []{ SetIconPath(type), file.Title, file.FileTypes.First().TypeName, file.DateTime.ToString("D") }
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
        public static string ToGridJson(this IFolderService service, DtoFolder folder)
        {
            var jsonObj = new { rows = new List<JsonGridFile>() };
            DtoFolder[] folders = service.GetNextLevelChildNodes(folder).ToArray();

            foreach (var chld in folders)
            {
                if(service.GetNextLevelChildNodes(chld).Any() || chld.Files.Any())
                    jsonObj.rows.Add(chld.ToGridObject("folder"));
                else
                    jsonObj.rows.Add(chld.ToGridObject("emptyfolder"));
            }

            foreach (var file in folder.Files)
            {
                jsonObj.rows.Add(file.ToGridObject(file.FileTypes.First().TypeName));
            }

            return new JavaScriptSerializer().Serialize(jsonObj);
        }

        private static void AddChlds(IFolderService service, JsonTreeFolder folder)
        {
            var chlds = service.GetNextLevelChildNodes(service.GetById(folder.id)).ToList();

            folder.item = new List<JsonTreeFolder>(chlds.Select(item => item.ToJsonFolder()));

            if (folder.item.Any())
            {
                foreach (var chld in folder.item)
                {
                    var posibleChlds =
                        service.GetNextLevelChildNodes(service.GetById(chld.id)).Select(item => item.ToJsonFolder());

                    if (posibleChlds.Any())
                        AddChlds(service, chld);
                }
            }
        }

        private static string SetIconPath(string type)
        {
            if(type.Contains("text"))
                return "../icons/document-text.png";

            if(type.Contains("video") || type.Contains("audio"))
                return "../icons/document-play.png";

            if(type.Contains("image"))
                return "../icons/document-image.png";

            if (type.Contains("image"))
                return "../icons/document-image.png";

            if (type.Contains("compressed"))
                return "../icons/document-zip.png";

            if (type.Equals("emptyfolder"))
                return "../icons/folder-empty.png";

            if (type.Equals("folder"))
                return "../icons/folders.png";

            return "../icons/document.png";
        }
    }
}