using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Tests.Result
{
    public class Result
    {
        internal Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }

        public static Result Success()
        {
            return new Result(true, Array.Empty<string>());
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }

        public static Result Failure(string error)
        {
            return new Result(false, new[] { error });
        }
    }

    public class Result<T> : Result
    {
        internal Result(bool succeeded, T data, IEnumerable<string> errors)
            : base(succeeded, errors)
        {
            Data = data;
        }

        public T Data { get; set; }

        public static Result<T> Success(T data)
        {
            return new Result<T>(true, data, Array.Empty<string>());
        }

        public static new Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false, default, errors);
        }

        public static new Result<T> Failure(string error)
        {
            return new Result<T>(false, default, new[] { error });
        }
    }

    public class PaginatedResult<T> : Result<List<T>>
    {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        private PaginatedResult(
            bool succeeded,
            List<T> data,
            int pageNumber,
            int pageSize,
            int totalCount,
            IEnumerable<string> errors)
            : base(succeeded, data, errors)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public static PaginatedResult<T> Success(List<T> data, int pageNumber, int pageSize, int totalCount)
        {
            return new PaginatedResult<T>(true, data, pageNumber, pageSize, totalCount, Array.Empty<string>());
        }

        public static new PaginatedResult<T> Failure(IEnumerable<string> errors)
        {
            return new PaginatedResult<T>(false, new List<T>(), 0, 0, 0, errors);
        }
    }
}
