USE jobberDB;

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
    ProfilePicture VARCHAR(255),
    ProfilePictureThumbnail VARCHAR(255),
    Portfolio JSON,
    PortfolioThumbnails JSON
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
    FOREIGN KEY (JobCategoryId) REFERENCES JobCategories(Id) ON DELETE CASCADE
);
CREATE INDEX index_contractor ON ContractorJobCategories (ContractorId);
CREATE INDEX index_jobcategory ON ContractorJobCategories (JobCategoryId);
