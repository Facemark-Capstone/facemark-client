// David Wahid
using System;
using System.Net;

namespace mobile.Exceptions
{
    public class AnalysisException : Exception
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode HttpStatus { get; set; }

        public AnalysisException(string errorCode, string errorMessage, HttpStatusCode statusCode)
            : base(errorMessage + "(" + errorCode + ")")
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            HttpStatus = statusCode;
        }
    }
}
