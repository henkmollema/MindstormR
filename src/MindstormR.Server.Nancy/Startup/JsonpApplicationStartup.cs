﻿using System;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.Json;
using Nancy.Bootstrapper;

namespace MindstormR.Client.Nancy
{
    /// <summary>
    /// Enables JSONP support at application startup.
    /// </summary>
    public class JsonpApplicationStartup : IApplicationStartup
    {
        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
            Jsonp2.Enable(pipelines);
        }
    }

    public static class Jsonp2
    {
        static PipelineItem<Action<NancyContext>> JsonpItem;

        static Jsonp2()
        {
            JsonpItem = new PipelineItem<Action<NancyContext>>("JSONP", PrepareJsonp);
        }

        /// <summary>
        /// Enable JSONP support in the application
        /// </summary>
        /// <param name="pipelines">Application Pipeline to Hook into</param>
        public static void Enable(IPipelines pipelines)
        {
            bool jsonpEnabled = pipelines.AfterRequest.PipelineItems.Any(ctx => ctx.Name == "JSONP");

            if (!jsonpEnabled)
            {
                pipelines.AfterRequest.AddItemToEndOfPipeline(JsonpItem);
            }
        }

        /// <summary>
        /// Disable JSONP support in the application
        /// </summary>
        /// <param name="pipelines">Application Pipeline to Hook into</param>
        public static void Disable(IPipelines pipelines)
        {
            pipelines.AfterRequest.RemoveByName("JSONP");
        }

        /// <summary>
        /// Transmogrify original response and apply JSONP Padding
        /// </summary>
        /// <param name="context">Current Nancy Context</param>
        private static void PrepareJsonp(NancyContext context)
        {
            bool hasCallback = context.Request.Query["callback"].HasValue;

            if (hasCallback)
            {
                // grab original contents for running later
                Action<Stream> original = context.Response.Contents;
                string callback = context.Request.Query["callback"].Value;

                // set content type to application/javascript so browsers can handle it by default
                // http://stackoverflow.com/questions/111302/best-content-type-to-serve-jsonp
                context.Response.ContentType = "application/javascript" + (String.IsNullOrWhiteSpace(JsonSettings.DefaultCharset) ? "" : "; charset=" + JsonSettings.DefaultCharset);
                context.Response.Contents = stream =>
                {
                    // disposing of stream is handled elsewhere
                    StreamWriter writer = new StreamWriter(stream)
                    {
                        AutoFlush = true
                    };

                    writer.Write("{0}(", callback);
                    original(stream);
                    writer.Write(");");
                };
            }
        }
    }
}

