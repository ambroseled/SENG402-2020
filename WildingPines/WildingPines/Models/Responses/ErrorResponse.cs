namespace WildingPines.Controllers.Responses
{
    /// <summary>
    /// Used to send a message back to the client when something goes wrong.
    /// </summary>
    public class ErrorResponse
    {
        public ErrorResponse(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}