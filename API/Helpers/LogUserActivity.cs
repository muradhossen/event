using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();
            if (userId <= 0) return;

            var repo = resultContext.HttpContext.RequestServices.GetService<IUserReposetory>();

            var user = await repo.GetUserByIdAsync(userId);
            if (user == null) return;

            user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();

        }
    }
}
