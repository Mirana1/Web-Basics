namespace SIS.HTTP.Cookies
{
    using SIS.HTTP.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public class HttpCookieCollection : IHttpCookieCollection
    {
        private Dictionary<string, HttpCookie> httpCookies;

        public HttpCookieCollection()
        {
            this.httpCookies = new Dictionary<string, HttpCookie>();
        }

        public void AddCookie(HttpCookie cookie)
        {
            CoreValidator.ThrowIfNull(cookie, nameof(cookie));

            this.httpCookies.Add(cookie.Key, cookie);
        }

        public bool ContainsCookie(string key)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));

            return this.httpCookies.ContainsKey(key);
        }

        public HttpCookie GetCookie(string key)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));

            return this.httpCookies[key];
        }

       

        public bool HasCookie()
        {
            return this.httpCookies.Count != 0;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.httpCookies.Values.GetEnumerator();
        }

        public IEnumerator<HttpCookie> GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var cookie in this.httpCookies.Values)
            {
                sb.Append($"Set-Cookie: {cookie}")
                    .Append(GlobalConstants.HttpNewLine);
            }

            return sb.ToString();
        }

    }
}
