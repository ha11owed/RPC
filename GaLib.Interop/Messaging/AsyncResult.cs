using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    /// <summary>
    /// Encapsulates the results of an asynchronous operation.
    /// </summary>
    class AsyncResult : IAsyncResult
    {
        private bool _isCompleted = false;
        private ManualResetEvent _AsyncWaitHandle = null;
        private volatile object _asyncState;

        /// <summary>Gets a value indicating whether the server has completed the call.</summary>
        /// <returns>true after the server has completed the call; otherwise, false.</returns>
        public bool IsCompleted
        {
            get { return this._isCompleted; }
        }

        /// <summary>Gets the object provided as the last parameter of a BeginInvoke method call.</summary>
        /// <returns>The object provided as the last parameter of a BeginInvoke method call.</returns>
        public object AsyncState
        {
            get { return this._asyncState; }
            internal set { this._asyncState = value; }
        }

        public AMessage GetMessage()
        {
            return AsyncState as AMessage;
        }

        /// <summary>Gets a value indicating whether the BeginInvoke call completed synchronously.</summary>
        /// <returns>true if the BeginInvoke call completed synchronously; otherwise, false.</returns>
        public bool CompletedSynchronously
        {
            get { return false; }
        }

        /// <summary>Gets a <see cref="T:System.Threading.WaitHandle" /> that encapsulates Win32 synchronization handles, and allows the implementation of various synchronization schemes.</summary>
        /// <returns>A <see cref="T:System.Threading.WaitHandle" /> that encapsulates Win32 synchronization handles, and allows the implementation of various synchronization schemes.</returns>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                this.FaultInWaitHandle();
                return this._AsyncWaitHandle;
            }
        }

        private void FaultInWaitHandle()
        {
            lock (this)
            {
                if (this._AsyncWaitHandle == null)
                {
                    this._AsyncWaitHandle = new ManualResetEvent(false);
                }
            }
        }

        /// <summary>Synchronously processes a response message returned by a method call on a remote object.</summary>
        /// <returns>Returns null.</returns>
        /// <param name="msg">A response message to a method call on a remote object.</param>
        [SecurityCritical]
        internal void Complete()
        {
            this._isCompleted = true;
            this.FaultInWaitHandle();
            this._AsyncWaitHandle.Set();
        }
    }
}
