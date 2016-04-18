USE [retdbim]
GO

SELECT HoursRides.Name, COUNT(HoursRides.Id) as 'count' FROM 
	(SELECT stations.Name, rs.Id FROM
		(SELECT 
			*
		FROM RideStops) 
		as rs
	INNER JOIN
		(SELECT * 
		FROM Platforms
		) as platforms
	ON rs.PlatformId = platforms.Id
	INNER JOIN
		(SELECT * FROM Stations) as stations
	on stations.Id = platforms.StationID

	GROUP BY stations.Name, rs.Id) as HoursRides
GROUP BY HoursRides.Name
ORDER BY 'count' DESC;
