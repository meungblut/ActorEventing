using System;

namespace Eventing.Atom.Burst
{
    public class Date
    {
        public static Func<DateTime> CurrentDate = () => DateTime.Now; 
    }
}
