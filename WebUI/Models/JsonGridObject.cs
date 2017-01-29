using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class JsonGridObject
    {
        public JsonGridObject()
        {
            data = new string[5];
        }
        public long id { get; set; }
        public string[] data { get; set; }
    }
}