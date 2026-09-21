using System.Text.Json.Serialization;

namespace SV22T1080045.Shop.Common.Result
{
    public class Result<T>
    {
        public Result(bool isSuccessed)
        {
            IsSuccessed = isSuccessed;
        }

        public Result(bool isSuccessed, string action, string message, T resultObj) : this(isSuccessed)
        {
            Action = action;
            Message = message;
            ResultObj = resultObj;
        }

        public bool IsSuccessed { get; set; }

        [JsonIgnore]
        public ResultStatus Status { get; private set; }

        public string Action { get; set; }
        public string Message { get; set; }

        public T ResultObj { get; set; }

        public static implicit operator Result<T>(Result result) => new(result.IsSuccessed, result.Action, result.Message, default) { Status = result.Status };

        public static Result<T> Success() => new(true) { Status = ResultStatus.Ok };
        public static Result<T> Success(string action, string message, T resultObj) => new(true, action, message, resultObj) { Status = ResultStatus.Ok };
        public static Result<T> Success(string action, string message) => new(true, action, message, default) { Status = ResultStatus.Ok };
        public static Result<T> Success(string action, T resultObj) => new(true, action, "Thành công!", resultObj) { Status = ResultStatus.Ok };
        public static Result<T> Success(string action) => new(true, action, "Thành công!", default) { Status = ResultStatus.Ok };
        public static Result<T> Error() => new(false) { Status = ResultStatus.Error };
        public static Result<T> Error(string action, string message, T resultObj) => new(false, action, message, resultObj) { Status = ResultStatus.Error };
        public static Result<T> Error(string action, string message) => new(false, action, message, default) { Status = ResultStatus.Error };
        public static Result<T> Error(string action, T resultObj) => new(false, action, "Thất bại!", resultObj) { Status = ResultStatus.Error };
        public static Result<T> Error(string action) => new(false, action, "Thất bại!", default) { Status = ResultStatus.Error };
        public static Result<T> NotFound() => new(false, "", "Không tìm thấy trang", default) { Status = ResultStatus.NotFound };
        public static Result<T> NotFound(string valueName) => new(false, "", $"{valueName} không tìm thấy", default) { Status = ResultStatus.NotFound };
    }

    public class Result
    {
        public Result(bool isSuccessed)
        {
            IsSuccessed = isSuccessed;
        }

        public Result(bool isSuccessed, string action, string message) : this(isSuccessed)
        {
            Message = message;
            Action = action;
        }
        public Result(bool isSuccessed, string message) : this(isSuccessed)
        {
            Message = message;
        }
        public bool IsSuccessed { get; private set; }

        [JsonIgnore]
        public ResultStatus Status { get; private set; }

        public string Message { get; set; }
        public string Action { get; set; }
        public static Result Success() => new(true) { Status = ResultStatus.Ok };
        public static Result Success(string action) => new(true, action, "Cập nhật  thành công!") { Status = ResultStatus.Ok };
        public static Result Success(string action, string message) => new(true, action, message) { Status = ResultStatus.Ok };
        public static Result Error() => new(false) { Status = ResultStatus.Error };
        public static Result Error(string action) => new(false, action, "Cập nhật không thành công!") { Status = ResultStatus.Error };
        public static Result Error(string action, string message) => new(false, action, message) { Status = ResultStatus.Error };
        public static Result NotFound(string action) => new(false, action, "Không tìm thấy trang") { Status = ResultStatus.NotFound };
        public static Result NotFound(string action, string valueName) => new(false, action, $"{valueName} không tìm thấy") { Status = ResultStatus.NotFound };
    }

    public enum ResultStatus
    {
        Ok,
        Error,
        NotFound
    }
}