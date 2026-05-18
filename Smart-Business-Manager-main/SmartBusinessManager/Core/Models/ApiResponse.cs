namespace SmartBusinessManager.Core.Models;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; }
    public string ErrorMessage { get; set; }

    public static ApiResponse<T> Success(T data) =>
        new() { IsSuccess = true, Data = data };

    public static ApiResponse<T> Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
