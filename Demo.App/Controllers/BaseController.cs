namespace Demo.App.Controllers
{
    using System.IO;
    using System.Runtime.CompilerServices;

    using SIS.HTTP.Cookies;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;
    using SIS.WebServer.Results;

    public abstract class BaseController
    {
        protected IHttpRequest HttpRequest { get; set; }
        private bool IsLoggedIn()
        {
            return this.HttpRequest.Session.ContainsParameter("username");
        }

        private string ParseTemplate(string viewContent)
        {
            if (this.IsLoggedIn())
            {
                return viewContent.Replace("@Model.HelloMessage", $"You've logged in, {this.HttpRequest.Session.GetParameter("username")}!");
            }
            else
            {
                return viewContent.Replace("@Model.HelloMessage", "Hello from the other side!");
            }

        }

        public IHttpResponse View([CallerMemberName] string view = null)
        {
            string controllerName = this.GetType().Name.Replace("Controller", string.Empty);
            string viewName = view;

            string viewContent = File.ReadAllText("Views/" + controllerName + "/" + viewName + ".html");

            viewContent = this.ParseTemplate(viewContent);

            var htmlResult = new HtmlResult(viewContent, SIS.HTTP.Enums.HttpResponseStatusCode.Ok);
            htmlResult.Cookies.AddCookie(new HttpCookie("lang", "en"));

            return htmlResult;
        }
        public IHttpResponse Redirect(string url)
        {
            return new RedirectResult(url);
        }
    }
}
