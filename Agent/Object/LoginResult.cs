namespace Demo.Object
{
    public class LoginResult
    {
        public string state { get; set; }
        public LoginError errors { get; set; }
    }
    public class LoginError
    {
        public string action_error { get; set; }
    }
}
