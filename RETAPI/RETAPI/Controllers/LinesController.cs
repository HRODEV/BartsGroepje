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
    builder.EntitySet<Line>("Lines");
    builder.EntitySet<Station>("Stations"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class LinesController : ODataController
    {
        private RETContext db = new RETContext();

        // GET: odata/Lines
        [EnableQuery]
        public IQueryable<Line> GetLines()
        {
            return db.Lines;
        }

        // GET: odata/Lines(5)
        [EnableQuery]
        public SingleResult<Line> GetLine([FromODataUri] int key)
        {
            return SingleResult.Create(db.Lines.Where(line => line.Id == key));
        }

        // PUT: odata/Lines(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Line> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Line line = await db.Lines.FindAsync(key);
            if (line == null)
            {
                return NotFound();
            }

            patch.Put(line);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(line);
        }

        // POST: odata/Lines
        public async Task<IHttpActionResult> Post(Line line)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Lines.Add(line);
            await db.SaveChangesAsync();

            return Created(line);
        }

        // PATCH: odata/Lines(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Line> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Line line = await db.Lines.FindAsync(key);
            if (line == null)
            {
                return NotFound();
            }

            patch.Patch(line);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(line);
        }

        // DELETE: odata/Lines(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Line line = await db.Lines.FindAsync(key);
            if (line == null)
            {
                return NotFound();
            }

            db.Lines.Remove(line);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Lines(5)/Stations
        [EnableQuery]
        public IQueryable<Station> GetStations([FromODataUri] int key)
        {
            return db.Lines.Where(m => m.Id == key).SelectMany(m => m.Stations);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LineExists(int key)
        {
            return db.Lines.Count(e => e.Id == key) > 0;
        }
    }
}
