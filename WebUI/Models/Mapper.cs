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
        public static JsonFolder ToJsonFolder(this DtoFolder folder)
        {
            if (ReferenceEquals(folder, null))
                return null;

            return new JsonFolder
            {
                id = folder.ID,
                text = folder.Title
            };
        }

        public static string ToJson(this IFolderService service,long userID)
        {
            JsonFolder jsonArr = new JsonFolder { id = 0 };

            foreach (var folder in service.GetFoldersByPredicate(fldr => fldr.Level == 1 && (fldr.OwnerID == userID)))
            {
                JsonFolder jf = folder.ToJsonFolder();
                AddChlds(service, jf);
                jsonArr.item.Add(jf);
            }

            return new JavaScriptSerializer().Serialize(jsonArr);
        }

        private static void AddChlds(IFolderService service, JsonFolder folder)
        {
            var chlds = service.GetNextLevelChildNodes(service.GetById(folder.id)).ToList();

            folder.item = new List<JsonFolder>(chlds.Select(item => item.ToJsonFolder()));

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