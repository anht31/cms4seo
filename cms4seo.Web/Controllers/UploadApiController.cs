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
using cms4seo.Common.Helpers;
using cms4seo.Model.Entities;
using cms4seo.Common.TurboJpeg;
using cms4seo.Data;
using cms4seo.Data.ConnectionString;
using cms4seo.Data.Repositories;
using cms4seo.Model.LekimaxType;
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

        private int large;
        private readonly int imageQuality;
        private readonly int maxHeight;

        private bool onlyResizeIfWider = true;


        public UploadApiController()
        {
            large = 1920;
            maxHeight = 1080;
            imageQuality = 90;
        }


        // POST: api/Upload        
        public async Task<IHttpActionResult> Post(string subFolder)
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
                while (File.Exists($@"{path}\{fileName}{extension}"))
                    fileName = $"{nameWithoutExtension.MakeNameFriendly()}-{++n}";


                // no resize for qualify banner
                var smName = $"{fileName}{extension}";
                var mdName = $"{fileName}{extension}";
                var lgName = $"{fileName}{extension}";



                if (_jpgExtension.Contains(extension.ToLower()))
                {
                    //get bitmap
                    var bitmap = (Bitmap)Image.FromStream(await content.ReadAsStreamAsync());


                    // fix rotate for mobile upload
                    try
                    {
                        // 0x0112 is tag Orientation save on Exif Property
                        var exifOrientation = bitmap.GetPropertyItem(0x0112); // return 1 -> 8
                        // ensure bitmap contain Property Exif
                        if (bitmap.PropertyIdList.Contains(0x0112))
                            bitmap.RotateFlip(Imager.GetOrientationToFlipType(exifOrientation.Value[0]));
                    }
                    catch (Exception e)
                    {
                        LogHelper.Write("UploadApiController", $"warning, message: {e.Message}");
                    }


                    // no resize for qualify banner
                    //var smallImage = Imager.Resize(bitmap, small, maxHeight, onlyResizeIfWider);
                    //var mediumImage = Imager.Resize(bitmap, medium, maxHeight, onlyResizeIfWider);
                    //var largeImage = Imager.Resize(bitmap, large, maxHeight, onlyResizeIfWider);

                    //using (MozJpeg mozJpeg = new MozJpeg())
                    //{
                    //    mozJpeg.Save(smallImage, $@"{path}\{smName}", imageQuality);
                    //    //default imageQuality 75 -> 85
                    //    mozJpeg.Save(mediumImage, $@"{path}\{mdName}", imageQuality);
                    //    //default imageQuality 75 -> 85
                    //    mozJpeg.Save(largeImage, $@"{path}\{lgName}", imageQuality);
                    //    //default imageQuality 75 -> 85
                    //}

                    //bitmap.Dispose();
                    //smallImage.Dispose();
                    //mediumImage.Dispose();
                    //largeImage.Dispose();
                    

                    // not allow image wider 1920px
                    if (bitmap.Width > 1920)
                    {
                        var largeImage = Imager.Resize(bitmap, large, maxHeight, onlyResizeIfWider);
                        using (MozJpeg mozJpeg = new MozJpeg())
                        {
                            mozJpeg.Save(largeImage, $@"{path}\{lgName}", imageQuality);
                            //default imageQuality 75 -> 85
                        }
                    }
                    else
                    {
                        bitmap.Save($@"{path}\{lgName}", ImageFormat.Jpeg);
                    }
                    



                    bitmap.Dispose();
                }
                else
                {


                    //var n2 = 0;
                    //var fileName2 = String.Copy(nameWithoutExtension.MakeNameFriendly());
                    //while (File.Exists($@"{path}\{fileName2}{extension}"))
                    //    fileName2 = $"{nameWithoutExtension.MakeNameFriendly()}-{++n2}";


                    // some stuff to save file
                    using (var fileStream = File.Create($@"{path}\{fileName}{extension}"))
                    {
                        (await content.ReadAsStreamAsync()).CopyTo(fileStream);
                    }

                    // other image will not resize, thus no need rename extra like sm, md, lg
                    //smName = $"{fileName}{extension}";
                    //mdName = $"{fileName}{extension}";
                    //lgName = $"{fileName}{extension}";
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
