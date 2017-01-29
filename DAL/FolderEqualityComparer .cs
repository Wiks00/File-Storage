using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM;

namespace DAL
{
    public class FolderEqualityComparer : IEqualityComparer<Folders>
    {
        public bool Equals(Folders lhs, Folders rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            else if (ReferenceEquals(lhs, null) | ReferenceEquals(rhs, null))
                return false;
            else if (lhs.id == rhs.id)
                return true;
            else
                return false;
        }

        public int GetHashCode(Folders folder)
        {
            int hCode = folder.level ^ folder.name.Length ^ folder.leftKey;
            return hCode.GetHashCode();
        }
    }
}
