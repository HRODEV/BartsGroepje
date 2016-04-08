DROP TABLE IF EXISTS `line`;
CREATE TABLE `line` (

`Id` int(11) AUTO_INCREMENT NOT NULL,

`Name` varchar(255) NOT NULL,

PRIMARY KEY (`Id`) 

);

DROP TABLE IF EXISTS `line_has_station`;
CREATE TABLE `line_has_station` (

`Id` int(11) AUTO_INCREMENT NOT NULL,

`Position` int(11) NOT NULL,

`Station_Id` int(11) NOT NULL,

`Line_Id` int(11) NOT NULL,

PRIMARY KEY (`Id`)

);

DROP TABLE IF EXISTS `platform`;
CREATE TABLE `platform` (

`Code` varchar(255) NOT NULL,

`Station_Id` int(11) NOT NULL,

`X` float(11,0) NOT NULL,

`Y` float(11,0) NOT NULL,

PRIMARY KEY (`Code`)

);



DROP TABLE IF EXISTS `ride`;
CREATE TABLE `ride` (

`Id` int(11) AUTO_INCREMENT NOT NULL,

`Date` datetime NOT NULL,

`Line_Id` int(11) NOT NULL,

PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `ride_stop`;
CREATE TABLE `ride_stop` (

`Id` int(11) AUTO_INCREMENT NOT NULL,

`Ride_Id` int(11) NOT NULL,

`Platform_Code` varchar(255) NOT NULL,

`Time` time NOT NULL,

PRIMARY KEY (`Id`)

);

DROP TABLE IF EXISTS `station`;
CREATE TABLE `station` (

`Id` int(11) AUTO_INCREMENT NOT NULL,

`Name` varchar(255) NOT NULL,

`X` float(255,0) NOT NULL,

`Y` float(255,0) NOT NULL,

PRIMARY KEY (`Id`) 

);

ALTER TABLE `line_has_station` ADD CONSTRAINT `fk_line_has_station_line_1` FOREIGN KEY (`Line_Id`) REFERENCES `line` (`Id`);

ALTER TABLE `ride` ADD CONSTRAINT `fk_ride_line_1` FOREIGN KEY (`Line_Id`) REFERENCES `line` (`Id`);

ALTER TABLE `platform` ADD CONSTRAINT `fk_platform_station_1` FOREIGN KEY (`Station_Id`) REFERENCES `station` (`Id`);

ALTER TABLE `ride_stop` ADD CONSTRAINT `fk_ride_point_ride_1` FOREIGN KEY (`Ride_Id`) REFERENCES `ride` (`Id`);

ALTER TABLE `ride_stop` ADD CONSTRAINT `fk_ride_point_perron_1` FOREIGN KEY (`Platform_Code`) REFERENCES `platform` (`Code`);

ALTER TABLE `line_has_station` ADD CONSTRAINT `fk_line_has_station_station_1` FOREIGN KEY (`Station_Id`) REFERENCES `station` (`Id`);