using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Birpors.API.Helpers.ActionResults
{

    public class UnAuthResult
    {
        public string Output { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public Dictionary<string, string> ErrorList { get; set; }
    }
    public class UnAuthActionResult : IActionResult
    {
        public async Task ExecuteResultAsync(ActionContext context)
        {

            var result = new UnAuthResult
            {
               Output = null,
               Status = (int)HttpStatusCode.Unauthorized,
               StatusText  = "Bu sorğunun icrası üçün hüququnuz yoxdur. (Authorization error)",
               ErrorList = null
            };

            var objectResult = new ObjectResult(result);
            await objectResult.ExecuteResultAsync(context);
        }
    }

}
