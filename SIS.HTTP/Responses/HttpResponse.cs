using System;
using System.Collections.Generic;
using System.Text;
using SIS.HTTP.Common;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;

namespace SIS.HTTP.Responses
{
    public class HttpResponse : IHttpResponse
    {
        public HttpResponse()
        {
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
            this.Content = new byte[0];

        }

        public HttpResponse(HttpResponseStatusCode statusCode) : this()
        {
            CoreValidator.ThrowIfNull(statusCode, nameof(statusCode));
            this.StatusCode = statusCode;
        }

        public HttpResponseStatusCode StatusCode { get; set; }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public byte[] Content { get; set; }

        public void AddHeader(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(header));
            this.Headers.AddHeader(header);
        }

        public void AddCookie(HttpCookie cookie)
        {
            //REMOVE?
            CoreValidator.ThrowIfNull(cookie, nameof(cookie));
            this.Cookies.AddCookie(cookie);
        }

        public byte[] GetBytes()
        {
            byte[] httpResponseBytesWithoutBody = Encoding.ASCII.GetBytes(this.ToString());

            byte[] httpResponseByteWithBody = new byte[httpResponseBytesWithoutBody.Length + this.Content.Length];

            for (int i = 0; i < httpResponseBytesWithoutBody.Length; i++)
            {
                httpResponseByteWithBody[i] = httpResponseBytesWithoutBody[i];
            }

            for (int i = 0; i < httpResponseByteWithBody.Length - httpResponseBytesWithoutBody.Length; i++)
            {
                httpResponseByteWithBody[i + httpResponseBytesWithoutBody.Length] = this.Content[i];
            }

            return httpResponseByteWithBody;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var intStatusCode = (int)this.StatusCode;
            var stringStatusCode = this.StatusCode.ToString();
            sb
                .Append($"{GlobalConstants.HttpOneProtocolFragment} {intStatusCode} {stringStatusCode}")
                .Append(GlobalConstants.HttpNewLine)
                .Append($"{this.Headers}")
                .Append(GlobalConstants.HttpNewLine);

            if (this.Cookies.HasCookie())
            {
                sb.Append($"{this.Cookies}").Append(GlobalConstants.HttpNewLine);
            }

            sb.Append(GlobalConstants.HttpNewLine);

            return sb.ToString();
        }
    }
}
