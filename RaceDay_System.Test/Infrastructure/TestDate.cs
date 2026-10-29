namespace RaceDay_System.Test.Infrastructure
{
    public static class TestDate
    {
        public static DateTime FutureDate => DateTime.Now.AddDays(30);
        public static DateTime PastDate => DateTime.Now.AddDays(-30);
    }
}