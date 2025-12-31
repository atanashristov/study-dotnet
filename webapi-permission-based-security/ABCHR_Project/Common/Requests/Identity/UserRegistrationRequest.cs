namespace Common.Requests.Identity
{
    public class UserRegistrationRequest
    {
        /// <summary>
        /// The first name of the user
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// The last name of the user
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// The email of the user
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// User name for the user
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password for the user
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Confirm password
        /// </summary>
        public string ComfirmPassword { get; set; }
        /// <summary>
        /// User's phone number
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// If `true`, activate the user automatically
        /// </summary>
        public bool ActivateUser { get; set; }
        /// <summary>
        /// It `true`, automatically mark the email as confirmed
        /// </summary>
        public bool AutoComfirmEmail { get; set; }
    }
}
