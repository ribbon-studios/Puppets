using Bogus;

namespace UnitTests
{
    internal class PTU
    {
        private static Faker _faker;
        public static Faker Faker
        {
            get
            {
                if (PTU._faker == null)
                {
                    PTU._faker = new Faker("en");
                }

                return PTU._faker;
            }
        }
    }
}
