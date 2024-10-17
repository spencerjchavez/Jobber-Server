USE jobberDB;

DROP TABLE IF EXISTS ContractorSectors;
DROP TABLE IF EXISTS Sectors;
DROP TABLE IF EXISTS ContractorJobCategories;
DROP TABLE IF EXISTS Contractors;
DROP TABLE IF EXISTS JobCategories;


CREATE TABLE Contractors (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Guid CHAR(36) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    BioShort VARCHAR(255),
    BioLong TEXT,
    Services JSON,
    ServiceArea JSON,
    ProfilePicture JSON,
    Portfolio JSON
);

CREATE TABLE JobCategories (
	Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(64) UNIQUE
);
CREATE INDEX index_id ON JobCategories(Id);
CREATE INDEX index_name ON JobCategories(Name);

CREATE TABLE ContractorJobCategories (
	ContractorId INT NOT NULL,
    JobCategoryId INT NOT NULL,
    PRIMARY KEY (ContractorId, JobCategoryId),
	FOREIGN KEY (ContractorId) REFERENCES Contractors(Id) ON DELETE CASCADE,
    FOREIGN KEY (JobCategoryId) REFERENCES JobCategories(Id) ON DELETE RESTRICT
);
CREATE INDEX index_contractor ON ContractorJobCategories (ContractorId);
CREATE INDEX index_jobcategory ON ContractorJobCategories (JobCategoryId);

-- Tree Structure Sectors Table
CREATE TABLE Sectors(
	Id INT PRIMARY KEY AUTO_INCREMENT,
    Latitude DOUBLE NOT NULL,
    Longitude DOUBLE NOT NULL,
    Depth INT NOT NULL,
    NWId INT,
    NEId INT,
    SEId INT,
    SWId INT,
    ParentId INT,
    FOREIGN KEY (NWId) REFERENCES Sectors(Id) ON DELETE SET NULL,
    FOREIGN KEY (NEId) REFERENCES Sectors(Id) ON DELETE SET NULL,
    FOREIGN KEY (SEId) REFERENCES Sectors(Id) ON DELETE SET NULL,
    FOREIGN KEY (SWId) REFERENCES Sectors(Id) ON DELETE SET NULL,
    FOREIGN KEY (ParentId) REFERENCES Sectors(Id) ON DELETE RESTRICT
);
CREATE INDEX index_parent ON Sectors(ParentId);

INSERT INTO Sectors (Id, Latitude, Longitude, Depth) VALUES (1, 0, -90, 0);
INSERT INTO Sectors (Id, Latitude, Longitude, Depth) VALUES (2, 0, 90, 0);

CREATE TABLE ContractorSectors(
	SectorId INT NOT NULL,
    ContractorId INT NOT NULL,
    ServesEntireSector BOOL NOT NULL,
    PRIMARY KEY(SectorId, ContractorId),
    FOREIGN KEY (SectorId) 
		REFERENCES Sectors(Id) 
        ON DELETE RESTRICT,
    FOREIGN KEY (ContractorId) REFERENCES Contractors(Id) ON DELETE CASCADE
);
CREATE INDEX index_sector ON ContractorSectors(SectorId);
CREATE INDEX index_contractor ON ContractorSectors(ContractorId);
CREATE INDEX index_serves_entire_sector ON ContractorSectors(ServesEntireSector);