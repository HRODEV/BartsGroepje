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
    builder.EntitySet<Station>("Stations");
    builder.EntitySet<Line>("Lines"); 
    builder.EntitySet<Platform>("Platforms"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class StationsController : ODataController
    {
        private RETContext db = new RETContext();

        // GET: odata/Stations
        [EnableQuery]
        public IQueryable<Station> GetStations()
        {
            return db.Stations;
        }

        // GET: odata/Stations(5)
        [EnableQuery]
        public SingleResult<Station> GetStation([FromODataUri] int key)
        {
            return SingleResult.Create(db.Stations.Where(station => station.Id == key));
        }

        // PUT: odata/Stations(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Station> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Station station = await db.Stations.FindAsync(key);
            if (station == null)
            {
                return NotFound();
            }

            patch.Put(station);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(station);
        }

        // POST: odata/Stations
        public async Task<IHttpActionResult> Post(Station station)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Stations.Add(station);
            await db.SaveChangesAsync();

            return Created(station);
        }

        // PATCH: odata/Stations(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Station> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Station station = await db.Stations.FindAsync(key);
            if (station == null)
            {
                return NotFound();
            }

            patch.Patch(station);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(station);
        }

        // DELETE: odata/Stations(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Station station = await db.Stations.FindAsync(key);
            if (station == null)
            {
                return NotFound();
            }

            db.Stations.Remove(station);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Stations(5)/Lines
        [EnableQuery]
        public IQueryable<Line> GetLines([FromODataUri] int key)
        {
            return db.Stations.Where(m => m.Id == key).SelectMany(m => m.Lines);
        }

        // GET: odata/Stations(5)/PlatForms
        [EnableQuery]
        public IQueryable<Platform> GetPlatForms([FromODataUri] int key)
        {
            return db.Stations.Where(m => m.Id == key).SelectMany(m => m.PlatForms);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StationExists(int key)
        {
            return db.Stations.Count(e => e.Id == key) > 0;
        }
    }
}
