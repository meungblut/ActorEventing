using System;

namespace Eventing.Api.WebApi
{
    internal class NoStartupConfigurationsException : Exception
    {
        public NoStartupConfigurationsException(string message): base(message)
        {
        }
    }
}