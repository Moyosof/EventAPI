using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Event.API.Helpers
{
    public struct ResponseMessages
    {
        public static string BadRequest
        {
            get { return "400 Bad request."; }
        }

        public static string UserAccountNotFound
        {
            get { return "User Account not Found"; }
        }

        public static string InvalidAccessToken
        {
            get { return "Invalid access token or no subscription was found for this user."; }
        }

        public static string SuccessLogin
        {
            get
            {
                return "";
            }
        }

        public static string IncorrectRole
        {
            get
            {
                { return "Invalid Role for this operation."; }
            }
        }

        public static string PasswordNotMatch
        {
            get
            {
                { return "Password does not match the account"; }
            }
        }

        public static string InternalServerError
        {
            get
            {
                return "Sorry, something went wrong while processing your request! " +
                       "We've noted it and we are going to fix this asap. " +
                       " Please try again later or contact info@selenia.app";
            }
        }

        public static string InvalidLogin
        {
            get { return "Invalid username or password."; }
        }

        public static string LoginException
        {
            get { return "Failed to sign you in at this moment, please try again later."; }
        }
        public static string DataExists
        {
            get { return "Sorry, email is already in use"; }
        }


        public static string AccountLockout
        {
            get
            {
                return "Your account has been locked out after several unsuccessful login attempts. " +
                       "It's best and secure to do so as this may be from an unauthorized outsider or hacker. " +
                       "Please try again in the next five minutes.";
            }
        }

    }
}
