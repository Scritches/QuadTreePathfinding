using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QTTest2
{
    /// <summary>
    /// implementation of a high resolution timer
    /// </summary>
    public class HighResolutionTimer
    {

        /// <summary>
        /// exception message
        /// </summary>
        private const string conNotSupportedExceptionMessage =
            "system does not support this implementation of the high performance timer";

        /// <summary>
        /// exception message
        /// </summary>
        private const string conIsRunningExceptionMessage =
            "timer is already running";

        /// <summary>
        /// exception message
        /// </summary>
        private const string conIsNotRunningExceptionMessage =
            "timer was not started";

        /// <summary>
        /// get os perfomance counter value
        /// </summary>
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private extern static short QueryPerformanceCounter(ref long cycles);

        /// <summary>
        /// get os performance counter frequency (cycles ber second)
        /// </summary>
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private extern static short QueryPerformanceFrequency(ref long cycles);

        /// <summary>
        /// duration
        /// </summary>
        private long _Duration;

        /// <summary>
        /// frequency in cycles per second
        /// </summary>
        private long _Frequency;

        /// <summary>
        /// value of os performace counter at start
        /// </summary>
        private long _StartValue;

        /// <summary>
        /// true if platform supports QueryPerformanceCounter
        /// </summary>
        private bool _IsSupported;

        /// <summary>
        /// true if timer is running
        /// </summary>
        private bool _IsRunning;

        /// <summary>
        /// constructor
        /// </summary>
        public HighResolutionTimer()
        {
            _Frequency = 0;
            _StartValue = 0;

            // frequency used here not to create a new variable as start
            if (QueryPerformanceCounter(ref _StartValue) != 0)
            {
                _IsSupported = true;

                // determine frequency
                QueryPerformanceFrequency(ref _Frequency);
            }
            else
            {
                _IsSupported = false;
            }

            // ensure the methods have been 'jitted' for first call
            if (_IsSupported)
            {
                Start();
                Stop();
            }
            _Duration = 0;
        }

        /// <summary>
        /// returns true if this implementation of a high resolution timer
        /// is supported by the system
        /// </summary>
        public bool IsSupported
        {
            get
            {
                return _IsSupported;
            }
        }

        /// <summary>
        /// start the timer
        /// </summary>
        public void Start()
        {
            if (!_IsSupported)
            {
                throw new System.NotSupportedException(conNotSupportedExceptionMessage);
            }
            if (_IsRunning)
            {
                throw new System.ApplicationException(conIsRunningExceptionMessage);
            }
            _IsRunning = true;

            QueryPerformanceCounter(ref _StartValue);
        }

        /// <summary>
        /// stop the timer
        /// </summary>
        public void Stop()
        {
            QueryPerformanceCounter(ref _Duration);
            if (!_IsRunning)
            {
                throw new System.ApplicationException(conIsNotRunningExceptionMessage);
            }
            _Duration -= _StartValue;
            _IsRunning = false;

            if (!_IsSupported)
            {
                throw new System.NotSupportedException(conNotSupportedExceptionMessage);
            }
        }

        /// <summary>
        /// number of cycles
        /// </summary>
        /// <remarks>
        /// dependent on cpu
        /// </remarks>
        public long Cycles
        {
            get
            {
                if (!_IsSupported)
                {
                    throw new System.NotSupportedException(conNotSupportedExceptionMessage);
                }
                return _Duration;
            }
        }

        /// <summary>
        /// number of milliseconds elapsed between start and stop
        /// </summary>
        public System.Decimal MilliSeconds
        {
            get
            {
                if (!_IsSupported)
                {
                    throw new System.NotSupportedException(conNotSupportedExceptionMessage);
                }
                return (System.Decimal)_Duration / (System.Decimal)_Frequency * 1000M;
            }
        }
    }
}