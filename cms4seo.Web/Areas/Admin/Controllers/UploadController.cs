using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.Helpers;
using cms4seo.Model.Entities;
using cms4seo.Common.TurboJpeg;
using cms4seo.Data;
using cms4seo.Data.ConnectionString;
using cms4seo.Data.Repositories;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Admin.Controllers
{

    [CustomAuthorize(Roles = "Basic.Upload")]
    public class UploadController : ApiController
    {

        ISettingRepository settingRepository = new SettingRepository();

        //private static readonly PhotoSettings PhotoSettings = new PhotoSettings();

        private readonly ApplicationDbContext db = new ApplicationDbContext();
        readonly string[] _photoExtension = { ".jpg", ".jpe", ".jpeg", ".bmp", ".gif", ".png", ".ico", ".webp" };
        //readonly string[] _jpgExtension = { ".jpg", ".jpe", ".jpeg"};
        readonly string[] _webImageExtension = { ".jpg", ".jpe", ".jpeg", ".webp" };
        readonly string[] _docExtension = { ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".ppt", ".pptx", ".rar", ".zip" };
        private readonly string[] _videoExtension = {".mp4", ".webm"};

        private int large;
        private readonly int medium;
        private readonly int small;
        private readonly int imageQuality;
        private readonly int maxHeight;

        private bool onlyResizeIfWider = true;
        static string _projectId = WebConfigurationManager.AppSettings["ProjectId"];
        private string folder = $"/Uploads/{_projectId.Translate()}/Photo";


        public UploadController()
        {
            large = Convert.ToInt16(settingRepository.Get(PhotoSettingType.LargeSize));
            medium = Convert.ToInt16(settingRepository.Get(PhotoSettingType.MediumSize));
            small = Convert.ToInt16(settingRepository.Get(PhotoSettingType.SmallSize));
            imageQuality = Convert.ToInt16(settingRepository.Get(PhotoSettingType.ImageQuality));
            maxHeight = Convert.ToInt16(settingRepository.Get(PhotoSettingType.MaxHeight));
        }

        // POST: api/Upload
        public async Task<IHttpActionResult> PostFormData(string entity, int modelId)
        {
            var subFolder = entity;

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {

                Photo photo;

                #region upload

                // set subfolder
                if (subFolder != "unknown")
                    folder += "/" + subFolder;


                // Check path and auto create 
                // ReSharper disable once AssignNullToNotNullAttribute
                if (!Directory.Exists(HostingEnvironment.MapPath(folder)))
                    Directory.CreateDirectory(HostingEnvironment.MapPath(folder));

                // set no-limit for sub
                if (entity == cms4seoEntityType.Slider || entity == "banner")
                    large = 1920; // default to 2000 -> 1110


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

                // change extension webp to jpeg
                // this mean, just support webp upload, but publish as jpeg
                if (extension == ".webp")
                    extension = ".jpg";

                if (!_photoExtension.Contains(extension.ToLower()))
                {
                    // check support documents or video
                    if (!_docExtension.Contains(extension.ToLower()) 
                        && !_videoExtension.Contains(extension.ToLower()))
                    {
                        return StatusCode(HttpStatusCode.UnsupportedMediaType);
                    }

                    // is document
                    folder = _docExtension.Contains(extension.ToLower()) 
                        ? $"/Uploads/{_projectId.Translate()}/Documents" 
                        : $"/Uploads/{_projectId.Translate()}/Videos";


                    if (!Directory.Exists(HostingEnvironment.MapPath(folder)))
                        Directory.CreateDirectory(HostingEnvironment.MapPath(folder));


                    path = HttpContext.Current.Server.MapPath("~" + folder);

                    // prevent conflict file ========================================================                    
                    string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    fileName = String.Copy(nameWithoutExtension.MakeNameFriendly());

                    var n = 0;
                    while (File.Exists($@"{path}\{fileName}{extension}"))
                        fileName = $"{nameWithoutExtension}-{++n}";


                    // some stuff to save file
                    using (var fileStream = File.Create($@"{path}\{fileName}{extension}"))
                    {
                        //(await content.ReadAsStreamAsync()).Seek(0, SeekOrigin.Begin);
                        (await content.ReadAsStreamAsync()).CopyTo(fileStream);
                    }


                    // for display document
                    photo = new Photo()
                    {
                        Name = fileName,
                        SmPath = $"{folder}/{fileName}{extension}",
                        MdPath = $"{folder}/{fileName}{extension}",
                        LgPath = $"{folder}/{fileName}{extension}",
                        AltAttribute = fileName,
                        MimeType = extension,
                        PostBy = User.Identity.Name,
                        Entity = entity,
                        ModelId = modelId
                    };
                    db.Photos.Add(photo);
                    await db.SaveChangesAsync();

                }
                else
                {
                    
                    // pre init
                    string smName, mdName, lgName;

                    string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                    fileName = String.Copy(nameWithoutExtension.MakeNameFriendly());

                    if (_webImageExtension.Contains(extension.ToLower()))
                    {
                        // sync all to jpg to optimize mobile mode
                        extension = ".jpg";


                        // prevent conflict file =============================================
                        var n = 0;
                        while (File.Exists($@"{path}\{fileName}-sm{extension}"))
                            fileName = $"{nameWithoutExtension.MakeNameFriendly()}-{++n}";


                        smName = $"{fileName}-sm{extension}";
                        mdName = $"{fileName}-md{extension}";
                        lgName = $"{fileName}-lg{extension}";


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

                        // legacy mode
                        //var smallImage = Imager.Resize(bitmap, small, maxHeight, onlyResizeIfWider);
                        //var mediumImage = Imager.Resize(bitmap, medium, maxHeight, onlyResizeIfWider);
                        //var largeImage = Imager.Resize(bitmap, large, maxHeight, onlyResizeIfWider);

                        // Test mode
                        var smallImage = Imager.ResizeImage(bitmap, small, maxHeight, onlyResizeIfWider);
                        var mediumImage = Imager.ResizeImage(bitmap, medium, maxHeight, onlyResizeIfWider);
                        var largeImage = Imager.ResizeImage(bitmap, large, maxHeight, onlyResizeIfWider);


                        using (MozJpeg mozJpeg = new MozJpeg())
                        {
                            mozJpeg.Save(smallImage, $@"{path}\{smName}", imageQuality);
                                //default imageQuality 75 -> 85
                            mozJpeg.Save(mediumImage, $@"{path}\{mdName}", imageQuality);
                                //default imageQuality 75 -> 85
                            mozJpeg.Save(largeImage, $@"{path}\{lgName}", imageQuality);
                                //default imageQuality 75 -> 85
                        }


                        bitmap.Dispose();
                        smallImage.Dispose();
                        mediumImage.Dispose();
                        largeImage.Dispose();
                    }
                    else
                    {


                        var n = 0;
                        while (File.Exists($@"{path}\{fileName}{extension}"))
                            fileName = $"{nameWithoutExtension}-{++n}";


                        // some stuff to save file
                        using (var fileStream = File.Create($@"{path}\{fileName}{extension}"))
                        {                            
                            (await content.ReadAsStreamAsync()).CopyTo(fileStream);
                        }

                        // Image not jpg not be resize, thus not extent name sm, md, lg
                        smName = $"{fileName}{extension}";
                        mdName = $"{fileName}{extension}";
                        lgName = $"{fileName}{extension}";
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
                        Entity = entity,
                        ModelId = modelId
                    };

                    db.Photos.Add(photo);
                    await db.SaveChangesAsync();
                    #endregion database


                    
                }
                #endregion upload





                #region entity selector for photo

                if (photo.Entity == cms4seoEntityType.Category)
                {
                    var category = db.Categories.Find(photo.ModelId);
                    category?.Photos.Add(photo);
                }
                else if (photo.Entity == cms4seoEntityType.Product)
                {
                    var product = db.Products.Find(photo.ModelId);
                    product?.Photos.Add(photo);
                }
                else if (photo.Entity == cms4seoEntityType.Topic)
                {
                    var topic = db.Topics.Find(photo.ModelId);
                    topic?.Photos.Add(photo);
                }
                else if (photo.Entity == cms4seoEntityType.Article)
                {
                    var article = db.Articles.Find(photo.ModelId);
                    article?.Photos.Add(photo);
                }
                else if (photo.Entity == cms4seoEntityType.Info)
                {
                    var info = db.Infos.Find(photo.ModelId);
                    info?.Photos.Add(photo);
                }
                else if (photo.Entity == cms4seoEntityType.Slider)
                {
                    var slider = db.Sliders.Find(photo.ModelId);
                    slider?.Photos.Add(photo);
                }
                else if (photo.Entity == cms4seoEntityType.Settings)
                {
                    // not map
                }
                else if (photo.Entity == cms4seoEntityType.Contents)
                {
                    // not map
                }
                else
                {
                    // throw new exception
                    throw new Exception(
                        AdminResources.UploadControllerPostFormData_Not_found_entity__Please_add_Entity_in_UploadController_cs);
                }

                db.SaveChanges();


                #endregion


                // response
                return Ok();
            }
            catch (Exception e)
            {
                LogHelper.Write("/admin/api/Upload", $"Upload Fail, Message: {e.Message}");
                // client success response, but will recheck condition in client side
                return CreatedAtRoute("DefaultApi", null, new { success = false, message = e.Message });

            }
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




    }
}