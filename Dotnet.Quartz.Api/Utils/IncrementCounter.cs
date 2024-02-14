namespace Dotnet.Quartz.Api.Utils
{
    public static class IncrementCounter
    {
        private static int counter = 0;
        public static void SetCounter() => counter++;
        public static int GetCounter() => counter;
    }
}