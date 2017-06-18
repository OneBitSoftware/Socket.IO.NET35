using System.ComponentModel;

namespace Socket.IO.NET35.Tasks
{
    /// <summary>
    /// Provides data for the System.ComponentModel.Custom.Generic.BackgroundWorker.DoWork event.
    /// </summary>
    /// <typeparam name="TArgument">The type of argument passed to the worker.</typeparam>
    /// <typeparam name="TResult">The type of result retrieved from the worker.</typeparam>
    public sealed class DoWorkEventArgs<TArgument, TResult> : CancelEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.Custom.Generic.DoWorkEventArgs class.
        /// </summary>
        /// <param name="argument">Specifies an argument for an asynchronous operation.</param>
        public DoWorkEventArgs(TArgument argument)
        {
            Argument = argument;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that represents the argument of an asynchronous operation.
        /// </summary>
        public TArgument Argument
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets a value that represents the result of an asynchronous operation.
        /// </summary>
        public TResult Result
        {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// Provides data for the System.ComponentModel.Custom.Generic.BackgroundWorker.DoWork event.
    /// </summary>
    /// <typeparam name="T">The type of argument passed to the worker
    /// and the type of result retrieved from the worker.</typeparam>
    public sealed class DoWorkEventArgs<T> : CancelEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.Custom.Generic.DoWorkEventArgs class.
        /// </summary>
        /// <param name="argument">Specifies an argument for an asynchronous operation.</param>
        public DoWorkEventArgs(T argument)
        {
            Argument = argument;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that represents the argument of an asynchronous operation.
        /// </summary>
        public T Argument
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets a value that represents the result of an asynchronous operation.
        /// </summary>
        public T Result
        {
            get;
            set;
        }

        #endregion
    }
}