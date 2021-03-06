﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace ProjectLocator.Shared.Exceptions
{
    public class UnauthorizedException : CustomException
    {     
        public UnauthorizedException(string message) : base(message)
        { }

        public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
        { }

        public UnauthorizedException(string message, Exception innerException, object model) 
            : base(message, innerException, HttpStatusCode.Unauthorized, model: model)
        { }
    }
}
