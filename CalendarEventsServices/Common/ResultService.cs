using CalendarEvents.Models;
using System;
using System.Diagnostics.Contracts;

namespace CalendarEvents.Services
{
    public class ResultService
    {
        public bool Success { get; private set; }
        public ErrorCode ErrorCode { get; private set; }
        public Exception Exception { get; private set; }

        public bool Failure
        {
            get { return !Success; }
        }

        public ResultService(bool success, ErrorCode errorCode)
        {
            Contract.Requires(success);            

            Success = success;
            ErrorCode = errorCode;
        }

        public ResultService(Exception error)
        {
            Contract.Requires(error != null);            

            Success = false;
            Exception = error;
            ErrorCode = ErrorCode.Unknown;
        }

        public static ResultService Fail(ErrorCode errorCode)
        {
            return new ResultService(false, errorCode);
        }

        public static ResultService Fail(Exception exception)
        {
            return new ResultService(exception);
        }

        public static ResultService<T> Fail<T>(ErrorCode errorCode)
        {
            return new ResultService<T>(default, false, errorCode);
        }

        public static ResultService<T> Fail<T>(Exception exception)
        {
            return new ResultService<T>(exception);
        }

        public static ResultService Ok()
        {
            return new ResultService(true, ErrorCode.Undefined);
        }

        public static ResultService<T> Ok<T>(T value)
        {
            return new ResultService<T>(value, true, ErrorCode.Undefined);
        }

        public static ResultService Combine(params ResultService[] results)
        {
            foreach (ResultService result in results)
            {
                if (result.Failure)
                    return result;
            }

            return Ok();
        }
    }


    public class ResultService<T> : ResultService
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

        public ResultService(T value, bool success, ErrorCode errorCode)
            : base(success, errorCode)
        {
            Contract.Requires(value != null || !success);

            Value = value;
        }

        public ResultService(Exception exception)
            : base(exception)
        {                        
        }
    }
}
