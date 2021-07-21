using System;
using System.Net;
using System.Runtime.Serialization;

namespace Model.Exceptions
{
    [Serializable]
    public abstract class RestException : Exception
    {
        public int Id { get; }
        public HttpStatusCode StatusCode { get; }
        public string ExternalMessage { get; }

        protected RestException(int id, string message, HttpStatusCode? statusCode = null,
            string externalMessage = null) : base(message)
        {
            Id = id;
            StatusCode = statusCode ?? HttpStatusCode.InternalServerError;
            ExternalMessage = externalMessage;
        }

        protected RestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = info.GetInt32("Id");
            StatusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode));
            ExternalMessage = info.GetString("ExternalMessage");
        }
    }
}