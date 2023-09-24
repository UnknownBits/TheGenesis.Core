using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;
using System.IO;

namespace TheGenesis.Core.Utils
{
    public static class Network
    {
        public static readonly HttpClient HttpClient = new HttpClient();

        public static async Task<HttpResponseMessage> HttpGetAsync(string url, string content_type = "application/json", Dictionary<string, string>? headers = null)
        {
            using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);
            message.Content = new StringContent("");
            message.Content.Headers.ContentType = new MediaTypeHeaderValue(content_type);
            if (headers != null)
                foreach (var pair in headers) message.Headers.Add(pair.Key, pair.Value);
            return await HttpClient.SendAsync(message);
        }

        public static HttpResponseMessage HttpGet(string url, string content_type = "application/json", Dictionary<string, string>? headers = null)
            => HttpGetAsync(url, content_type, headers).Result;

        public static async Task<HttpResponseMessage> HttpPostAsync(string url, HttpContent content, Dictionary<string, string>? headers = null)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            if (headers != null) foreach (var pair in headers) message.Headers.Add(pair.Key, pair.Value);
            return await HttpClient.SendAsync(message);
        }

        public static async Task<HttpResponseMessage> HttpPostAsync(string url, string content, Dictionary<string, string>? headers = null, string content_type = "application/json")
        {
            HttpContent strContent = new StringContent(content);
            strContent.Headers.ContentType = new MediaTypeHeaderValue(content_type);
            return await HttpPostAsync(url, strContent, headers);
        }

        public static HttpResponseMessage HttpPost(string url, HttpContent content, Dictionary<string, string>? headers = null)
            => HttpPostAsync(url, content, headers).Result;

        public static HttpResponseMessage HttpPost(string url, string content, string content_type = "application/json", Dictionary<string, string>? headers = null) 
            => HttpPostAsync(url, content, headers, content_type).Result;

        public static string ReadAsString(this HttpContent content)
        {
            using var stream = content.ReadAsStreamAsync().Result;
            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
    }
}
