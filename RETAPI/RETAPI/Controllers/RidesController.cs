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
    builder.EntitySet<Ride>("Rides");
    builder.EntitySet<RideStop>("RideStops"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class RidesController : ODataController
    {
        private RETContext db = new RETContext();

        // GET: odata/Rides
        [EnableQuery]
        public IQueryable<Ride> GetRides()
        {
            return db.Rides;
        }

        // GET: odata/Rides(5)
        [EnableQuery]
        public SingleResult<Ride> GetRide([FromODataUri] int key)
        {
            return SingleResult.Create(db.Rides.Where(ride => ride.Id == key));
        }

        // PUT: odata/Rides(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Ride> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ride ride = await db.Rides.FindAsync(key);
            if (ride == null)
            {
                return NotFound();
            }

            patch.Put(ride);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RideExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ride);
        }

        // POST: odata/Rides
        public async Task<IHttpActionResult> Post(Ride ride)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Rides.Add(ride);
            await db.SaveChangesAsync();

            return Created(ride);
        }

        // PATCH: odata/Rides(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Ride> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ride ride = await db.Rides.FindAsync(key);
            if (ride == null)
            {
                return NotFound();
            }

            patch.Patch(ride);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RideExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ride);
        }

        // DELETE: odata/Rides(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Ride ride = await db.Rides.FindAsync(key);
            if (ride == null)
            {
                return NotFound();
            }

            db.Rides.Remove(ride);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Rides(5)/RideStops
        [EnableQuery]
        public IQueryable<RideStop> GetRideStops([FromODataUri] int key)
        {
            return db.Rides.Where(m => m.Id == key).SelectMany(m => m.RideStops);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RideExists(int key)
        {
            return db.Rides.Count(e => e.Id == key) > 0;
        }
    }
}
