using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace Demo.App.Controllers
{
    public class HomeController : BaseController
    {
        public IHttpResponse Home(IHttpRequest httpRequest)
        {
            this.HttpRequest = httpRequest;
            
            var sessionId = this.HttpRequest.Session;
            return this.View();
        }

        public IHttpResponse Login(IHttpRequest httpRequest)
        {
            HttpRequest.Session.AddParameter("username", "Stanoicho");

            return this.Redirect("/");
        }
    }
}
