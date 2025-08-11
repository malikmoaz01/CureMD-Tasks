CREATE TABLE Author (
    AuthorID INT PRIMARY KEY,
    AuthorName VARCHAR(100) NOT NULL,
    DateOfBirth DATE
);

CREATE TABLE Publisher (
    PublisherID INT PRIMARY KEY,
    PublisherName VARCHAR(100) NOT NULL,
    Address VARCHAR(200)
);

CREATE TABLE Book (
    BookID INT PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    AuthorID INT,
    PublisherID INT,
    PublicationDate DATE,
    Price DECIMAL(10,2) DEFAULT 0,
    Genre VARCHAR(500),
    ISBN CHAR(13) UNIQUE,

    CONSTRAINT fk_author FOREIGN KEY (AuthorID) REFERENCES Author(AuthorID),
    CONSTRAINT fk_publisher FOREIGN KEY (PublisherID) REFERENCES Publisher(PublisherID)
);

CREATE TABLE Borrower (
    BorrowerID INT PRIMARY KEY,
    BorrowerName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE,
    Phone CHAR(10)
);

CREATE TABLE Loan (
    LoanID INT PRIMARY KEY,
    BookID INT,
    BorrowerID INT,
    LoanDate DATE DEFAULT CAST(GETDATE() AS DATE),
    ReturnDate DATE,

    CONSTRAINT fk_book FOREIGN KEY (BookID) REFERENCES Book(BookID),
    CONSTRAINT fk_borrower FOREIGN KEY (BorrowerID) REFERENCES Borrower(BorrowerID)
);

-- CREATE TABLE Emp(
--	EmpID INT ,
-- );

-- Query 2
	ALTER TABLE Borrower
	ALTER COLUMN BorrowerName VARCHAR(100) NOT NULL;

-- Query 3 
	ALTER TABLE Book
	ADD CONSTRAINT unique_isbn UNIQUE (ISBN);

-- Query 4 

	ALTER TABLE Book
	ADD CONSTRAINT chk_price_positive CHECK (Price > 0);

--Query 5
	INSERT INTO Author (AuthorID, AuthorName, DateOfBirth) VALUES
	(1, 'Ali', '1980-01-15'),
	(2, 'Ahmed', '1975-06-20'),
	(3, 'Awais', '1990-09-05');


--Query 6 

	INSERT INTO Publisher (PublisherID, PublisherName, Address) VALUES
	(1, 'Lahore Book House', 'Mall Road, Lahore'),
	(2, 'Karachi Publications', 'Shahrah-e-Faisal, Karachi');


-- Query 7

	INSERT INTO Book (BookID, Title, AuthorID, PublisherID, PublicationDate, Price, Genre, ISBN) VALUES
	(1, 'Journey Through Lahore', 1, 1, '2010-03-15', 22.50, 'Travel', '9780000000001'),
	(2, 'The Soul of Karachi', 2, 2, '2015-07-10', 18.00, 'Historical', '9780000000002'),
	(3, 'Code in Urdu', 3, 1, '2020-11-25', 30.00, 'Technology', '9780000000003'),
	(4, 'Mysteries of Multan', 1, 2, '2018-05-01', 25.00, 'Fiction', '9780000000004'),
	(5, 'Cultural Tapestry', 2, 1, '2022-09-12', 15.00, NULL, '9780000000005');

-- Query 8

	INSERT INTO Borrower (BorrowerID, BorrowerName, Email, Phone) VALUES
	(1, 'Hassan', 'hassan@gmail.com', '0312345678'),
	(2, 'Sara', 'sara@gmail.com', '0321987654');

-- Query 9

	INSERT INTO Loan (LoanID, BookID, BorrowerID, LoanDate, ReturnDate) VALUES
	(1, 1, 1, '2025-08-01', '2025-08-05'),
	(2, 3, 2, '2025-08-03', NULL),
	(3, 4, 1, '2025-08-04', '2025-08-06');

-- Query 10 

	SELECT * FROM Book
	WHERE Price > 20;

-- Query 11
	SELECT B.Title , Br.BorrowerName FROM Book B
	INNER JOIN Loan L ON L.BookID = B.BookID
	INNER JOIN Borrower Br ON Br.BorrowerID = L.BorrowerID
	

-- Query 12
	SELECT A.AuthorName , count(B.BookID) AS Noofbooks FROM  Author A 
	INNER JOIN Book B ON A.AuthorID = B.AuthorID
	GROUP BY A.AuthorName
	HAVING COUNT(B.BookID) > 2 

