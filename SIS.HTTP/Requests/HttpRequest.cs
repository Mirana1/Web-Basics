using System;
using System.Collections.Generic;
using System.Linq;
using SIS.HTTP.Common;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Exceptions;
using SIS.HTTP.Headers;
using SIS.HTTP.Sessions;

namespace SIS.HTTP.Requests
{
    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestString)
        {
            CoreValidator.ThrowIfNullOrEmpty(requestString, nameof(requestString));

            this.FormData = new Dictionary<string, object>();
            this.QueryData = new Dictionary<string, object>();
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();

            this.ParseRequest(requestString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        public IHttpSession Session { get; set; }

        private bool IsValidRequestLine(string[] requestLines)
        {
            if (requestLines.Length != 3 || requestLines[2] != GlobalConstants.HttpOneProtocolFragment)
            {
                return false;
            }
            return true;
        }

        private bool IsValidRequestQueryString(string queryString, string[] queryParameters)
        {
            CoreValidator.ThrowIfNullOrEmpty(queryString, nameof(queryString));

            return true;
        }

        private bool HasQueryString()
        {
            return this.Url.Split('?').Length > 1;
        }

        private IEnumerable<string> GetPlainRequestHeaders(string[] requestLines)
        {
            for (int i = 1; i < requestLines.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(requestLines[i]))
                {
                    yield return requestLines[i];
                }
            }
        }

        private void ParseRequestQueryParameters()
        {
            if (this.HasQueryString())
            {
                this.Url.Split(new[] { '?', '#' })[1]
                       .Split('&')
                       .Select(plainQueryParameter => plainQueryParameter.Split('='))
                       .ToList()
                       .ForEach(queryParameterKeyValuePair =>
                           this.QueryData.Add(queryParameterKeyValuePair[0], queryParameterKeyValuePair[1]));
            }

        }

        private void ParseRequestFormDataParameters(string requestBody)
        {
            if (!string.IsNullOrEmpty(requestBody))
            {
                requestBody
                    .Split('&')
                    .Select(queryParameter => queryParameter.Split('='))
                    .ToList()
                    .ForEach(parsedQueryParameterKVP => this.FormData.Add(parsedQueryParameterKVP[0], parsedQueryParameterKVP[1]));
            }

        }

        private void ParseRequestMethod(string[] requestLineParams)
        {
            HttpRequestMethod method;
            bool result = HttpRequestMethod.TryParse(requestLineParams[0], true, out method);

            if (!result)
            {
                throw new BadRequestException(string.Format(GlobalConstants.UnsupportedHttpMethodExceptionMssg, requestLineParams[0]));
            }

            this.RequestMethod = method;
        }

        private void ParseRequestUrl(string[] requestLineParams)
        {
            this.Url = requestLineParams[1];
        }

        private void ParseRequestPath()
        {
            this.Path = this.Url.Split('?')[0];
        }

        private void ParseRequestHeaders(string[] unparsedHeaders)
        {
            unparsedHeaders.Select(plainHeader => plainHeader.Split(new[] { ':', ' ' }
                , StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(headerKeyValuePair => this.Headers.AddHeader(new HttpHeader(headerKeyValuePair[0], headerKeyValuePair[1])));
        }

        private void ParseRequestParameters(string requestBody)
        {
            this.ParseRequestQueryParameters();
            this.ParseRequestFormDataParameters(requestBody);
        }

        private void ParseCookies()
        {
            if (this.Headers.ContainsHeader(HttpHeader.Cookie))
            {
                string value = this.Headers.GetHeader(HttpHeader.Cookie).Value;

                string[] unparsedCookies = value.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string unparsedCookie in unparsedCookies)
                {
                    string[] cookieKVP = unparsedCookie.Split(new[] { '=' }, 2);

                    HttpCookie httpCookie = new HttpCookie(cookieKVP[0], cookieKVP[1], false);

                    this.Cookies.AddCookie(httpCookie);
                }
            }
        }

        private void ParseRequest(string requestString)
        {
            string[] splitRqstString = requestString.Split(new[] { GlobalConstants.HttpNewLine }, System.StringSplitOptions.None);

            string[] requestLine = splitRqstString[0].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (!this.IsValidRequestLine(requestLine))
            {
                throw new BadRequestException();
            }

            this.ParseRequestMethod(requestLine);
            this.ParseRequestUrl(requestLine);
            this.ParseRequestPath();

            this.ParseRequestHeaders(this.GetPlainRequestHeaders(splitRqstString).ToArray());
            this.ParseCookies();

            this.ParseRequestParameters(splitRqstString[splitRqstString.Length - 1]);
        }
    }
}