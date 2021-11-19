using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Admin.Controllers;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Service.Email;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace cms4seo.Admin.Controllers
{
    //[Authorize]
    [Authorize]
    public class ManageController : BaseController
    {
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        //
        // GET: /Account/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess
                    ? AdminResources.ManageControllerIndex_Your_password_has_been_changed_
                    : message == ManageMessageId.SetPasswordSuccess
                        ? AdminResources.ManageControllerIndex_Your_password_has_been_set_
                        : message == ManageMessageId.SetTwoFactorSuccess
                            ? AdminResources.ManageControllerIndex_Your_two_factor_provider_has_been_set_
                            : message == ManageMessageId.Error
                                ? AdminResources.ManageControllerIndex_An_error_has_occurred_
                                : message == ManageMessageId.AddPhoneSuccess
                                    ? AdminResources.ManageControllerIndex_The_phone_number_was_added_
                                    : message == ManageMessageId.RemovePhoneSuccess
                                        ? AdminResources.ManageControllerIndex_Your_phone_number_was_removed_
                                        : "";

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(User.Identity.GetUserId()),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId()),
                Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId()),
                BrowserRemembered =
                    await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId())
            };

            ViewBag.EmailConfirmed = await UserManager.IsEmailConfirmedAsync(User.Identity.GetUserId());

            return View(model);
        }

        //
        // GET: /Account/RemoveLogin
        public ActionResult RemoveLogin()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return View(linkedAccounts);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result =
                await
                    UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                        new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new {Message = message});
        }

        //
        // GET: /Account/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Account/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = AdminResources.ManageControllerAddPhoneNumber_Your_security_code_is__ + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new {PhoneNumber = model.Number});
        }

        //
        // POST: /Manage/RememberBrowser
        [HttpPost]
        public ActionResult RememberBrowser()
        {
            var rememberBrowserIdentity =
                AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(User.Identity.GetUserId());
            AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = true}, rememberBrowserIdentity);
            //return RedirectToAction("Index", "Manage", new { area = "Admin" });
            return RedirectToAction("Index");
        }

        //
        // POST: /Manage/ForgetBrowser
        [HttpPost]
        public ActionResult ForgetBrowser()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            return RedirectToAction("Index", "Manage", new { area = "Admin" });
        }

        //
        // POST: /Manage/EnableTFA
        [HttpPost]
        public async Task<ActionResult> EnableTFA()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, false);
            }
            return RedirectToAction("Index", "Manage", new { area = "Admin" });
        }

        //
        // POST: /Manage/DisableTFA
        [HttpPost]
        public async Task<ActionResult> DisableTFA()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, false);
            }
            return RedirectToAction("Index", "Manage", new { area = "Admin" });
        }

        //
        // GET: /Account/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            // This code allows you exercise the flow without actually sending codes
            // For production use please register a SMS provider in IdentityConfig and generate a code here.
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            ViewBag.Status = 
                AdminResources.ManageControllerVerifyPhoneNumber_For_DEMO_purposes_only__the_current_code_is_ + code;
            return phoneNumber == null
                ? View("Error")
                : View(new VerifyPhoneNumberViewModel {PhoneNumber = phoneNumber});
        }

        //
        // POST: /Account/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result =
                await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, false);
                }
                return RedirectToAction("Index", new {Message = ManageMessageId.AddPhoneSuccess});
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", AdminResources.ManageControllerVerifyPhoneNumberFailedToVerifyPhone);
            return View(model);
        }

        //
        // GET: /Account/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new {Message = ManageMessageId.Error});
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, false);
            }
            return RedirectToAction("Index", new {Message = ManageMessageId.RemovePhoneSuccess});
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result =
                await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, false);
                }
                return RedirectToAction("Index", new {Message = ManageMessageId.ChangePasswordSuccess});
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInAsync(user, false);
                    }
                    return RedirectToAction("Index", new {Message = ManageMessageId.SetPasswordSuccess});
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Manage
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess
                    ? AdminResources.ManageControllerManageLogins_The_external_login_was_removed_
                    : message == ManageMessageId.Error
                        ? AdminResources.ManageControllerManageLogins_An_error_has_occurred_
                        : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins =
                AuthenticationManager.GetExternalAuthenticationTypes()
                    .Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider))
                    .ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"),
                User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new {Message = ManageMessageId.Error});
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded
                ? RedirectToAction("ManageLogins")
                : RedirectToAction("ManageLogins", new {Message = ManageMessageId.Error});
        }


        public async Task<ActionResult> VerifyEmail()
        {
            var email = await UserManager.GetEmailAsync(User.Identity.GetUserId());

            var code = await UserManager.GenerateEmailConfirmationTokenAsync(User.Identity.GetUserId());

            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = User.Identity.GetUserId(), code },
                Request.Url.Scheme);

            try
            {
                var emailService = new EmailProcessor();
                emailService.SendMail(email,
                    AdminResources.AccountControllerRegister_Confirm_your_account,
                    AdminResources.AccountControllerRegister_Please_confirm_your_account_by_clicking_this_link__ +
                    callbackUrl);
            }
            catch (System.Exception exception)
            {
                TempData[MessageType.Danger] =
                    string.Format(AdminResources.AccountController_Register_MaybeYouNotHaveBeenSetSMTPEmail, exception);
            }

            TempData[MessageType.Info] = AdminResources.ManageController_VerifyEmail_PleaseCheckYourEmailAndConfirmYourEmailAddress;

            return RedirectToAction("Index");
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie,
                DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent},
                await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}