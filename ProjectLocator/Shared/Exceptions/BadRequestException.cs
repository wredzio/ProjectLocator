using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace ProjectLocator.Shared.Exceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException()
        { }

        public BadRequestException(string message) : base(message)
        { }

        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        { }

        public BadRequestException(string message, Exception innerException, object model)
            : base(message, innerException, HttpStatusCode.BadRequest, model)
        { }
    }
}
