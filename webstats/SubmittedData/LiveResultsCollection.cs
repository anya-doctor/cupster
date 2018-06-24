﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace SubmittedData
{
    public class LiveResultsCollection : ILiveResultsCollection, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationTokenSource _cancellationTokenSourceCurrent;
        private readonly CancellationTokenSource _cancellationTokenSourcePrevious;

        public LiveResultsCollection()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSourceCurrent = new CancellationTokenSource();
            _cancellationTokenSourcePrevious = new CancellationTokenSource();
        }

        public ILiveResults Previous { get; set; }
        IResults IResultCollection.Previous
        {
            get => Previous;
            set => Previous = value as ILiveResults;
        }

        public ILiveResults Current { get; set; }
        IResults IResultCollection.Current
        {
            get => Current;
            set => Current = (ILiveResults) value;
        }

        public ILiveResults Backup { get; set; }
        IResults IResultCollection.Backup
        {
            get => Backup;
            set => Backup = value as ILiveResults;
        }


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

        public async Task UpdateCurrentResultsAsync(TimeSpan interval)
        {
            await Task.Delay(interval, _cancellationTokenSourceCurrent.Token);
            while (true)
            {
                Console.WriteLine("{0:G}: Update current", DateTime.Now);
                UpdateResults(Current);
                await Task.Delay(interval, _cancellationTokenSourceCurrent.Token);
                if (_cancellationTokenSourceCurrent.IsCancellationRequested)
                    break;
            }
        }

        public async Task UpdatePreviousResultsAsync(TimeSpan interval)
        {
            await Task.Delay(interval, _cancellationTokenSourcePrevious.Token);
            while (true)
            {
                Console.WriteLine("{0:G}: Update previous", DateTime.Now);
                UpdateResults(Previous);
                await Task.Delay(interval, _cancellationTokenSourcePrevious.Token);
                if (_cancellationTokenSourcePrevious.IsCancellationRequested)
                    break;
            }
        }

        private void UpdateResults(ILiveResults results)
        {
            Backup.Copy(Current);
            try
            {
                results.Load(null);
            }
            catch
            {
                results.Copy(Backup);
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
            _cancellationTokenSourceCurrent.Cancel();
            _cancellationTokenSourceCurrent.Dispose();
            _cancellationTokenSourcePrevious.Cancel();
            _cancellationTokenSourcePrevious.Dispose();
        }

    }
}
