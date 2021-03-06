﻿using Microsoft.Owin;

using System.IO;
using System.Threading.Tasks;

namespace Owin
{
    /// <summary>
    /// Due to the way that Nancy handles self-hosting environments, this forces uploaded files to be prematurely read into memory before continuing the owin pipeline execution.
    /// </summary>
    public class BufferContentIntoAMemoryStreamMiddleware : OwinMiddleware
    {
        public BufferContentIntoAMemoryStreamMiddleware(OwinMiddleware next) : base(next) { }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task Invoke(IOwinContext context)
        {
            // DIRTY HACK TO GET FILES TO UPLOAD!  This is only needed if you need to use client certificates.
            // need to find out why client certificates seem to bork the pipeline.
            var bytes = ReadFully(context.Request.Body);

            context.Request.Body = new MemoryStream(bytes);
            context.Request.Body.Seek(0, 0);

            return Next.Invoke(context);
        }

        static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
