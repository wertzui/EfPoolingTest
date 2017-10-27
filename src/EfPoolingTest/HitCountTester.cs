using System.Diagnostics;

namespace EfPoolingTest
{
    public class HitCountTester
    {
        private Stopwatch stopwatch = new Stopwatch();
        private decimal hitCount;

        public void Start()
        {
            stopwatch.Start();
        }

        public void Stop()
        {
            stopwatch.Stop();
        }

        public void Hit()
        {
            hitCount++;
            var hitCountPerMillisecond = hitCount / stopwatch.ElapsedMilliseconds;
            var hitCountPerSecond = hitCountPerMillisecond * 1000;
            var hitCountPerMinute = hitCountPerSecond * 60;
            Debug.WriteLine($"Hits: {hitCountPerMinute:n2} per minute, {hitCountPerSecond:n2} per second, {hitCountPerMillisecond:n2} per millisecond.");
        }
    }
}