namespace SIS.HTTP.Exceptions
{
    using System;

    public class InternalServerErrorException : Exception
    {
        public const string InternalServerErrorExceptionMssg = "The Server has encountered an error.";

        public InternalServerErrorException() : this(InternalServerErrorExceptionMssg)
        {
        }

        public InternalServerErrorException(string name):base(name)
        {
        }
    }
}
