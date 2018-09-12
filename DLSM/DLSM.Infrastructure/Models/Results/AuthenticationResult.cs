namespace DLSM.Infrastructure.Models {
    public class AuthenticationResult {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string LoginToken { get; set; }
        
    }
}