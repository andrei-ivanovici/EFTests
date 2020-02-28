using System;
using System.Collections.Generic;

namespace Ef.Main.Data
{
    public partial class Book
    {
        public Book()
        {
            AuthorBooks = new HashSet<AuthorBook>();
        }

        public string Title { get; set; }
        public string Isbn { get; set; }
        public int Id { get; set; }

        public virtual ICollection<AuthorBook> AuthorBooks { get; set; }
    }
}
