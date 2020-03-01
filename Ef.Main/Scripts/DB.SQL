create schema dbo
go

create table Authors
(
	Id int identity
		constraint Author_pk
			primary key nonclustered,
	Name varchar(100),
	ProfileId int
)
go

create table Books
(
	Title varchar(500),
	ISBN varchar(50),
	Id int identity
		constraint Books_pk
			primary key nonclustered
)
go

create table AuthorBooks
(
	AuthorId int not null
		constraint AuthorBooks_Author_Id_fk
			references Authors,
	BookId int not null
		constraint AuthorBooks_Books_Id_fk
			references Books,
	constraint AuthorBooks_pk
		primary key nonclustered (AuthorId, BookId)
)
go

create table History
(
	Id int identity
		constraint History_pk
			primary key nonclustered,
	EventTime datetime2,
	Event varchar(500),
	Owner varchar(100)
)
go

create table Profiles
(
	Id int identity
		constraint Profiles_pk
			primary key nonclustered,
	Alias varchar(100),
	Country varchar(100)
)
go

CREATE   PROCEDURE dbo.update_book @bookId int, @title varchar(500), @isbn varchar(100)
AS
BEGIN
    UPDATE dbo.Books
    SET Title = ISNULL(@title, Title),
        ISBN=ISNULL(@isbn, ISBN)
    WHERE id = @bookId

    SELECT Id, Title, ISBN
    FROM Books
    WHERE Id = @bookId
END;
go

