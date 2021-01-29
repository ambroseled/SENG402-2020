namespace WildingPines.Models.Responses
{
    /// <summary>
    /// A response that allows to say whether the request succeeded and can attach a message.
    /// This can be used for operations that do not expect a data type back.
    /// </summary>
    public class SuccessResponse
    {
        public string Message { get; }
        
        public SuccessResponse(string message)
        {
            Message = message;
        }
    }
}