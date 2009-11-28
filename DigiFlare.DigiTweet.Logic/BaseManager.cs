using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigiFlare.DigiTweet
{
    public class BaseManager
    {
        #region Instance Variables

        protected string _username = null;
        protected string _password = null;

        #endregion

        #region Constructor

        public BaseManager()
        {
        }

        public BaseManager(string username, string password)
        {
            _username = username;
            _password = password;
        }

        #endregion

        #region Custom Events

        public delegate void OperationCompletedHandler(object sender, OperationCompletedEventArgs e);
        
        public event OperationCompletedHandler OperationCompleted;

        public delegate void OperationErrorHandler(object sender, OperationErrorEventArgs e);
        public event OperationErrorHandler OperationError;

        #endregion

        #region Helper Event Methods

        protected virtual void OnError(string message, Exception exception)
        {
            if (null != OperationError)
            {
                OperationErrorEventArgs args = new OperationErrorEventArgs
                {
                    Message = message,
                    InnerException = exception
                };
                OperationError(this, args);
            }
        }

        protected virtual void OnCompleted(object info)
        {
            if (null != OperationCompleted)
            {
                OperationCompleted(this, new OperationCompletedEventArgs { Info = info });
            }
        }

        #endregion

    }
}
