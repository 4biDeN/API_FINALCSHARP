using System;

namespace apifinal.Services.Exceptions

{
    public class InvalidEntityException : Exception
    {
        public InvalidEntityException(string message) : base(message) { }
    }
}
