using Flunt.Notifications;

namespace BankingSystem.Domain.Common;

public class Result
{
    public bool Success { get; private set; }
    public IReadOnlyCollection<Notification> Notifications { get; private set; }
    public string Message { get; private set; }

    protected Result(bool success, IReadOnlyCollection<Notification> notifications, string message)
    {
        Success = success;
        Notifications = notifications ?? new List<Notification>();
        Message = message;
    }

    public static Result Ok(string message = "")
    {
        return new Result(true, new List<Notification>(), message);
    }

    public static Result Fail(IReadOnlyCollection<Notification> notifications, string message = "")
    {
        return new Result(false, notifications, message);
    }
}

public class Result<T> : Result
{
    public T Data { get; private set; }

    private Result(bool success, IReadOnlyCollection<Notification> notifications, T data, string message)
        : base(success, notifications, message)
    {
        Data = data;
    }

    public static Result<T> Ok(T data, string message = "")
    {
        return new Result<T>(true, new List<Notification>(), data, message);
    }

    public static Result<T> Fail(IReadOnlyCollection<Notification> notifications, T data = default, string message = "")
    {
        return new Result<T>(false, notifications, data, message);
    }
}