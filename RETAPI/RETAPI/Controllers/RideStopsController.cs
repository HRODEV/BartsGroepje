using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using RETAPI.Models;

namespace RETAPI.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using RETAPI.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<RideStop>("RideStops");
    builder.EntitySet<Platform>("Platforms"); 
    builder.EntitySet<Ride>("Rides"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class RideStopsController : ODataController
    {
        private RETContext db = new RETContext();

        // GET: odata/RideStops
        [EnableQuery]
        public IQueryable<RideStop> GetRideStops()
        {
            return db.RideStops;
        }

        // GET: odata/RideStops(5)
        [EnableQuery]
        public SingleResult<RideStop> GetRideStop([FromODataUri] int key)
        {
            return SingleResult.Create(db.RideStops.Where(rideStop => rideStop.Id == key));
        }

        // PUT: odata/RideStops(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<RideStop> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RideStop rideStop = await db.RideStops.FindAsync(key);
            if (rideStop == null)
            {
                return NotFound();
            }

            patch.Put(rideStop);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RideStopExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(rideStop);
        }

        // POST: odata/RideStops
        public async Task<IHttpActionResult> Post(RideStop rideStop)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RideStops.Add(rideStop);
            await db.SaveChangesAsync();

            return Created(rideStop);
        }

        // PATCH: odata/RideStops(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<RideStop> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RideStop rideStop = await db.RideStops.FindAsync(key);
            if (rideStop == null)
            {
                return NotFound();
            }

            patch.Patch(rideStop);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RideStopExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(rideStop);
        }

        // DELETE: odata/RideStops(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            RideStop rideStop = await db.RideStops.FindAsync(key);
            if (rideStop == null)
            {
                return NotFound();
            }

            db.RideStops.Remove(rideStop);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/RideStops(5)/Platform
        [EnableQuery]
        public SingleResult<Platform> GetPlatform([FromODataUri] int key)
        {
            return SingleResult.Create(db.RideStops.Where(m => m.Id == key).Select(m => m.Platform));
        }

        // GET: odata/RideStops(5)/Ride
        [EnableQuery]
        public SingleResult<Ride> GetRide([FromODataUri] int key)
        {
            return SingleResult.Create(db.RideStops.Where(m => m.Id == key).Select(m => m.Ride));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RideStopExists(int key)
        {
            return db.RideStops.Count(e => e.Id == key) > 0;
        }
    }
}
