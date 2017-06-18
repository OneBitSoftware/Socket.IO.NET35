using System;
using System.ComponentModel;

namespace Socket.IO.NET35.Tasks
{
    /// <summary>
    /// Provides data for the System.ComponentModel.Custom.Generic.RunWorkerCompleted event.
    /// </summary>
    /// <typeparam name="T">The type of result retrieved from the worker.</typeparam>
    public sealed class RunWorkerCompletedEventArgs<T> : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.Custom.Generic.RunWorkerCompletedEventArgs class.
        /// </summary>
        /// <param name="result">The result of an asynchronous operation.</param>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="cancelled">A value indicating whether the asynchronous operation was cancelled.</param>
        public RunWorkerCompletedEventArgs(T result, Exception error, bool cancelled)
        {
            Result = result;
            Error = error;
            Cancelled = cancelled;
        }

        #endregion

        #region Operator Overloads

        /// <summary>
        /// System.ComponentModel.RunWorkerCompletedEventArgs derives from System.ComponentModel.AsyncCompletedEventArgs.
        /// For compatibility, this method facilitates implicit conversion to System.ComponentModel.AsyncCompletedEventArgs.
        /// </summary>
        /// <param name="e">The System.ComponentModel.Custom.Generic.RunWorkerCompletedEventArgs instance to convert.</param>
        /// <returns>A System.ComponentModel.AsyncCompletedEventArgs instance.</returns>
        public static implicit operator AsyncCompletedEventArgs(RunWorkerCompletedEventArgs<T> e)
        {
            return new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.Result);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether an asynchronous operation has been canceled.
        /// </summary>
        public bool Cancelled
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets a value indicating which error occurred during an asynchronous operation.
        /// </summary>
        public Exception Error
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets a value that represents the result of an asynchronous operation.
        /// </summary>
        public T Result
        {
            get;
            private set;
        }

        #endregion
    }
}