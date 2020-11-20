// David Wahid
using System;
namespace mobile.Models.HttpResponse
{
    public class Response<T>
    {
        public int StatusCode { get; set; }
        public bool IsSuccessful { get; set; }
        public T ReturnObject { get; set; }
        public string Message { get; set; }
    }
}
