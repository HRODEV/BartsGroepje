USE [retdbim]
GO

SELECT HoursRides.rideHour, COUNT(HoursRides.Id) as 'count' FROM 
	(SELECT r.Id, rs.rideHour FROM
		(SELECT * FROM Rides) as r
	INNER JOIN
		(SELECT *, DATEPART(HOUR, RideStops.Time) as 'rideHour' FROM RideStops) as rs
	ON r.Id = rs.RideId

	GROUP BY r.Id, rs.rideHour) as HoursRides
GROUP BY HoursRides.rideHour;


SELECT HoursRides.rideDay, HoursRides.rideHour, COUNT(HoursRides.Id) as 'count' FROM 
	(SELECT r.Id, rs.rideHour, rs.rideDay FROM
		(SELECT * FROM Rides) as r
	INNER JOIN
		(SELECT 
			*, 
			DATEPART(HOUR, RideStops.Time) as 'rideHour',
			DATEPART(WEEKDAY, RideStops.Time) as 'rideDay'
		FROM RideStops) 
		as rs
	ON r.Id = rs.RideId

	GROUP BY r.Id, rs.rideHour, rs.rideDay) as HoursRides
GROUP BY HoursRides.rideDay, HoursRides.rideHour
ORDER BY HoursRides.rideDay, HoursRides.rideHour;