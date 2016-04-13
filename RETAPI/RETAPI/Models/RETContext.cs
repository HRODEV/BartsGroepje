namespace RETAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public class RETContext : DbContext
    {
        // Your context has been configured to use a 'RETContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'RETAPI.Models.RETContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'RETContext' 
        // connection string in the application configuration file.
        public RETContext()
            : base("name=RETContext")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<Station> Stations { get; set; }
        public virtual DbSet<Line> Lines { get; set; }
        public virtual DbSet<Platform> Platforms { get; set; }
        public virtual DbSet<Ride> Rides { get; set; }
        public virtual DbSet<RideStop> RideStops { get; set; }

    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public virtual ICollection<Line> Lines { get; set; }
        public virtual ICollection<Platform> PlatForms { get; set; }
    }
    public class Line
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Station> Stations { get; set; }
        public virtual ICollection<Ride> Rides { get; set; }
    }
    public class Platform
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        [ForeignKey("Station")]
        public int StationID { get; set; }
        public Station Station { get; set; }
        public virtual ICollection<RideStop> RideStops { get; set; }
    }
    public class Ride
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("Line")]
        public int LineId { get; set; }
        public Line Line { get; set; }
        public virtual ICollection<RideStop> RideStops { get; set; }
    }
    public class RideStop
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        [ForeignKey("Ride")]
        public int RideId { get; set; }
        public Ride Ride { get; set; }
        [ForeignKey("Platform")]
        public int PlatformId { get; set; }
        public Platform Platform { get; set; }
    }

}