USE [retdbim]
GO

SELECT 
	CONVERT(date, Rides.Date) as d,
	Count(*)
FROM Rides
GROUP BY CONVERT(date, Rides.Date)
Order By d;