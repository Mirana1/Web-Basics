namespace SIS.HTTP.Exceptions
{
    using System;

    public class BadRequestException : Exception
    {
        private const string BadRequestExceptionMssg = "The Request was malformed or contains unsupported elements.";

        public BadRequestException() : this(BadRequestExceptionMssg)
        {
        }

        public BadRequestException(string name) : base(name)
        {
        }
    }
}
