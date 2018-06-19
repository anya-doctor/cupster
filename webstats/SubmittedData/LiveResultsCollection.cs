using System;
using System.Threading;
using System.Threading.Tasks;

namespace SubmittedData
{
    public class LiveResultsCollection : ILiveResultsCollection, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public LiveResultsCollection()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public ILiveResults Current { get; set; }
        IResults IResultCollection.Previous
        {
            get => Previous;
            set => Previous = value as ILiveResults;
        }

        IResults IResultCollection.Current
        {
            get => Current;
            set => Current = (ILiveResults) value;
        }

        public ILiveResults Previous { get; set; }

        public async Task UpdateResultsAsync(TimeSpan interval)
        {
            await Task.Delay(interval, _cancellationTokenSource.Token);
            while (true)
            {
                UpdateResuls();
                await Task.Delay(interval, _cancellationTokenSource.Token);
                if (_cancellationTokenSource.IsCancellationRequested)
                    break;
            }
        }

        private void UpdateResuls()
        {
            Previous.Copy(Current);
            try
            {
                Current.Load(null);
            }
            catch
            {
                Current.Copy(Previous);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
