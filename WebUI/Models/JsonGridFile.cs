using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class JsonGridFile
    {
        public JsonGridFile()
        {
            data = new string[4];
        }
        public long id { get; set; }
        public string[] data { get; set; }
    }
}