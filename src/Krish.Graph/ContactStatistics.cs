using System.Threading;

namespace Krish.Graph
{
    public class ContactStatistics
    {
        private long _successfulCount;

        public long Successful => Interlocked.Read(ref _successfulCount);

        private long _skippedCount;

        public long Skipped => Interlocked.Read(ref _skippedCount);

        public long Total { get; internal set; }

        public int PercentCompletion => Total == 0 ? 0 : (int)((Successful + Skipped) * 100 / Total);

        internal void IncrementSuccessful()
        {
            Interlocked.Increment(ref _successfulCount);
        }

        internal void IncrementSkipped()
        {
            Interlocked.Increment(ref _skippedCount);
        }

        internal void Reset()
        {
            Interlocked.Exchange(ref _successfulCount, 0);
            Interlocked.Exchange(ref _skippedCount, 0);
            Total = 0;
        }
    }
}
