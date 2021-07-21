using System.Collections.Generic;
using Model.Capabilities;
using System.Net;

namespace Model.Exceptions
{
    public class InvalidProductException : RestException
    {
        /// <param name="invalidMessage">Specify the the reason why this product is invalid</param>
        public InvalidProductException(string invalidMessage) : base((int) ExceptionCode.InvalidProductException,
            $"The product is invalid. {invalidMessage}", HttpStatusCode.BadRequest, "The product values are invalid.") { }
        
    }
}
