/*
' Copyright (c) 2023 AndrasPali
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;
using XpressKuponXpressKupon.Components;
using XpressKuponXpressKupon.Models;

namespace XpressKuponXpressKupon.Controllers
{
	[DnnHandleError]
	public class ItemController : DnnController
	{

		public ActionResult Delete(int itemId)
		{
			ItemManager.Instance.DeleteItem(itemId, ModuleContext.ModuleId);
			return RedirectToDefaultRoute();
		}

		public ActionResult Edit(int itemId = -1)
		{
			DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);

			var userlist = UserController.GetUsers(PortalSettings.PortalId);
			var users = from user in userlist.Cast<UserInfo>().ToList()
						select new SelectListItem { Text = user.DisplayName, Value = user.UserID.ToString() };

			ViewBag.Users = users;

			var item = (itemId == -1)
				 ? new Item { ModuleId = ModuleContext.ModuleId }
				 : ItemManager.Instance.GetItem(itemId);

			return View(item);
		}

		[HttpPost]
		[DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
		public ActionResult Edit(Item item)
		{
			if (item.GiftCardId == -1)
			{
				item.IssueDateUtc = DateTime.UtcNow;

				ItemManager.Instance.CreateItem(item);
			}
			else
			{
				var existingItem = ItemManager.Instance.GetItem(item.GiftCardId);				
				existingItem.CardNumber = item.CardNumber;
				existingItem.Amount = item.Amount;
				existingItem.UsedAmount = item.UsedAmount;
				existingItem.ExpirationDateUtc = item.ExpirationDateUtc;
				existingItem.RecipientName = item.RecipientName;
				existingItem.Enabled = item.Enabled;				

				ItemManager.Instance.UpdateItem(existingItem);
			}

			return RedirectToDefaultRoute();
		}

		//[ModuleAction(ControlKey = "Edit", TitleKey = "AddItem")]
		//public ActionResult Index()
		//{
		//	var items = ItemManager.Instance.GetItems();
		//	return View(items);
		//}
		[ModuleAction(ControlKey = "Edit", TitleKey = "AddItem")]
		public ActionResult Index()
		{
			var items = ItemManager.Instance.GetItems();
			return View(items);
		}
	}
}
