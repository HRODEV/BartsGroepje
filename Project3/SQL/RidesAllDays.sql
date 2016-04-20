USE [retdbim]
GO

SELECT rs.day, Count(*) as countRides FROM
	(SELECT * FROM Rides) as r
INNER JOIN
	(SELECT 
		*, 
		CONVERT(date, RideStops.Time) as 'day'
	FROM RideStops) 
	as rs
ON r.Id = rs.RideId

GROUP BY rs.day
ORDER BY rs.day;