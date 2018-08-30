namespace DLSM.Infrastructure.API.MdmServices.Models {
    public class AuthenticationResult {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string LoginToken { get; }

        public AuthenticationResult(bool isSuccess, string message, string loginToken) {
            IsSuccess = isSuccess;
            Message = message;
            LoginToken = loginToken;
        }
    }
}