using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Puppets.Time;
using System;

namespace Puppets.SeFunctions
{
    public class SeTime
    {
        public static TimeStamp GetServerTime()
            => new(Framework.GetServerTime() * 1000);

        private unsafe TimeStamp GetEorzeaTime()
        {
            var serverTime = SeTime.GetServerTime();
            var framework = Framework.Instance();
            if (framework == null)
                return serverTime.ConvertToEorzea();

            return Math.Abs(new TimeStamp(framework->ServerTime * 1000) - serverTime) < 5000
                ? new TimeStamp(framework->EorzeaTime * 1000)
                : serverTime.ConvertToEorzea();
        }
    }
}
