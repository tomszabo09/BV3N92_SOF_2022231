using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Backend.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Backend.Helpers
{
	public class EditSiteUserModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			EditSiteUser user = new EditSiteUser();
			user.FirstName = bindingContext.ValueProvider.GetValue("firstname").FirstValue;
			user.Age = int.Parse(bindingContext.ValueProvider.GetValue("age").FirstValue);
			user.Bio = bindingContext.ValueProvider.GetValue("bio").FirstValue;
			user.Gender = (Gender)int.Parse(bindingContext.ValueProvider.GetValue("gender").FirstValue);
			user.Orientation = (Orientation)int.Parse(bindingContext.ValueProvider.GetValue("orientation").FirstValue);

			if (bindingContext.HttpContext.Request.Form.Files.Count > 0)
			{
				user.ProfilePicture = bindingContext.HttpContext.Request.Form.Files[0];
				for (int i = 1; i < bindingContext.HttpContext.Request.Form.Files.Count; i++)
				{
					user.UserPictures.Add(bindingContext.HttpContext.Request.Form.Files[i]);
				}
			}
			bindingContext.Result = ModelBindingResult.Success(user);
			return Task.CompletedTask;
		}
	}
}
