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
            references Author (Id),
    BookId int not null
        constraint AuthorBooks_Books_Id_fk
            references Books,
    constraint AuthorBooks_pk
        primary key nonclustered (AuthorId, BookId)
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

