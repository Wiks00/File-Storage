CREATE DATABASE Storage
    ON
    PRIMARY ( NAME = Storage1,
        FILENAME = '<<path>>\Storagedat1.mdf'),
    FILEGROUP FileStreamGroup1 CONTAINS FILESTREAM( NAME = Storage3,
        FILENAME = '<<path>>\filestream1')
    LOG ON  ( NAME = Archlog1,
        FILENAME = '<<path>>\Storagelog1.ldf')
    GO

CREATE TABLE Folders (
	id bigint PRIMARY KEY,
	ownerId bigint NOT NULL UNIQUE,
	name VARCHAR(1000) NOT NULL,
	dateTime DATE NOT NULL,
	level INT NOT NULL,
	leftKey INT NOT NULL,
	rightKey INT NOT NULL
);

CREATE TABLE Users (
	id bigint IDENTITY(1,1) PRIMARY KEY,
	login VARCHAR(50) NOT NULL UNIQUE,
	email VARCHAR(100) NOT NULL UNIQUE,
	password VARCHAR(1000) NOT NULL UNIQUE
);

CREATE TABLE Roles (
	id bigint IDENTITY(1,1) PRIMARY KEY,
	role VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Files (
	id bigint IDENTITY(1,1) PRIMARY KEY,
	idFile UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL UNIQUE,
	content varbinary(max) FILESTREAM  NULL,
	folderId bigint NOT NULL,
	dateTime DATE NOT NULL,
	name VARCHAR(1000) NOT NULL
);

CREATE TABLE FileTypes (
	id bigint IDENTITY(1,1) PRIMARY KEY,
	typeName VARCHAR(100) NOT NULL UNIQUE,
	format VARCHAR(1000) NOT NULL
);

CREATE TABLE Membership (
	userId bigint NOT NULL,
	roleId bigint NOT NULL
);

CREATE TABLE Association (
	fileId bigint NOT NULL,
	fileTypeId bigint NOT NULL
);

CREATE TABLE Share (
	userId bigint NOT NULL,
	folderId bigint NOT NULL
);

ALTER TABLE Folders ADD CONSTRAINT Folders_fk0 FOREIGN KEY (ownerID) REFERENCES Users(id);

ALTER TABLE Files ADD CONSTRAINT Files_fk0 FOREIGN KEY (folderId) REFERENCES Folders(id);

ALTER TABLE Files ADD  CONSTRAINT DF_Files_IdFile  DEFAULT (newid()) FOR idFile;

ALTER TABLE Membership ADD CONSTRAINT Membership_fk0 FOREIGN KEY (userId) REFERENCES Users(id);

ALTER TABLE Membership ADD CONSTRAINT Membership_fk1 FOREIGN KEY (roleId) REFERENCES Roles(id);

ALTER TABLE Association ADD CONSTRAINT Association_fk0 FOREIGN KEY (fileId) REFERENCES Files(id);

ALTER TABLE Association ADD CONSTRAINT Association_fk1 FOREIGN KEY (fileTypeId) REFERENCES FileTypes(id);

ALTER TABLE Share ADD CONSTRAINT Share_fk0 FOREIGN KEY (userId) REFERENCES Users(id);

ALTER TABLE Share ADD CONSTRAINT Share_fk1 FOREIGN KEY (folderId) REFERENCES Folders(id);