using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.WebPages;
using cms4seo.Common.Helpers;
using cms4seo.Model.Entities;
using cms4seo.Common.TurboJpeg;
using cms4seo.Data;
using cms4seo.Data.ConnectionString;
using cms4seo.Data.Repositories;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;

namespace cms4seo.Web.Controllers
{
    /// <summary>
    /// For CMS ContentManager upload photo in Frontend (PhotoSettings quickly effect)
    /// </summary>
    public class UploadApiController : ApiController
    {
        ISettingRepository settingRepository = new SettingRepository();

        private readonly ApplicationDbContext db = new ApplicationDbContext();
        static string _projectId = WebConfigurationManager.AppSettings["ProjectId"];
        private string folder = $"/Uploads/{_projectId.Translate()}/Photo";
        readonly string[] _photoExtension = { ".jpg", ".jpe", ".jpeg", ".bmp", ".gif", ".png", ".ico" };
        readonly string[] _jpgExtension = { ".jpg", ".jpe", ".jpeg" };
        readonly string[] _lossyImageExtension = { ".jpg", ".jpe", ".jpeg", ".webp" }; // save webp as jpeg

        private int large;
        private readonly int imageQuality;
        private readonly int maxHeight;

        private bool onlyResizeIfWider = true;

        static readonly object _object = new object();
        private static string lastUpload;

        public UploadApiController()
        {
            large = 1920;
            maxHeight = 1080;
            imageQuality = 90;
        }


        // POST: api/Upload        
        public async Task<IHttpActionResult> Post(string subFolder, string type = null, int width = 0, int height = 0)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }


            try
            {
                Photo photo;

                // set subfolder
                if (subFolder != "unknown")
                    folder += "/" + subFolder;

                // Check path and auto create 
                if (!Directory.Exists(HostingEnvironment.MapPath(folder)))
                    Directory.CreateDirectory(HostingEnvironment.MapPath(folder));

                string path = HttpContext.Current.Server.MapPath("~" + folder);
                var provider = new MultipartMemoryStreamProvider();

                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                var content = provider.Contents.FirstOrDefault();

                if (content == null)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }


                string fileName = content.Headers.ContentDisposition.FileName;
                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    fileName = fileName.Trim('"');

                if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                    fileName = Path.GetFileName(fileName);

                var extension = Path.GetExtension(fileName);

                if (!_photoExtension.Contains(extension.ToLower()))
                {
                    return Content(HttpStatusCode.UnsupportedMediaType, extension);
                }






                // prevent conflict file ========================================================                    
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                // init name
                fileName = String.Copy(nameWithoutExtension.MakeNameFriendly());


                // jpeg check $@"{path}\{fileName}-sm{extension}"
                // png check $@"{path}\{fileName}{extension}"

                // same way check with png, jpg

                var n = 0;
                while (File.Exists($@"{path}\{fileName}{extension}") || lastUpload == fileName)
                    fileName = $"{nameWithoutExtension.MakeNameFriendly()}-{++n}";

                lastUpload = fileName;
                

                // no resize for qualify banner
                var smName = $"{fileName}{extension}";
                var mdName = $"{fileName}{extension}";
                var lgName = $"{fileName}{extension}";

                var image = Image.FromStream(await content.ReadAsStreamAsync());

                var mimeType = image.GetMimeType();

                bool isUseMozJpeg = _lossyImageExtension.Contains(extension.ToLower())
                                    || mimeType == "image/jpeg"
                                    || Setting.WebSettings[PhotoSettingType.IsAutoConvertPngToJpg].AsBool();


