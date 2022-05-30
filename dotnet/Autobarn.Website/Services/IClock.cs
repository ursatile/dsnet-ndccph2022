using System;

namespace Autobarn.Website.Services {
    public interface IClock {
        DateTimeOffset Now { get; }
    }

    public class SystemClock : IClock {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }

    public class TestingClock : IClock {
        private readonly DateTimeOffset now;

        public TestingClock(DateTimeOffset now) {
            this.now = now;
        }

        public DateTimeOffset Now => now;
    }
}
