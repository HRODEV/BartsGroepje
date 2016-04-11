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
    builder.EntitySet<Platform>("Platforms");
    builder.EntitySet<RideStop>("RideStops"); 
    builder.EntitySet<Station>("Stations"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PlatformsController : ODataController
    {
        private RETContext db = new RETContext();

        // GET: odata/Platforms
        [EnableQuery]
        public IQueryable<Platform> GetPlatforms()
        {
            return db.Platforms;
        }

        // GET: odata/Platforms(5)
        [EnableQuery]
        public SingleResult<Platform> GetPlatform([FromODataUri] int key)
        {
            return SingleResult.Create(db.Platforms.Where(platform => platform.Id == key));
        }

        // PUT: odata/Platforms(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Platform> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Platform platform = await db.Platforms.FindAsync(key);
            if (platform == null)
            {
                return NotFound();
            }

            patch.Put(platform);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatformExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(platform);
        }

        // POST: odata/Platforms
        public async Task<IHttpActionResult> Post(Platform platform)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Platforms.Add(platform);
            await db.SaveChangesAsync();

            return Created(platform);
        }

        // PATCH: odata/Platforms(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Platform> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Platform platform = await db.Platforms.FindAsync(key);
            if (platform == null)
            {
                return NotFound();
            }

            patch.Patch(platform);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatformExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(platform);
        }

        // DELETE: odata/Platforms(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Platform platform = await db.Platforms.FindAsync(key);
            if (platform == null)
            {
                return NotFound();
            }

            db.Platforms.Remove(platform);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Platforms(5)/RideStops
        [EnableQuery]
        public IQueryable<RideStop> GetRideStops([FromODataUri] int key)
        {
            return db.Platforms.Where(m => m.Id == key).SelectMany(m => m.RideStops);
        }

        // GET: odata/Platforms(5)/Station
        [EnableQuery]
        public SingleResult<Station> GetStation([FromODataUri] int key)
        {
            return SingleResult.Create(db.Platforms.Where(m => m.Id == key).Select(m => m.Station));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlatformExists(int key)
        {
            return db.Platforms.Count(e => e.Id == key) > 0;
        }
    }
}
