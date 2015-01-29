using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BurritoBarn.Models;
using System.Security.Cryptography;
using System.Text;

namespace BurritoBarn.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private BurritoBarnContext db = new BurritoBarnContext();

		public AccountController()
		{
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, change to shouldLockout: true
			var result = PasswordSign(model.Email, model.Password);
			switch (result)
			{
				case SignInStatus.Success:
					FormsAuthentication.SetAuthCookie(model.Email, true);
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid login attempt.");
					return View(model);
			}
		}

	   
		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var employee = new Employee { 
					emailAddress = model.Email,
					password = GetHashString(model.Password),
					phoneNumber = model.PhoneNumber,
					firstName = model.FirstName,
					lastName = model.LastName
				};
				db.Employees.Add(employee);
				db.SaveChanges();
					
				//take em to a screen that lets them know they have to wait
				return RedirectToAction("Awaiting", "Home");
				
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

	   
	 
		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}



		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}


	 
		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}

		private SignInStatus PasswordSign(string userName, string password)
		{
			var user = db.Employees.Where(e => e.emailAddress == userName).FirstOrDefault();
			if (user == null)
			{
				return SignInStatus.Failure;
			}
			if (user.password == GetHashString(password) && user.isActive.HasValue && user.isActive.Value)
			{
				return SignInStatus.Success;
			}

			return SignInStatus.Failure;
		}

		private static byte[] GetHash(string inputString)
		{
			var algorithm = MD5.Create();
			return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
		}

		private static string GetHashString(string inputString)
		{
			var sb = new StringBuilder();
			foreach (byte b in GetHash(inputString))
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}
		#endregion
	}
}