using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Birpors.Application.CommonModels
{
    public  class ApiResult<TResponse> 
    {
        public  TResponse Output { get; set; }
        public  int Status { get; set; }
        public  string StatusText { get; set; }
        public  Dictionary<string,string> ErrorList { get; set; }


        public static ApiResult<TResponse> OK(TResponse response)
        {
            return new ApiResult<TResponse>
            {
                Output = response,
                Status = (int)HttpStatusCode.OK,
                StatusText = "Əməliyyat uğurla başa çatdı",
                ErrorList = null
            };
        }
        public static ApiResult<TResponse> Error(Dictionary<string,string> errors,int status,string statusText)
        {
            return new ApiResult<TResponse>
            {
                Output = default,
                ErrorList = errors,
                Status = status,
                StatusText = statusText
            };
        }
    }
}
