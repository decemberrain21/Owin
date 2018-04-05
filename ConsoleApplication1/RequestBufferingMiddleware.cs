using Microsoft.Owin;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class RequestBufferingMiddleware : OwinMiddleware
    {
        public RequestBufferingMiddleware(OwinMiddleware next)
            : base(next)
        {
        }
        
        public override Task Invoke(IOwinContext context)
        {
            var requestStream = context.Request.Body;
            var requestMemoryBuffer = new MemoryStream();
            requestStream.CopyTo(requestMemoryBuffer);
            requestMemoryBuffer.Seek(0, SeekOrigin.Begin);

            context.Request.Body = requestMemoryBuffer;

            return Next.Invoke(context);
        }
    }
}
