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

        public bool Failure
        {
            get { return !Success; }
        }

        public ResultHandler(bool success, ErrorCode errorCode)
        {
            Contract.Requires(success);            

            Success = success;
            ErrorCode = errorCode;
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
            return new ResultHandler(false, errorCode);
        }

        public static ResultHandler Fail(Exception exception)
        {
            return new ResultHandler(exception);
        }

        public static ResultHandler<T> Fail<T>(ErrorCode errorCode)
        {
            return new ResultHandler<T>(default, false, errorCode);
        }

        public static ResultHandler<T> Fail<T>(Exception exception)
        {
            return new ResultHandler<T>(exception);
        }

        public static ResultHandler Ok()
        {
            return new ResultHandler(true, ErrorCode.Undefined);
        }

        public static ResultHandler<T> Ok<T>(T value)
        {
            if (value == null)
                return new ResultHandler<T>(value, false, ErrorCode.NotFound);
            else
                return new ResultHandler<T>(value, true, ErrorCode.Undefined);
        }

        public static ResultHandler Combine(params ResultHandler[] results)
        {
            foreach (ResultHandler result in results)
            {
                if (result.Failure)
                    return result;
            }

            return Ok();
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

        public ResultHandler(T value, bool success, ErrorCode errorCode)
            : base(success, errorCode)
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
