using CalendarEvents.Models;
using System;
using System.Diagnostics.Contracts;

namespace CalendarEvents.Services
{
    public class ResultHandler
    {
        public bool Success { get; private set; }
        public ErrorCode ErrorCode { get; private set; }
        public Exception Exception { get; private set; }
        public string ErrorMessage { get; private set; }

        public bool Failure
        {
            get { return !Success; }
        }

        public ResultHandler(bool success, ErrorCode errorCode, string errorMessage)
        {
            Contract.Requires(success);            

            Success = success;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public ResultHandler(Exception error)
        {
            Contract.Requires(error != null);            

            Success = false;
            Exception = error;
            ErrorCode = ErrorCode.Unknown;
        }

        public static ResultHandler Fail(ErrorCode errorCode)
        {
            return new ResultHandler(false, errorCode, errorCode.ToString());
        }

        public static ResultHandler Fail(ErrorCode errorCode, string errorMessage)
        {
            return new ResultHandler(false, errorCode, errorMessage);
        }

        public static ResultHandler Fail(Exception exception)
        {
            return new ResultHandler(exception);
        }

        public static ResultHandler<T> Fail<T>(ErrorCode errorCode)
        {
            return new ResultHandler<T>(default, false, errorCode, null);
        }

        public static ResultHandler<T> Fail<T>(ErrorCode errorCode, string errorMessage)
        {
            return new ResultHandler<T>(default, false, errorCode, errorMessage);
        }

        public static ResultHandler<T> Fail<T>(Exception exception)
        {
            return new ResultHandler<T>(exception);
        }

        public static ResultHandler Ok()
        {
            return new ResultHandler(true, ErrorCode.Undefined, null);
        }

        public static ResultHandler<T> Ok<T>(T value)
        {
            if (value == null)
                return new ResultHandler<T>(value, false, ErrorCode.NotFound, "Value is Null");
            else
                return new ResultHandler<T>(value, true, ErrorCode.Undefined, null);
        }        
    }


    public class ResultHandler<T> : ResultHandler
    {
        private T _value;

        public T Value
        {
            get
            {
                Contract.Requires(Success);

                return _value;
            }
            private set { _value = value; }
        }

        public ResultHandler(T value, bool success, ErrorCode errorCode, string errorMessage)
            : base(success, errorCode, errorMessage)
        {
            Contract.Requires(value != null || !success);

            Value = value;
        }

        public ResultHandler(Exception exception)
            : base(exception)
        {                        
        }
    }
}