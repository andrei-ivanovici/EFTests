using System;
using System.Collections.Generic;

namespace Ef.Main.Data
{
    public partial class Author
    {
        public Author()
        {
            AuthorBooks = new HashSet<AuthorBook>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AuthorBook> AuthorBooks { get; set; }
        public Profile Profile { get; set; }

        public int ProfileId { get; set; }
    }
}