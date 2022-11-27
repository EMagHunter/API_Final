CREATE DATABASE MatchmakingGameService;

USE MatchmakingGameService;

CREATE TABLE Players(
	PlayerId INT NOT NULL AUTO_INCREMENT,
	PlayerName varchar(20) NOT NULL,
	IsOnline bool NOT NULL,
	PlayerRegion varchar(10) NOT NULL,
	PRIMARY KEY (PlayerId)
);

CREATE TABLE Matches(
	MatchId INT NOT NULL AUTO_INCREMENT,
	Player1 int NOT NULL,
	Player2 int NOT NULL,
	Winner int NOT NULL,
	MatchTimestamp DateTime NOT NULL,
	PRIMARY KEY (MatchId)
);

ALTER TABLE Matches ADD CONSTRAINT FK_Player1 FOREIGN KEY (Player1) REFERENCES Players(PlayerId);
ALTER TABLE Matches ADD CONSTRAINT FK_Player2 FOREIGN KEY (Player2) REFERENCES Players(PlayerId);
ALTER TABLE Matches ADD CONSTRAINT FK_Winner FOREIGN KEY (Winner) REFERENCES Players(PlayerId);

ALTER TABLE Players ADD CONSTRAINT UC_PlayerName UNIQUE (PlayerName);