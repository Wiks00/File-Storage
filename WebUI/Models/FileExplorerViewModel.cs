using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class FileExplorerViewModel
    {
        public string UserName { get; set; }
        public string TreeStructJson { get; set; }
        public string SharedTreeStructJson { get; set; }
    }
}