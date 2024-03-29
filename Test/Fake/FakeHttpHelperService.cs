﻿using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using LogicLayer.Helpers;
using Moq;

namespace Tests.Helpers
{
    public class FakeHttpHelperService : Mock<IHttpHelperService>
    {
        public FakeHttpHelperService GetStringContentReturnValue(string param, string value)
        {
            Setup(x => x.GetStringContent(param))
                .Returns(Task.FromResult(value))
                .Verifiable();

            return this;
        }
        
        public FakeHttpHelperService GetImageContent(string param, Image value)
        {
            Setup(x => x.GetImageContent(param))
                .Returns(Task.FromResult(value))
                .Verifiable();

            return this;
        }

        public FakeHttpHelperService GetStringContentFromFile(string param, string path)
        {
            string feed_data = null;
            using (StreamReader r = new StreamReader(path))
            {
                feed_data = r.ReadToEnd();
            }

            Setup(x => x.GetStringContent(param))
                .Returns(Task.FromResult(feed_data))
                .Verifiable();

            return this;
        }

        public FakeHttpHelperService GetRssHttpResponse(string param, HttpHelperService.HttpCallResponse value)
        {
            Setup(x => x.GetRssHttpResponse(param))
                .Returns(Task.FromResult(value))
                .Verifiable();

            return this;
        }

        public FakeHttpHelperService GetRssHttpResponseFromFile(string param, string requestUri, string contentPath)
        {
            string feed_data = null;
            using (StreamReader r = new StreamReader(contentPath))
            {
                feed_data = r.ReadToEnd();
            }

            var result = new HttpHelperService.HttpCallResponse();
            result.Content = feed_data;
            result.RequestUrl = requestUri;

            Setup(x => x.GetRssHttpResponse(param))
                .Returns(Task.FromResult(result))
                .Verifiable();

            return this;
        }
    }
}
