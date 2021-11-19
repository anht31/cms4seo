using System.Linq;
using System.Web.Mvc;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Service.Email;
using cms4seo.Service.Resolver;

namespace cms4seo.Plugins.ShoppingCart.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();


        public ViewResult Index(string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                ReturnUrl = returnUrl,

                // binding
                Cart = GetCart()
            });
        }

        
        public ActionResult TableCartDetail()
        {
            return PartialView("_TableCartDetail",
                new CartIndexViewModel
                {
                    // binding
                    Cart = GetCart()
                });
        }


        public JsonResult AddToCart(int productId, int quality, string returnUrl)
        {
            var cart = GetCart();
            var product = db.Products
                .FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                // binding
                cart.AddItem(product, quality);
            }

            var qualityTotal = cart.Lines.Sum(x => x.Quantity);


            return Json(new
            {
                success = true,
                qualityTotal,
                responseText = "Đã thêm sản phẩm thành công."
            },
                JsonRequestBehavior.AllowGet);

            //return RedirectToAction("Index", new { returnUrl });
            //return RedirectToAction("Index", new {returnUrl});
        }


        public JsonResult UpdateCart(int productId, int quality)
        {
            var cart = GetCart();

            var product = db.Products
                .FirstOrDefault(p => p.Id == productId);

            var productQuality = cart.Lines.FirstOrDefault(x => x.Product.Id == productId)?.Quantity;

            int qualityTotal = cart.Lines.Sum(x => x.Quantity);
            int? lineQuality = 0;

            if (product != null)
            {
                if (productQuality == 1 && quality == -1)
                {
                    cart.RemoveLine(product);
                    return Json(new
                    {
                        success = false,
                        qualityTotal = qualityTotal - 1,
                        lineQuality,
                        responseText = "Đã cập nhật giỏ hàng thành công."
                    },
                    JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // binding
                    cart.AddItem(product, quality);
                }

            }


            lineQuality = cart.Lines.FirstOrDefault(x => x.Product.Id == productId)?.Quantity;

            return Json(new
            {
                success = true,
                qualityTotal,
                lineQuality,
                responseText = "Đã cập nhật giỏ hàng thành công."
            },
                JsonRequestBehavior.AllowGet);

            //return RedirectToAction("Index", new { returnUrl });
            //return RedirectToAction("Index", new {returnUrl});
        }


        public JsonResult RemoveFromCart(int productId, string returnUrl)
        {
            var cart = GetCart();

            var product = db.Products
                .FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                // binding
                cart.RemoveLine(product);

                //GetCart().RemoveLine(product);
            }

            int qualityTotal = cart.Lines.Sum(x => x.Quantity);

            return Json(new
            {
                success = false,
                qualityTotal,
                lineQuality = 0,
                responseText = "Đã cập nhật giỏ hàng thành công."
            },
                    JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", new { returnUrl });
        }

        private Cart GetCart()
        {
            var cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }


        public PartialViewResult Summary()
        {
            return PartialView(GetCart());
        }

        
        // check out ========================================================

        [HttpGet]
        public ViewResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(ShippingDetails shippingDetails)
        {

            var cart = GetCart();

            IOrderProcessor orderProcessor = new EmailProcessor();

            if (!cart.Lines.Any())
            {
                // no need
                //ModelState.AddModelError("", "Sorry,  your  cart  is empty!");

                // although cart is empty, just get customer info.
                orderProcessor.ProcessContact(shippingDetails.Name, shippingDetails.Email, shippingDetails.Phone,
                    "Khách hàng chưa thêm sản phẩm vào giỏ hàng (hoặc đã quá thời gian giao dịch). Địa chỉ: "
                    + shippingDetails.Address
                    );

                // virtual path to completed page like shopping-cart-completed
                return RedirectToAction("Completed");
            }
            if (ModelState.IsValid)
            {
                orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();

                // virtual path to completed page like shopping-cart-completed
                return RedirectToAction("Completed");

            }

            return View(shippingDetails);

        }


        
        public ActionResult Completed()
        {
            return View();
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
