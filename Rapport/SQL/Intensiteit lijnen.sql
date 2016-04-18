USE [retdbim]
GO

SELECT lines.Name, Rides.Direction, COUNT(rides.Id) FROM
	(
		SELECT * 
		FROM Rides
	) 
	as rides
INNER JOIN
	(
		SELECT * 
		FROM Lines
	)
	as lines
on rides.LineId = lines.Id
GROUP BY Lines.Name, Rides.Direction