-- Query 13 
	SELECT B.BookID, B.Title, Br.BorrowerName FROM Book B
	LEFT JOIN Loan L ON B.BookID = L.BookID AND L.ReturnDate IS NULL
	LEFT JOIN Borrower Br ON L.BorrowerID = Br.BorrowerID
	ORDER BY B.BookID;

-- Query 14 
	-- select * from book
	SELECT P.PublisherID , P.PublisherName , B.Title FROM Publisher P 
	LEFT JOIN Book B ON P.PublisherID = B.PublisherID
	
-- Query 15 
	
	SELECT DISTINCT Genre from Book 

-- Query 16 
	SELECT * FROM Book
	WHERE Title LIKE 'The%'
	OR
	Title LIKE '%SQL%'
	
-- Query 17

--  INSERT INTO Loan (LoanID, BookID, BorrowerID, LoanDate, ReturnDate) VALUES
--    (4, 1, 1, '2025-08-05', '2025-08-10'),
--     (5, 1, 1, '2025-08-06', NULL),
--     (6, 1, 1, '2025-08-07', '2025-08-12'),
--     (7, 1, 1, '2025-08-08', NULL),
--     (8, 1, 1, '2025-08-09', '2025-08-14');

	SELECT TOP 5 * FROM Loan L
	ORDER BY L.LoanDate DESC  


-- Query 18
	UPDATE Loan 
	SET ReturnDate = GETDATE()
	where LoanID = 2
	-- SELECT * FROM Loan L

-- Query 19 
--	INSERT INTO Book (BookID, Title, AuthorID, PublisherID, PublicationDate, Price, Genre, ISBN) VALUES
--  (7, 'Journey Through Lahore', 1, 1, '2009-03-15', 22.50, 'Travel', '9780000000007');

	UPDATE Book 
	SET Price = price * 1.10
	where PublicationDate < '2010-01-01'
	-- Select * FROM Book

-- Query 20 
	DELETE B FROM Borrower B
	INNER JOIN Loan L ON L.BorrowerID = B.BorrowerID
	WHERE L.LoanDate IS NULL;

-- Query 21 
-- INSERT INTO Book (BookID, Title, AuthorID, PublisherID, PublicationDate, Price, Genre, ISBN) VALUES
--  (8, 'Journey Through Lahore', 1, 1, '2009-03-15', 2.50, 'Travel', '9780007000007');
  -- Select * FROM Book
	DELETE FROM Book
	WHERE Price < 5

-- Query 22 
	ALTER TABLE Publisher 
	DROP COLUMN Address
	-- SELECT * FROM PUBLISHER

-- Query 23 
	DROP TABLE Loan

-- Query 24

-- 	CREATE TABLE Loan (
--     LoanID INT PRIMARY KEY,
--     BookID INT,
--     BorrowerID INT,
--     LoanDate DATE 


-- DEFAULT CAST(GETDATE() AS DATE),



--     ReturnDate DATE,

--     CONSTRAINT fk_book FOREIGN KEY (BookID) REFERENCES Book(BookID),
--     CONSTRAINT fk_borrower FOREIGN KEY (BorrowerID) REFERENCES Borrower(BorrowerID)
-- );

	-- DEFAULT CAST(GETDATE() AS DATE),

	INSERT INTO Loan (LoanID, BookID, BorrowerID, ReturnDate)
	VALUES (10, 2, 1, NULL);

	-- SELECT * FROM Loan


-- Query 25
	CREATE TABLE BookCategories (
		BookID INT,
		Category VARCHAR(100),
		PRIMARY KEY (BookID, Category),
		CONSTRAINT fk_book
			FOREIGN KEY (BookID) REFERENCES Book(BookID)
	);

-- Query 26 
	SELECT Br.BorrowerID, COUNT(L.LoanID) AS countloans FROM Borrower Br
	LEFT JOIN Loan L ON L.BorrowerID = Br.BorrowerID
	GROUP BY Br.BorrowerID
	ORDER BY countloans DESC;
	

-- Query 27 
	UPDATE Book 
	SET Genre = 'Unknown'
	WHERE 
	Genre is Null
	-- Select * from Book

-- Query 28 
	-- DELETE FROM Loan
	-- WHERE LoanDate < '2024-08-08';
	DELETE FROM Loan
	WHERE LoanDate < DATEADD(year, -1, GETDATE());

-- Query 29
	-- Loan(LoanID, BookTitle, BorrowerName, LoanDate)
	-- Primary Key: LoanID

	-- Borrower(BorrowerName, BorrowerAddress)
	-- Primary Key: BorrowerName
