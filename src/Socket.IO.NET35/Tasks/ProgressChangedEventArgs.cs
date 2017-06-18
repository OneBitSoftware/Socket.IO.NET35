using System;

namespace Socket.IO.NET35.Tasks
{
    /// <summary>
    /// Provides data for the System.ComponentModel.Custom.Generic.BackgroundWorker.ProgressChanged event.
    /// </summary>
    /// <typeparam name="T">The type of UserState.</typeparam>
    public sealed class ProgressChangedEventArgs<T> : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.Custom.Generic.ProgressChangedEventArgs class.
        /// </summary>
        /// <param name="progressPercentage">The percentage of an asynchronous task that has been completed.</param>
        /// <param name="userState">A unique user state.</param>
        public ProgressChangedEventArgs(int progressPercentage, T userState)
        {
            ProgressPercentage = progressPercentage;
            UserState = userState;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the asynchronous task progress percentage.
        /// </summary>
        public int ProgressPercentage
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets a unique user state.
        /// </summary>
        public T UserState
        {
            get;
            private set;
        }

        #endregion
    }
}