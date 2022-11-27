using System;

namespace API_Final.Models
{
    public class Response<T>
    {
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public T? Data { get; set; }

        public Response(int c, string d, T data)
        {
            StatusCode = c;
            StatusDescription = d;
            Data = data;
        }
    }
}
