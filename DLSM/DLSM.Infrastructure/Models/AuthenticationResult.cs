namespace DLSM.Infrastructure.API.MdmServices.Models {
    public class AuthenticationResult {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string LoginToken { get; }
        public MdmAuthenticationInput UserInput { get; }

        public AuthenticationResult(bool isSuccess, string message, string loginToken, MdmAuthenticationInput userInput) {
            IsSuccess = isSuccess;
            Message = message;
            LoginToken = loginToken;
            UserInput = userInput;
        }
    }
}