USE jobberDB;

DROP TABLE IF EXISTS SectorsContractors;
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

-- GeoLocating Contractors
CREATE TABLE Sectors(
	LatitudinalSlice SMALLINT NOT NULL,
    LatitudinalSubSlice SMALLINT NOT NULL,
    LongitudinalSlice SMALLINT NOT NULL,
    PRIMARY KEY(LatitudinalSlice, LatitudinalSubSlice, LongitudinalSlice)
);

CREATE TABLE SectorContractors(
	LatitudinalSlice SMALLINT NOT NULL,
    LatitudinalSubSlice SMALLINT NOT NULL,
    LongitudinalSlice SMALLINT NOT NULL,
    ContractorId INT NOT NULL,
    PRIMARY KEY(LatitudinalSlice, LatitudinalSubSlice, LongitudinalSlice, ContractorId),
    FOREIGN KEY (LatitudinalSlice, LatitudinalSubSlice, LongitudinalSlice) 
		REFERENCES Sectors(LatitudinalSlice, LatitudinalSubSlice, LongitudinalSlice) 
        ON DELETE RESTRICT,
    FOREIGN KEY (ContractorId) REFERENCES Contractors(Id) ON DELETE CASCADE
);