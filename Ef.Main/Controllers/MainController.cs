using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Ef.Main.Data;
using Ef.Main.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ef.Main.Controllers
{
    [ApiController]
    [Route("main")]
    public class MainController : ControllerBase
    {
        private readonly EfTestsContext _context;

        public MainController(EfTestsContext context)
        {
            _context = context;
        }

        #region CREATE

        [HttpPost("author")]
        public AuthorModel AddAuthor(AuthorModel model)
        {
            var newAuthor = new Author()
            {
                Name = model.Name
            };
            _context.Authors.Add(newAuthor);
            _context.SaveChanges();
            model.Id = newAuthor.Id;
            return model;
        }

        [HttpGet("author")]
        public List<AuthorModel> Authors()
        {
            return _context.Authors
                .AsNoTracking()
                .Select(a => new AuthorModel()
                    {
                        Id = a.Id,
                        Name = a.Name
                    }
                ).ToList();
        }

        [HttpPost("author/profile")]
        public ActionResult<ProfileModel> AddAuthor(ProfileModel model)
        {
            var newProfile = new Profile()
            {
                Alias = model.Alias
            };
            var author = _context.Authors.Find(model.AuthorId);
            author.Profile = newProfile;
            _context.SaveChanges();
            model.Id = newProfile.Id;
            return model;
        }


        /// <summary>
        /// Adding a book by querying for the author
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost("book-query")]
        public BookModel AddBook(BookModel book)
        {
            var author = _context.Authors.Find(book.AuthorId);
            var bookEntity = new Book()
            {
                Isbn = book.Isbn,
                Title = book.Title
            };
            author.AuthorBooks.Add(new AuthorBook()
            {
                Author = author,
                Book = bookEntity
            });
            _context.Books.Add(bookEntity);
            _context.SaveChanges();
            return book;
        }

        /// <summary>
        /// Adding a book without an extra trip to the DB
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost("book-no-query")]
        public BookModel AddBookNoQuery(BookModel book)
        {
            var bookEntity = new Book()
            {
                Isbn = book.Isbn,
                Title = book.Title
            };
            _context.AuthorBooks.Add(new AuthorBook()
            {
                AuthorId = book.AuthorId,
                Book = bookEntity
            });
            _context.Books.Add(bookEntity);
            _context.SaveChanges();
            return book;
        }

        #endregion

        #region RETRIEVE

        /// <summary>
        /// Less efficient  query for books by author
        /// </summary>
        /// <param name="authId"></param>
        /// <returns></returns>
        [HttpGet("books/{authId:int}")]
        public List<BookModel> Books(int authId)
        {
            return _context.Authors
                .AsNoTracking().Where(auth => auth.Id == authId)
                .Include(auth => auth.AuthorBooks)
                .ThenInclude(map => map.Book)
                //.ToList()
                .SelectMany(auth => auth.AuthorBooks.Select(map => new BookModel()
                {
                    Isbn = map.Book.Isbn,
                    Title = map.Book.Title,
                    AuthorId = map.AuthorId
                }))
                .ToList();
        }

        /// <summary>
        /// An improvement for querying  books by author 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("books-improve/{id:int}")]
        public List<BookModel> BooksImprove(int id)
        {
            var maps = _context.AuthorBooks
                .AsNoTracking()
                .Where(b => b.AuthorId == id)
                .Include(b => b.Book)
                .Select(map => new BookModel()
                {
                    Id = map.BookId,
                    AuthorId = map.AuthorId,
                    Isbn = map.Book.Isbn,
                    Title = map.Book.Title,
                }).ToList();

            return maps;
        }

        #endregion

        #region UPDATE

        /// <summary>
        /// Updating a book. This will cause a full update with potential data loss
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        [HttpPut("books")]
        public ActionResult<BookModel> UpdateBook(BookModel changes)
        {
            var book = new Book()
            {
                Id = changes.Id,
                Title = changes.Title
            };

            _context.Update(book);
            _context.SaveChanges();
            return changes;
        }

        /// <summary>
        /// Updating a book. This will perform a partial update but will issue an extra query
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        [HttpPut("books-partial-update")]
        public ActionResult<BookModel> UpdateBookPartial(BookModel changes)
        {
            var book = _context.Books.Find(changes.Id);
            book.Title = changes.Title;
            book.Isbn = changes.Isbn;
            _context.SaveChanges();
            return changes;
        }

        /// <summary>
        /// Updating a book. This will perform a partial update but will issue an extra query
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        [HttpPut("books-custom-update")]
        public ActionResult<BookModel> BooksCustomUpdate(BookModel changes)
        {
            #region SQL Injection

            // var query = $"EXEC dbo.update_book '{changes.Id}', '{changes.Title}', '{changes.Isbn}'";
            // var book = _context.Books.FromSqlRaw(query).AsEnumerable().FirstOrDefault();

            #endregion


            var book = _context.Books.FromSqlRaw("EXEC dbo.update_book {0}, {1}, {2}",
                changes.Id, changes.Title, changes.Isbn).AsEnumerable().FirstOrDefault();

            return new BookModel
            {
                Id = book?.Id ?? -1,
                Isbn = book?.Isbn,
                Title = book?.Title,
                AuthorId = changes.AuthorId
            };
        }

        #endregion
    }
}