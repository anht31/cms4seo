using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Admin.Resources;
using cms4seo.Admin.Services;
using cms4seo.Common.Attribute;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Photo;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Basic.PhotosApi")]
    public class PhotosApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/PhotosApi
        public IQueryable<Photo> GetPhotos(string entity, int modelId)
        {

            // way 1 - query entity by relation ship (Unique Way)
            IQueryable<Photo> photos;

            #region entity selector for photo

            if (entity == cms4seoEntityType.Category)
            {
                var category = db.Categories.Find(modelId);
                photos = category?.Photos.AsQueryable();
            }
            else if (entity == cms4seoEntityType.Product)
            {
                var product = db.Products.Find(modelId);
                photos = product?.Photos.AsQueryable();
            }
            else if (entity == cms4seoEntityType.Topic)
            {
                var topic = db.Topics.Find(modelId);
                photos = topic?.Photos.AsQueryable();
            }
            else if (entity == cms4seoEntityType.Article)
            {
                var article = db.Articles.Find(modelId);
                photos = article?.Photos.AsQueryable();
            }
            else if (entity == cms4seoEntityType.Info)
            {
                var info = db.Infos.Find(modelId);
                photos = info?.Photos.AsQueryable();
            }
            else if (entity == cms4seoEntityType.Slider)
            {
                var slider = db.Sliders.Find(modelId);
                photos = slider?.Photos.AsQueryable();
            }
            else if (entity == cms4seoEntityType.Settings)
            {
                photos = db.Photos.Where(x => x.Entity == entity).AsQueryable();
            }
            else if (entity == cms4seoEntityType.Contents)
            {
                photos = db.Photos.Where(x => x.Entity == entity).AsQueryable();
            }
            else
            {
                // throw new exception
                throw new Exception(AdminResources.PhotosApiControllerGetPhotos_Not_found_entity__Please_add_Entity_in_UploadController_cs);
            }

            #endregion



            return photos.OrderBy(x => x.Sort);

            // way 2 - Query by manual property
            //return db.Photos.Where(x => x.Entity == entity && x.ModelId == modelId);
        }



        // PUT: api/PhotosApi/5/Categories/5/AddScope
        //[ActionName("Sort")]
        [ResponseType(typeof(Photo))]
        public async Task<IHttpActionResult> PutPhoto(int id, Photo data)
        {

            var photo = await db.Photos.FindAsync(id);
            
            if (photo == null)
            {
                return BadRequest();
            }

            photo.Sort = data.Sort;

            db.Entry(photo).State = EntityState.Modified;
            db.SaveChanges();

            // resort
            var photos = db.Photos
                .Where(x => x.Entity == photo.Entity && x.ModelId == photo.ModelId)
                .OrderBy(x => x.Sort)
                .ToList();

            int i = 0;
            //int ii = sort;
            foreach (var item in photos)
            {
                item.Sort = ++i*10;
            }

            db.SaveChanges();
            

            return Ok(photo);
        }




        // DELETE: api/PhotosApi/5
        //[ActionName("Delete")]
        [ResponseType(typeof(Photo))]
        public async Task<IHttpActionResult> DeletePhoto(int id)
        {
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            try
            {
                var photoService = new PhotoService();
                photoService.DeletePhoto(photo);
            }
            catch (Exception e)
            {

                LogHelper.Write(@"Admin/PhotoApi",
                    $"Access denied user: {User.Identity.Name} for try delete photo, message: {e.Message}");

                // client success response, but will recheck condition in client side
                return CreatedAtRoute("DefaultApi", null, new { success = false, message = e.Message });
            }


            db.Photos.Remove(photo);
            await db.SaveChangesAsync();


            return Ok(photo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //private bool PhotoExists(int id)
        //{
        //    return db.Photos.Count(e => e.Id == id) > 0;
        //}
    }
}