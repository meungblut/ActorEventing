using System;

namespace Euventing.Api.WebApi
{
    internal class NoStartupConfigurationsException : Exception
    {
        public NoStartupConfigurationsException(string message): base(message)
        {
        }
    }
}