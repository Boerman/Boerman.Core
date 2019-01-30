using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

using Timer = System.Timers.Timer;

namespace Boerman.Core
{
    /// <summary>
    /// The <see cref="ScalingDataProcessor{TProcessor, TObject, TResult}" /> class 
    /// provides a way to process data in a multithreaded fashion with classes
    /// which are not thread-safe. This class will rely on instantiation of
    /// multiple objects to facilitate the throughput of a certain amount of
    /// data. This class automatically creates and disposes instances based on
    /// the actual throughput, and then provides data in a synchronous fashion
    /// to each available instance.
    /// </summary>
    public class ScalingDataProcessor<TProcessor, TIn, TOut>
    {
        private Func<TProcessor> _instanceProvider;
        private Func<TProcessor, TIn, TOut> _processor;

        private Timer _timer;
        private Random _random = new Random();

        private QueueMetric _currentMetricCycle = new QueueMetric();

        private ConcurrentQueue<TIn> _queue = new ConcurrentQueue<TIn>();
        private List<ProcessorInstance<TProcessor>> _instances = new List<ProcessorInstance<TProcessor>>();

        public int UpscaleTreshold { get; set; }
        public int DownscaleTreshold { get; set; }

        public int MinInstanceCount { get; set; }
        public int MaxInstanceCount { get; set; }

        public ScalingDataProcessor(
            int evaluationInterval,
            Func<TProcessor> instanceProvider,
            Func<TProcessor, TIn, TOut> processor)
        {
            _timer = new Timer(evaluationInterval);
            _instanceProvider = instanceProvider;
            _processor = processor;

            _timer.Elapsed += UpdateMetrics;
            _timer.Start();
        }

        void UpdateMetrics(object sender, ElapsedEventArgs e)
        {
            // ToDo: Fire an event when we scale up or down
            // ToDo: Fire an update when we have new metrics available over said time period.
            //       We do not want to have this data in memory just in case someone needs it.
            //       Therefore if you want to use it you'd have to subscribe to it.
            // ToDo: Only scale when there are an excessive amount of data points which cannot be processed.

            int delta = _currentMetricCycle.Enqueued - _currentMetricCycle.Dequeued;
            int averageInstanceThroughput = _currentMetricCycle.Dequeued / _instances.Count;

            var requiredInstances = delta / averageInstanceThroughput;

            // If this is positive, add instances. If negative, remove some
            var requiredInstanceChange = requiredInstances - _instances.Count;

            if (requiredInstanceChange < 0) {
                _instances.RemoveRange(0, -requiredInstanceChange);
            } else if (requiredInstanceChange > 0) {
                Parallel.For(0, requiredInstanceChange, (i) =>
                {
                    _instances.Add(new ProcessorInstance<TProcessor>(_instanceProvider.Invoke()));
                });
            }

            _currentMetricCycle.Reset();
        }

        public async Task<TOut> Process(TIn item)
        {
            _queue.Enqueue(item);

            _currentMetricCycle.Enqueued++;

            var instance = _instances[_random.Next(0, _instances.Count)];

            return await Task.Run(() =>
            {
                lock (instance.Lock)
                {
                    return _processor.Invoke(instance.Instance, item);
                }
            });
        }
    }

    internal class ProcessorInstance<TProcessor>
    {
        internal readonly object Lock = new object();
        internal TProcessor Instance { get; }

        public ProcessorInstance(TProcessor instance)
        {
            Instance = instance;
        }   
    }

    internal struct QueueMetric
    {
        public int Enqueued { get; set; }
        public int Dequeued { get; set; }

        public void Reset()
        {
            Enqueued = 0;
            Dequeued = 0;
        }
    }
}
