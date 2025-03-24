using Flunt.Notifications;
using System.Diagnostics.CodeAnalysis;

namespace BankingSystem.Domain.Common;

[ExcludeFromCodeCoverage]
public class Result
{
    public bool Success { get; private set; }
    public IList<Notification> Errors { get; private set; }
    public string Message { get; private set; }

    protected Result(bool success, IList<Notification> notifications, string message)
    {
        Success = success;
        Errors = notifications ?? [];
        Message = message;
    }

    protected Result(bool success, string message)
    {
        Success = success;
        Message = message;
        Errors = [];
    }

    public static Result Ok(string message = "")
    {
        return new Result(true, [], message);
    }

    public static Result Fail(IList<Notification> notifications, string message = "")
    {
        return new Result(false, notifications, message);
    }

    public static Result Fail(string message = "")
    {
        return new Result(false, message);
    }
}

public class Result<T> : Result
{
    public T? Data { get; private set; }
    private Result(bool success, IList<Notification> notifications, string message)
        : base(success, notifications, message) { }

    private Result(bool success, IList<Notification> notifications, T? data, string message)
        : base(success, notifications, message)
    {
        Data = data;
    }


    public static Result<T> Ok(T data = default!)
    {
        return new Result<T>(true, [], data, string.Empty);
    }

    public static new Result<T> Fail(IList<Notification> notifications, string message = "")
    {
        return new Result<T>(false, notifications, message);
    }

    public static new Result<T> Fail(string message = "")
    {
        return new Result<T>(false, [], default!, message);
    }
}
