namespace FeedsApi.Controllers
{
    public static class HttpContextExtensions
    {
        public static int GetLoggedInUserId(this HttpContext context) 
        {
            return int.Parse(context.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);
        }
    }
}