                if (isUseMozJpeg)
                {
                    //get bitmap



                    // fix rotate for mobile upload
                    try
                    {
                        // 0x0112 is tag Orientation save on Exif Property
                        var exifOrientation = image.GetPropertyItem(0x0112); // return 1 -> 8
                        // ensure bitmap contain Property Exif
                        if (image.PropertyIdList.Contains(0x0112))
                            image.RotateFlip(Imager.GetOrientationToFlipType(exifOrientation.Value[0]));
                    }
                    catch (Exception e)
                    {
                        LogHelper.Write("UploadApiController", $"warning, message: {e.Message}");
                    }



                    if (width == 0 ^ height == 0)
                    {
                        // Resize by one side
                        var expectedImage = Imager.ResizeBy(image, width, height, onlyResizeIfWider);
                        using (MozJpeg mozJpeg = new MozJpeg())
                        {
                            mozJpeg.Save(expectedImage, $@"{path}\{lgName}", imageQuality);
                        }
                        expectedImage.Dispose();
                    }
                    else if (width > 0 && image.Width > width)
                    {
                        var expectedImage = Imager.Resize(image, width, height, onlyResizeIfWider);
                        using (MozJpeg mozJpeg = new MozJpeg())
                        {
                            mozJpeg.Save(expectedImage, $@"{path}\{lgName}", imageQuality);
                        }
                        expectedImage.Dispose();
                    }
                    else if (image.Width > 1920)
                    {
                        // not allow image wider 1920px
                        var largeImage = Imager.Resize(image, large, maxHeight, onlyResizeIfWider);
                        using (MozJpeg mozJpeg = new MozJpeg())
                        {
                            mozJpeg.Save(largeImage, $@"{path}\{lgName}", imageQuality);
                        }
                        largeImage.Dispose();
                    }
                    else if (width > 0 && image.Width < width)
                    {
                        using (MozJpeg mozJpeg = new MozJpeg())
                        {
                            mozJpeg.Save((Bitmap)image, $@"{path}\{lgName}", imageQuality);
                        }
                    }
                    else // true mode
                    {
                        image.Save($@"{path}\{lgName}", ImageFormat.Jpeg);
                    }


                    //store
                    image.Save($@"{path}\{fileName}-xl{extension}");

                    image.Dispose();
                }
                else
                {


                    //var n2 = 0;
                    //var fileName2 = String.Copy(nameWithoutExtension.MakeNameFriendly());
                    //while (File.Exists($@"{path}\{fileName2}{extension}"))
                    //    fileName2 = $"{nameWithoutExtension.MakeNameFriendly()}-{++n2}";

                    if (width > 0 && image.Width > width)
                    {
                        var expectedImage = Imager.Resize(image, width, height, onlyResizeIfWider);
                        expectedImage.Save($@"{path}\{fileName}{extension}");
                        expectedImage.Dispose();
                    }
                    else if (image.Width > 1920)
                    {
                        var largeImage = Imager.Resize(image, large, maxHeight, onlyResizeIfWider);
                        largeImage.Save($@"{path}\{fileName}{extension}");
                        largeImage.Dispose();
                    }
                    else
                    {
                        image.Save($@"{path}\{fileName}{extension}");
                    }


                    //store
                    image.Save($@"{path}\{fileName}-xl{extension}");

                    image.Dispose();

                }
                



                #region database
                photo = new Photo()
                {
                    Name = fileName,
                    SmPath = $"{folder}/{smName}",
                    MdPath = $"{folder}/{mdName}",
                    LgPath = $"{folder}/{lgName}",
                    AltAttribute = fileName,
                    MimeType = extension,
                    PostBy = User.Identity.Name,
                    //Status = Statuses.Processing,
                    //ProcessingBy = User.Identity.Name,
                    Entity = cms4seoEntityType.Contents,
                    //ModelId = modelId
                };

                db.Photos.Add(photo);
                await db.SaveChangesAsync();
                #endregion database





                return Ok(new { status = false, path = photo.LgPath, note = "some notes" });
            }
            catch(Exception e)
            {
                return Content(HttpStatusCode.NotAcceptable, e.Message);
            }

        }

        //GET: api/UploadApi?name=
        /// <summary>
        /// Return name of reserve image on server
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Get(string name)
        {
            var fileName = name;
            folder += "/Contents";

            // Check path and auto create 
            if (!Directory.Exists(HostingEnvironment.MapPath(folder)))
                Directory.CreateDirectory(HostingEnvironment.MapPath(folder));

            string path = HttpContext.Current.Server.MapPath("~" + folder);

            if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                fileName = Path.GetFileName(fileName);

            var extension = Path.GetExtension(fileName);

            // prevent conflict file ========================================================                    
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            // init name
            fileName = String.Copy(nameWithoutExtension.MakeNameFriendly());

            // same way check with png, jpg
            var n = 0;
            while (File.Exists($@"{path}\{fileName}{extension}") || lastUpload == fileName)
                fileName = $"{nameWithoutExtension.MakeNameFriendly()}-{++n}";

            lastUpload = fileName;

            // no resize for qualify banner
            var url = $"{folder}/{fileName}{extension}";

            return url;
        }

        //// GET: api/UploadApi/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/UploadApi
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/UploadApi/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/UploadApi/5
        //public void Delete(int id)
        //{
        //}
    }
}
