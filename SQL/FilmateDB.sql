CREATE DATABASE FilmateDB;
GO
USE FilmateDB;
GO


-- Suggestion Table
CREATE TABLE Suggestion(
    SuggestionID INT NOT NULL IDENTITY(10000, 1),
    AccountID INT NOT NULL,
    OriginalMovieID INT NOT NULL,
    SuggestionMovieID INT NOT NULL,
    Upvotes INT NOT NULL DEFAULT 0,
    Downvotes INT NOT NULL DEFAULT 0,
    PostDate DATETIME NOT NULL DEFAULT GETDATE()
);
ALTER TABLE
    Suggestion ADD CONSTRAINT PK_Suggestion_SuggestionID PRIMARY KEY(SuggestionID);


-- Liked Movies Table
CREATE TABLE LikedMovies(
    AccountID INT NOT NULL,
    MovieID INT NOT NULL
);
ALTER TABLE
    LikedMovies ADD CONSTRAINT PK_LikedMovies_AccountID_MovieID PRIMARY KEY(AccountID, MovieID);


-- Account Table
CREATE TABLE Account(
    AccountID INT NOT NULL IDENTITY(10000, 1),
    AccountName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Username NVARCHAR(255) NOT NULL,
    Pass NVARCHAR(1000) NOT NULL,
    Age INT NOT NULL,
    ProfilePicture NVARCHAR(255) NOT NULL DEFAULT 'default_pfp.jpg',
    IsAdmin BIT NOT NULL DEFAULT 0,
    Salt NVARCHAR(255) NOT NULL UNIQUE,
	SignUpDate DATE NOT NULL DEFAULT GETDATE(),
);
ALTER TABLE
    Account ADD CONSTRAINT PK_Account_AccountID PRIMARY KEY(AccountID);
CREATE UNIQUE INDEX I_Account_Email ON
    Account(Email);
CREATE UNIQUE INDEX I_Account_Username ON
    Account(Username);


-- Review Table
CREATE TABLE Review(
    ReviewID INT NOT NULL IDENTITY(10000, 1),
    AccountID INT NOT NULL,
    Rating INT NOT NULL,
    Content NVARCHAR(255) NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    PostDate DATETIME NOT NULL DEFAULT GETDATE(),
    Upvotes INT NOT NULL DEFAULT 0,
    Downvotes INT NOT NULL DEFAULT 0
);
ALTER TABLE
    Review ADD CONSTRAINT PK_Review_ReviewID PRIMARY KEY(ReviewID);


-- Chat Table
CREATE TABLE Chat(
    ChatID INT NOT NULL IDENTITY(10000, 1),
    ChatName NVARCHAR(255) NOT NULL,
    ChatDescription INT NOT NULL,
    CreationDate DATETIME NOT NULL DEFAULT GETDATE(),
    Icon NVARCHAR(255) NOT NULL -- DEFAULT '/imgs/default_chat_icon.png'
);
ALTER TABLE
    Chat ADD CONSTRAINT PK_Chat_ChatID PRIMARY KEY(ChatID);


-- Chat Members Table
CREATE TABLE ChatMembers(
    AccountID INT NOT NULL,
    ChatID INT NOT NULL
);
ALTER TABLE
    ChatMembers ADD CONSTRAINT PK_ChatMembers_AccountID_ChatID PRIMARY KEY(AccountID, ChatID);


-- Chat Suggestions Table
CREATE TABLE ChatSuggestions(
    ChatID INT NOT NULL,
    MovieID INT NOT NULL,
	IsActive BIT NOT NULL DEFAULT 1
);


-- Message Table
CREATE TABLE Msg(
    MsgID INT NOT NULL IDENTITY(10000, 1),
    AccountID INT NOT NULL,
    ChatID INT NOT NULL,
    Content NVARCHAR(255) NOT NULL,
    SentDate DATETIME NOT NULL DEFAULT GETDATE()
);
ALTER TABLE
    Msg ADD CONSTRAINT PK_Msg_MsgID PRIMARY KEY(MsgID);


-- User Auth Token Table
CREATE TABLE UserAuthToken(
    AccountID INT NOT NULL,
    AuthToken NVARCHAR(255) NOT NULL,
	CreationDate DATETIME NOT NULL DEFAULT GETDATE()
);
ALTER TABLE
    UserAuthToken ADD CONSTRAINT PK_UserAuthToken_AuthToken PRIMARY KEY(AuthToken);


-- Account Votes History Table
CREATE TABLE AccountVotesHistory(
    VoteID INT NOT NULL IDENTITY(10000, 1),
    SuggestionID INT NOT NULL,
    AccountID INT NOT NULL,
    VotedDate DATETIME NOT NULL DEFAULT GETDATE(),
    VoteType BIT NOT NULL
);
ALTER TABLE
    AccountVotesHistory ADD CONSTRAINT PK_AccountVotesHistory_VoteID PRIMARY KEY(VoteID);


-- Foreign Keys
ALTER TABLE
    LikedMovies ADD CONSTRAINT FK_LikedMovies_AccountID FOREIGN KEY(AccountID) REFERENCES Account(AccountID);
ALTER TABLE
    AccountVotesHistory ADD CONSTRAINT FK_AccountVoteHistory_AccountID FOREIGN KEY(AccountID) REFERENCES Account(AccountID);
ALTER TABLE
    Suggestion ADD CONSTRAINT FK_Suggestion_AccountID FOREIGN KEY(AccountID) REFERENCES Account(AccountID);
ALTER TABLE
    ChatMembers ADD CONSTRAINT FK_ChatMembers_AccountID FOREIGN KEY(AccountID) REFERENCES Account(AccountID);
ALTER TABLE
    ChatMembers ADD CONSTRAINT FK_ChatMembers_ChatID FOREIGN KEY(ChatID) REFERENCES Chat(ChatID);
ALTER TABLE
    Review ADD CONSTRAINT FK_Review_AccountID FOREIGN KEY(AccountID) REFERENCES Account(AccountID);
ALTER TABLE
    ChatSuggestions ADD CONSTRAINT FK_ChatSuggestions_ChatID FOREIGN KEY(ChatID) REFERENCES Chat(ChatID);
ALTER TABLE
    Msg ADD CONSTRAINT FK_Msg_ChatID FOREIGN KEY(ChatID) REFERENCES Chat(ChatID);
ALTER TABLE
    Msg ADD CONSTRAINT FK_Msg_AccountID FOREIGN KEY(AccountID) REFERENCES Account(AccountID);
ALTER TABLE
    UserAuthToken ADD CONSTRAINT FK_UserAuthToken_AccountID FOREIGN KEY(AccountID) REFERENCES Account(AccountID);
ALTER TABLE
    AccountVotesHistory ADD CONSTRAINT FK_AccountVotesHistory_SuggestionID FOREIGN KEY(SuggestionID) REFERENCES Suggestion(SuggestionID);