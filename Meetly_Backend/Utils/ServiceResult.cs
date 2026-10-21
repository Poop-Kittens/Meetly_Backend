using Microsoft.AspNetCore.Mvc;

namespace Meetly_Backend.Common
{
    public enum ServiceError
    {
        None,
        Validation,   // 400
        Unauthorized, // 401
        Forbidden,    // 403
        NotFound,     // 404
        Conflict      // 409
    }

    /// <summary>Result of a service operation without a return value.</summary>
    public class ServiceResult
    {
        public bool Success { get; }
        public bool Failure => !Success;
        public ServiceError Error { get; }
        public string? Message { get; }
        public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

        protected ServiceResult(bool success, ServiceError error, string? message,
            IReadOnlyDictionary<string, string[]>? validationErrors = null)
        {
            Success = success;
            Error = error;
            Message = message;
            ValidationErrors = validationErrors;
        }

        // ---------- Success ----------
        public static ServiceResult Ok() => new(true, ServiceError.None, null);
        public static ServiceResult<T> Ok<T>(T data) => new(data);

        // ---------- Failures (non-generic) ----------
        public static ServiceResult Fail(ServiceError error, string message) => new(false, error, message);
        public static ServiceResult NotFound(string message = "Resource not found.") => Fail(ServiceError.NotFound, message);
        public static ServiceResult Conflict(string message) => Fail(ServiceError.Conflict, message);
        public static ServiceResult Forbidden(string message = "You are not allowed to do this.") => Fail(ServiceError.Forbidden, message);
        public static ServiceResult Unauthorized(string message = "Authentication required.") => Fail(ServiceError.Unauthorized, message);
        public static ServiceResult Invalid(string message) => Fail(ServiceError.Validation, message);
        public static ServiceResult Invalid(IDictionary<string, string[]> errors) =>
            new(false, ServiceError.Validation, "One or more validation errors occurred.",
                new Dictionary<string, string[]>(errors));

        // ---------- Failures (generic) ----------
        public static ServiceResult<T> Fail<T>(ServiceError error, string message) => new(error, message);
        public static ServiceResult<T> NotFound<T>(string message = "Resource not found.") => Fail<T>(ServiceError.NotFound, message);
        public static ServiceResult<T> Conflict<T>(string message) => Fail<T>(ServiceError.Conflict, message);
        public static ServiceResult<T> Forbidden<T>(string message = "You are not allowed to do this.") => Fail<T>(ServiceError.Forbidden, message);
        public static ServiceResult<T> Unauthorized<T>(string message = "Authentication required.") => Fail<T>(ServiceError.Unauthorized, message);
        public static ServiceResult<T> Invalid<T>(string message) => Fail<T>(ServiceError.Validation, message);
        public static ServiceResult<T> Invalid<T>(IDictionary<string, string[]> errors) =>
            new(ServiceError.Validation, "One or more validation errors occurred.",
                new Dictionary<string, string[]>(errors));
    }

    /// <summary>Result of a service operation that returns data on success.</summary>
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; }

        internal ServiceResult(T data) : base(true, ServiceError.None, null)
        {
            Data = data;
        }

        internal ServiceResult(ServiceError error, string message,
            IReadOnlyDictionary<string, string[]>? validationErrors = null)
            : base(false, error, message, validationErrors) { }

        // Lets a service simply "return meeting;" on success.
        public static implicit operator ServiceResult<T>(T data) => new(data);
    }

    /// <summary>Maps a ServiceResult to an HTTP response inside a controller.</summary>
    public static class ServiceResultExtensions
    {
        public static IActionResult ToActionResult(this ServiceResult result, ControllerBase controller)
        {
            if (result.Success) return controller.NoContent();
            return MapError(result, controller);
        }

        public static ActionResult<T> ToActionResult<T>(this ServiceResult<T> result, ControllerBase controller,
        Func<T, ActionResult>? onSuccess = null)
        {
            if (result.Failure) return MapError(result, controller);

            if (onSuccess is not null) return onSuccess(result.Data!);
            return result.Data!; // implicit conversion to ActionResult<T> => 200 with body
        }

        private static ActionResult MapError(ServiceResult result, ControllerBase controller)
        {
            switch (result.Error)
            {
                case ServiceError.Validation when result.ValidationErrors is not null:
                    var modelState = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
                    foreach (var (key, messages) in result.ValidationErrors)
                        foreach (var m in messages) modelState.AddModelError(key, m);
                    return controller.ValidationProblem(modelState);

                case ServiceError.Validation:
                    return controller.Problem(result.Message, statusCode: StatusCodes.Status400BadRequest);
                case ServiceError.Unauthorized:
                    return controller.Problem(result.Message, statusCode: StatusCodes.Status401Unauthorized);
                case ServiceError.Forbidden:
                    return controller.Problem(result.Message, statusCode: StatusCodes.Status403Forbidden);
                case ServiceError.NotFound:
                    return controller.Problem(result.Message, statusCode: StatusCodes.Status404NotFound);
                case ServiceError.Conflict:
                    return controller.Problem(result.Message, statusCode: StatusCodes.Status409Conflict);
                default:
                    return controller.Problem(result.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}