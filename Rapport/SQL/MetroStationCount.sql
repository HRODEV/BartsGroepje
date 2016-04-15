
USE retdbim
GO

--stations on line
SELECT 
	Lines.Name, Count(*)
FROM Lines, Stations, StationLines
WHERE
	StationLines.Line_Id = Lines.Id AND StationLines.Station_Id = Stations.Id
GROUP BY Lines.Name;

--lines on station
SELECT 
	Stations.Name, Count(*)
FROM Lines, Stations, StationLines
WHERE
	StationLines.Line_Id = Lines.Id AND StationLines.Station_Id = Stations.Id
GROUP BY Stations.Name;