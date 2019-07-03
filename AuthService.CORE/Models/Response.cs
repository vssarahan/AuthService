using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.CORE.Models
{
    public class Response<TData>
    {
        public int Code { get; set; } = 200;

        public TData Data { get; set; } = default(TData);

        public string Error { get; set; } = null;

        public Response(TData item)
        {
            Data = item;
            Code = 200;
        }

        public Response(int code, string error)
        {
            Code = code;
            Error = error;
        }

        public bool Succeeded()
        {
            return Code == 200 && Error == null;
        }
    }
}
