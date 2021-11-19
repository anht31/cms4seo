using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using cms4seo.Common.Helpers;
using cms4seo.Data;

namespace cms4seo.Service.Photo
{
    public class PhotoService : IDisposable
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// Delete Physical photo (not delete photo model)
        /// </summary>
        /// <param name="photoList">photo list</param>
        public void DeletePhotoList(ICollection<Model.Entities.Photo> photoList)
        {

            if (!photoList.Any())
                return;

            foreach (var photo in photoList)
            {

                if(photo == null)
                    continue;

                if (File.Exists(HostingEnvironment.MapPath(photo.SmPath)))
                    // ReSharper disable once AssignNullToNotNullAttribute
                    File.Delete(HostingEnvironment.MapPath(photo.SmPath));
                if (File.Exists(HostingEnvironment.MapPath(photo.MdPath)))
                    // ReSharper disable once AssignNullToNotNullAttribute
                    File.Delete(HostingEnvironment.MapPath(photo.MdPath));
                if (File.Exists(HostingEnvironment.MapPath(photo.LgPath)))
                    // ReSharper disable once AssignNullToNotNullAttribute
                    File.Delete(HostingEnvironment.MapPath(photo.LgPath));
                
            }
        }



        /// <summary>
        /// Delete a physical photo (not photo model)
        /// </summary>
        /// <param name="photo"></param>
        public void DeletePhoto(Model.Entities.Photo photo)
        {

            if (photo == null)
                return;

            if (File.Exists(HostingEnvironment.MapPath(photo.SmPath)))
                // ReSharper disable once AssignNullToNotNullAttribute
                File.Delete(HostingEnvironment.MapPath(photo.SmPath));
            if (File.Exists(HostingEnvironment.MapPath(photo.MdPath)))
                // ReSharper disable once AssignNullToNotNullAttribute
                File.Delete(HostingEnvironment.MapPath(photo.MdPath));
            if (File.Exists(HostingEnvironment.MapPath(photo.LgPath)))
                // ReSharper disable once AssignNullToNotNullAttribute
                File.Delete(HostingEnvironment.MapPath(photo.LgPath));

            if (File.Exists(HostingEnvironment.MapPath(photo.LgPath.Huge())))
                // ReSharper disable once AssignNullToNotNullAttribute
                File.Delete(HostingEnvironment.MapPath(photo.LgPath.Huge()));
        }



        #region Dispose
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    
}