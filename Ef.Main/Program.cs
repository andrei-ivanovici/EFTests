using System.Linq;
using Ef.Main.Data;
using Microsoft.Extensions.Logging;

namespace Ef.Main
{
    class Program
    {
        public static readonly ILoggerFactory loggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        private static ILogger _log = new Logger<Program>(loggerFactory);

        static void Main(string[] args)
        {
            // AddAuthor();
            var author = GetAuthor();
            // AddBook(author.Id);
            AddBookProblematic(author);
        }

        private static Author GetAuthor()
        {
            using var ctx = new EfTestsContext();
            var author = ctx.Authors.SingleOrDefault(auth => auth.Name == "George Orwell");
            return author;
        }


        private static void AddBook(int authorId)
        {
            using var ctx = new EfTestsContext();

            _log.LogInformation("========Adding Book===========");
            var author = ctx.Authors.Find(authorId);
            var book = new Book()
            {
                Isbn = "12345",
                Title = "1984",
            };

            ctx.Books.Add(book);
            book.AuthorBooks.Add(new AuthorBook()
            {
                Author = author,
                Book = book
            });

            ctx.SaveChanges();
        }

        private static void AddBookProblematic(Author author)
        {
            using var ctx = new EfTestsContext();

            _log.LogInformation("========Adding Book Problematic===========");

            var book = new Book()
            {
                Isbn = "12345",
                Title = "1984",
            };

            ctx.Books.Add(book);
            book.AuthorBooks.Add(new AuthorBook()
            {
                Author = author,
                Book = book
            });

            ctx.SaveChanges();
        }

        private static void AddBookAlternative()
        {
            var name = "George";
            using var ctx = new EfTestsContext();
            var author = ctx.Authors.Select(auth => new Author
            {
                Id = auth.Id
            }).SingleOrDefault(auth => auth.Name == "George Orwell");

            var book = new Book()
            {
                Isbn = "12345",
                Title = "Book 3",
            };
        }

        private static void AddAuthor()
        {
            var auth = new Author
            {
                Name = "George Orwell"
            };
            using var ctx = new EfTestsContext();
            ctx.Add(auth);
            ctx.SaveChanges();
        }
    }
}