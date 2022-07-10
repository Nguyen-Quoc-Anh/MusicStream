create database MusicStreaming

create table Role(
	roleID int primary key identity(1, 1),
	name nvarchar(50)
)

create table Account (
	accountID varchar(50) primary key not null,
	email nvarchar(50) not null,
	password nvarchar(100) not null,
	roleID int references Role(roleID),
	fullname nvarchar(100) not null,
	image nvarchar(max) not null
)

create table Artist (
	artistID varchar(50) primary key not null,
	fullname nvarchar(100) not null,
	image nvarchar(max) not null,
	description nvarchar(max)
)


create table Album (
	albumID varchar(50) primary key not null,
	albumName nvarchar(200) not null,
	image nvarchar(max) not null,
	releaseDate date default getDate(),
	[view] int default 0
)

create table Artist_Album(
	artistID varchar(50) references Artist(artistID) not null,
	albumID varchar(50) references Album(albumID) not null,
	Primary key (artistID, albumID)
)

create table Track (
	trackID varchar(50) primary key not null,
	name nvarchar(100) not null,
	image nvarchar(max) not null,
	duration int default 0,
	mp3 nvarchar(max) not null,
	albumID varchar(50) references Album(albumID) not null,
	[view] int default 0
)

create table ArtistTrack(
	artistID varchar(50) references Artist(artistID) not null,
	trackID varchar(50) references Track(trackID) not null,
	Primary key (artistID, trackID)
)

create table Genre (
	genreID varchar(50) primary key not null,
	name nvarchar(100) not null
)

create table AlbumGenre(
	genreID varchar(50) references Genre(genreID) not null,
	albumID varchar(50) references Album(albumID) not null,
	Primary key (genreID, albumID)
)
create table GenreOfTrack(
	genreID varchar(50) references Genre(genreID) not null,
	trackID varchar(50) references Track(trackID) not null,
	Primary key (genreID, trackID)
)

create table Playlist(
	playlistID nvarchar(50) primary key not null,
	name nvarchar(100) not null,
	image nvarchar(max) not null,
	accountID varchar(50) references Account(accountID) not null,
	isPrivate bit default 0,
	createdTime datetime default getDate()
)

create table PlayListTrack (
	playlistID nvarchar(50) references Playlist(playlistID) not null,
	trackID varchar(50) references Track(trackID) not null,
	Primary key (playlistID, trackID)
)

create table LikeTrack (
	trackID varchar(50) references Track(trackID) not null,
	accountID varchar(50) references Account(accountID) not null,
	Primary key (accountID, trackID)
)

create table Comment(
	commentID varchar(50) primary key not null,
	content nvarchar(300) not null,
	accountID varchar(50) references Account(accountID) not null,
	trackID varchar(50) references Track(trackID) not null,
	parentID varchar(50) references Comment(commentID)
)

create table PlayListFollow (
	accountID varchar(50) references Account(accountID) not null,
	playlistID nvarchar(50) references Playlist(playlistID) not null,
	Primary key (accountID, playlistID)
)

insert into Role (name) values (N'Admin'), (N'User')

