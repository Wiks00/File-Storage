//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ORM
{
    using System;
    using System.Collections.Generic;
    
    public partial class Files
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Files()
        {
            this.FileTypes = new HashSet<FileTypes>();
        }
    
        public long id { get; set; }
        public System.Guid idFile { get; set; }
        public byte[] content { get; set; }
        public long folderId { get; set; }
        public System.DateTime dateTime { get; set; }
        public string name { get; set; }
    
        public virtual Folders Folder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FileTypes> FileTypes { get; set; }
    }
}
