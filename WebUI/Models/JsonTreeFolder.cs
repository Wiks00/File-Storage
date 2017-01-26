using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class JsonTreeFolder
    {
        public JsonTreeFolder()
        {
            item = new List<JsonTreeFolder>();
            im0 = "folderClosed.gif";
        }

        public long id { get; set; }
        public string text { get; set; }
        public string im0 { get; set; }
        public List<JsonTreeFolder> item { get; set; }
    }
}