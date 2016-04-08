DROP TABLE IF EXISTS `line`;
CREATE TABLE `line` (
`Id` int(11) NOT NULL,
`Name` varchar(255) CHARACTER SET latin1 NOT NULL,
PRIMARY KEY (`Id`) 
);

DROP TABLE IF EXISTS `line_has_station`;
CREATE TABLE `line_has_station` (
	`Id` int(11) NOT NULL,
	`Position` int(11) NOT NULL,
	`Station_Id` int(11) NOT NULL,
	`Line_Id` int(11) NOT NULL,
	PRIMARY KEY (`Id`) ,
	INDEX `FK_Line` (`Line_Id`),
	INDEX `Fk_Station` (`Station_Id`)
)
ENGINE=MyISAM
DEFAULT CHARACTER SET=latin1 COLLATE=latin1_swedish_ci
;

DROP TABLE IF EXISTS `perron`;
CREATE TABLE `perron` (
`Code` varchar(255) CHARACTER SET latin1 NOT NULL,
`Station_Id` int(11) NOT NULL,
`X` float(11,0) NOT NULL,
`Y` float(11,0) NOT NULL,
PRIMARY KEY (`Code`) ,
INDEX `FK_Station` (`Station_Id`)
)
ENGINE=MyISAM
DEFAULT CHARACTER SET=latin1 COLLATE=latin1_swedish_ci
;

DROP TABLE IF EXISTS `ride`;
CREATE TABLE `ride` (
`Id` int(11) NOT NULL,
`Date` datetime NOT NULL,
`Line_Id` int(11) NOT NULL,
PRIMARY KEY (`Id`) ,
INDEX `FK_Lines` (`Line_Id`)
);
DROP TABLE IF EXISTS `ride_point`;
CREATE TABLE `ride_point` (
`Id` int(11) NOT NULL,
`Ride_Id` int(11) NOT NULL,
`Perron_Code` varchar(255) CHARACTER SET latin1 NOT NULL,
`Time` time NOT NULL,
PRIMARY KEY (`Id`) ,
INDEX `FK_Ride` (`Ride_Id`),
INDEX `FK_Perron` (`Perron_Code`)
);

DROP TABLE IF EXISTS `station`;
CREATE TABLE `station` (
`Id` int(11) NOT NULL,
`Name` varchar(255) CHARACTER SET latin1 NOT NULL,
`X` float(255,0) NOT NULL,
`Y` float(255,0) NOT NULL,
PRIMARY KEY (`Id`) 
);

ALTER TABLE `station` ADD CONSTRAINT `fk_station_line_has_station_1` FOREIGN KEY (`Id`) REFERENCES `line_has_station` (`Station_Id`);

ALTER TABLE `line` ADD CONSTRAINT `fk_line_line_has_station_1` FOREIGN KEY (`Id`) REFERENCES `line_has_station` (`Line_Id`);

ALTER TABLE `perron` ADD CONSTRAINT `fk_perron_ride_point_1` FOREIGN KEY (`Code`) REFERENCES `ride_point` (`Perron_Code`);

ALTER TABLE `ride` ADD CONSTRAINT `fk_ride_ride_point_1` FOREIGN KEY (`Id`) REFERENCES `ride_point` (`Ride_Id`);

ALTER TABLE `station` ADD CONSTRAINT `fk_station_perron_1` FOREIGN KEY (`Id`) REFERENCES `perron` (`Station_Id`);

ALTER TABLE `line` ADD CONSTRAINT `fk_line_ride_1` FOREIGN KEY (`Id`) REFERENCES `ride` (`Line_Id`);





