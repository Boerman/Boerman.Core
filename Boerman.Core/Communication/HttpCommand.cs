using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Boerman.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Boerman.Core.Communication
{
    public class HttpCommand
    {
        public HttpCommand()
        {
            
        }

        public HttpCommand(string address)
        {
            Address = new Uri(address);
        }

        public HttpCommand(Uri address)
        {
            Address = address;
        }

        public string Accept { get; set; }
        public Uri Address { get; set; }
        public string Method { get; set; }
        public string UserAgent { get; set; }
        public string ContentType { get; set; }

        public string Content { get; set; }
        public ICollection<KeyValuePair<string, string>> Data { get; set; }
        public ICollection<KeyValuePair<string, string>> QueryString { get; set; }
        public WebHeaderCollection Headers { get; set; }


        public static HttpCommand CreateHttpCommand()
        {
            return new HttpCommand();
        }

        public HttpCommand SetAddress(Uri address)
        {
            Address = address;
            return this;
        }

        public HttpCommand SetAddress(string address)
        {
            SetAddress(new Uri(address));
            return this;
        }

        public HttpCommand AppendToAddress(string address)
        {
            Address = new Uri(Address.OriginalString + address);
            return this;
        }

        public HttpCommand SetMethod(string method)
        {
            Method = method;
            return this;
        }

        public HttpCommand SetAccept(string accept)
        {
            Accept = accept;
            return this;
        }

        public HttpCommand SetContent(string content)
        {
            Content = content;
            return this;
        }

        public HttpCommand AppendQueryString(string key, string value, bool raw = false)
        {
            if (QueryString == null) QueryString = new Collection<KeyValuePair<string, string>>();

            QueryString.Add(new KeyValuePair<string, string>(
                key,
                raw ? value
                    : HttpUtility.UrlEncode(value)));
            return this;
        }

        public HttpCommand AppendData(string key, string value, bool raw = false)
        {
            if (Data == null) Data = new Collection<KeyValuePair<string, string>>();

            Data.Add(new KeyValuePair<string, string>(
                key,
                raw ? value
                    : HttpUtility.UrlEncode(value)));
            return this;
        }

        public HttpCommand AppendData(string key, int value)
        {
            AppendData(key, value.ToString());
            return this;
        }

        public HttpCommand AppendData(string key, double value)
        {
            AppendData(key, value.ToString());
            return this;
        }

        public HttpCommand AppendHeader(string header, string value)
        {
            if (Headers == null) Headers = new WebHeaderCollection();

            Headers.Add(header, value);
            return this;
        }

        private async Task<WebResponse> BeginGetResponse()
        {
            var address = new Uri(Address.OriginalString.AddQueryParameters(QueryString));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.Headers = (WebHeaderCollection)Headers.OrDefault(new WebHeaderCollection());
            request.Method = Method;
            request.ContentType = ContentType.OrDefault("application/x-www-form-urlencoded");
            request.Accept = Accept.OrDefault("application/json");

            if (request.Method.ToUpper() == "POST"
                && (Data != null
                || !String.IsNullOrEmpty(Content)))
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var data = String.IsNullOrEmpty(Content)
                        ? new FormUrlEncodedContent(Data).ReadAsStringAsync().Result
                        : Content;

                    await streamWriter.WriteAsync(data);
                }
            }

            return await request.GetResponseAsync();
        }

        public async Task Execute()
        {
            await BeginGetResponse();
        }

        public async Task<Stream> GetStreamResponse()
        {
            WebResponse response = await BeginGetResponse();

            return response.GetResponseStream();
        }

        public async Task<string> GetStringResponse()
        {
            WebResponse response = await BeginGetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        public async Task<dynamic> GetDynamicResponse()
        {
            var data = await GetStringResponse();
            return await Task.Factory.StartNew(() => JObject.Parse(data));
        }

        public async Task<T> GetResponse<T>()
        {
            if (typeof(T) == typeof(Stream)) return (T)(object)await GetStreamResponse();

            // Assuming we're dealing with a json response.
            var data = await GetStringResponse();

            if (typeof(T) == typeof(String)) return (T)(object)data;

            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(data));
        }

        public async Task<string> GetResponse()
        {
            return await GetResponse<string>();
        }
    }
}
