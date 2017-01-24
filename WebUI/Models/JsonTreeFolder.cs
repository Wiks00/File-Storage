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
        }

        public long id { get; set; }
        public string text { get; set; }
        public List<JsonTreeFolder> item { get; set; }
    }
}