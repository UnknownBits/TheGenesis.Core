﻿using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TheGenesis.Core.Utils
{
    public class Network : IDisposable
    {
        private HttpClient HttpClient;
        private bool disposedValue;

        public Network()
        {
            HttpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        public HttpResponseMessage HttpGet(string url, string content_type = "application/json", Dictionary<string, string> headerPairs = null)
            => HttpGetAsync(url, content_type, headerPairs).Result;

        public async Task<HttpResponseMessage> HttpGetAsync(string url, string content_type = "application/json", Dictionary<string, string> headerPairs = null)
        {
            using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);
            message.Content = new StringContent("");
            message.Content.Headers.ContentType = new MediaTypeHeaderValue(content_type);
            if (headerPairs != null)
                foreach (var pair in headerPairs) message.Headers.Add(pair.Key, pair.Value);
            return await HttpClient.SendAsync(message);
        }

        public HttpResponseMessage HttpPost(string url, string content, string content_type = "application/json", Dictionary<string, string> headerPairs = null)
            => HttpPostAsync(url, content, content_type, headerPairs).Result;
        public HttpResponseMessage HttpPost(string url, HttpContent content, Dictionary<string, string> headerPairs = null)
            => HttpPostAsync(url, content, headerPairs).Result;

        public async Task<HttpResponseMessage> HttpPostAsync(string url, string content, string content_type = "application/json", Dictionary<string, string> headerPairs = null)
        {
            using var strContent = new StringContent(content);
            strContent.Headers.ContentType = new MediaTypeHeaderValue(content_type);
            return await HttpPostAsync(url, strContent, headerPairs);
        }

        public async Task<HttpResponseMessage> HttpPostAsync(string url, HttpContent content, Dictionary<string, string> headerPairs = null)
        {
            using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url);
            message.Content = content;
            if (headerPairs != null)
                foreach (var pair in headerPairs) message.Headers.Add(pair.Key, pair.Value);
            return await HttpClient.SendAsync(message);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) HttpClient?.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
