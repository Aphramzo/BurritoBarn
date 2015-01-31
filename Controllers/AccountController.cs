using System;
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

		public ActionResult ApproveUsers()
		{
			var unapprovedUsers = db.Employees.Where(e => !e.isActive.HasValue).ToList();
			return View(new ApproveUsersViewModel
			{
				Employees = unapprovedUsers
			});
		}

		[HttpPost]
		public JsonResult ApproveUser(string employeeId)
		{
			return UpdateEmployeeActive(employeeId, true);
		}

		[HttpPost]
		public JsonResult DisapproveUser(string employeeId)
		{
			return UpdateEmployeeActive(employeeId, false);
		}

		private JsonResult UpdateEmployeeActive(string employeeId, bool active)
		{
			var employeeIdInt = Convert.ToInt64(employeeId);
			var user = db.Employees.FirstOrDefault(e => e.id == employeeIdInt);
			if (user != null)
			{
				user.isActive = active;
				db.SaveChanges();
				return Json(true);
			}

			return Json(false);
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
					FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
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
				try
				{
					db.Employees.Add(employee);
					db.SaveChanges();
				}
				catch (Exception)
				{
					ModelState.AddModelError("", "Unable to register with that email address.");
					return View(model);
				}
					
				//take em to a screen that lets them know they have to wait
				return RedirectToAction("Awaiting", "Home");
				
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}
	 
		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			FormsAuthentication.SignOut();
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

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		private SignInStatus PasswordSign(string userName, string password)
		{
			var user = db.Employees.FirstOrDefault(e => e.emailAddress == userName);
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