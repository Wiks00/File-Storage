using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class JsonFolder
    {
        public JsonFolder()
        {
            item = new List<JsonFolder>();
        }

        public long id { get; set; }
        public string text { get; set; }
        public List<JsonFolder> item { get; set; }
    }
}