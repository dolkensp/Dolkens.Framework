using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dolkens.Framework.Telemetry
{
    public class TimedSection : IDisposable
    {
        private static ThreadLocal<String> _activeId = new ThreadLocal<String> { };
        private static TelemetryClient _client = new TelemetryClient(TelemetryConfiguration.Active);

        private String _parentId;
        internal IOperationHolder<DependencyTelemetry> _operation;

        private Boolean? _success;

        public Boolean? Success
        {
            get { return this._operation.Telemetry.Success; }
            set
            {
                this._success = value;
                this._operation.Telemetry.Success = value;
            }
        }

        public String ResultCode
        {
            get { return this._operation.Telemetry.ResultCode; }
            set { this._operation.Telemetry.ResultCode = value; }
        }

        public Guid ID { get; } = Guid.NewGuid();

        public IDictionary<String, String> Properties
        {
            get { return this._operation.Telemetry.Properties; }
        }

        public IDictionary<String, Double> Metrics
        {
            get { return this._operation.Telemetry.Metrics; }
        }

        private Exception _lastException;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationName">The name of the timed section</param>
        /// <param name="args">Any arguments useful for tracking the timed section</param>
        public TimedSection(String operationName, params Object[] args)
        {
            this._parentId = TimedSection._activeId?.Value;
            TimedSection._activeId.Value = $"{this.ID}";

            this._lastException = LastException.GetLastException();

            this._operation = TimedSection._client.StartOperation<DependencyTelemetry>(operationName, $"{this.ID}", this._parentId);
            this._operation.Telemetry.Sequence = $"{DateTime.UtcNow.Ticks}";
            this._operation.Telemetry.Type = "TimedSection";
            this._operation.Telemetry.Data = args.ToJSON(Newtonsoft.Json.Formatting.None);
        }
        
        public void Dispose()
        {
            if (this._success != true)
            {
                var ex = LastException.GetLastException();

                // If an exception occurred, and it isn't the exception that was last thrown - flag a failure.
                if ((ex != null) && (ex != this._lastException))
                {
                    this._operation.Telemetry.Success = false;
                    this._operation.Telemetry.ResultCode = ex.Message;
                }
            }
            
            TimedSection._client.StopOperation(this._operation);
            
            TimedSection._activeId.Value = this._parentId;
        }

        #region Static Helpers

        public static void Run(Action @delegate, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", args))
            {
                @delegate.Invoke();
            }
        }

        public static void Run<T1>(Action<T1> @delegate, T1 arg1, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, args))
            {
                @delegate.Invoke(arg1);
            }
        }

        public static void Run<T1, T2>(Action<T1, T2> @delegate, T1 arg1, T2 arg2, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, args))
            {
                @delegate.Invoke(arg1, arg2);
            }
        }

        public static void Run<T1, T2, T3>(Action<T1, T2, T3> @delegate, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, args))
            {
                @delegate.Invoke(arg1, arg2, arg3);
            }
        }

        public static void Run<T1, T2, T3, T4>(Action<T1, T2, T3, T4> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4);
            }
        }

        public static void Run<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            }
        }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args))
            {
                @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            }
        }

        public static TOut Run<TOut>(Func<TOut> @delegate, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", args))
            {
                return @delegate.Invoke();
            }
        }

        public static TOut Run<T1, TOut>(Func<T1, TOut> @delegate, T1 arg1, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, args))
            {
                return @delegate.Invoke(arg1);
            }
        }

        public static TOut Run<T1, T2, TOut>(Func<T1, T2, TOut> @delegate, T1 arg1, T2 arg2, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, args))
            {
                return @delegate.Invoke(arg1, arg2);
            }
        }

        public static TOut Run<T1, T2, T3, TOut>(Func<T1, T2, T3, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3);
            }
        }

        public static TOut Run<T1, T2, T3, T4, TOut>(Func<T1, T2, T3, T4, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, TOut>(Func<T1, T2, T3, T4, T5, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, TOut>(Func<T1, T2, T3, T4, T5, T6, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, T7, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, T7, T8, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            }
        }

        public static TOut Run<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TOut>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TOut> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            using (new TimedSection($"{@delegate.Method.DeclaringType.FullName}.{@delegate.Method.Name}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args))
            {
                return @delegate.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            }
        }

        #endregion
    }
}
