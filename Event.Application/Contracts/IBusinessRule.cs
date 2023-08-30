using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Application.Contracts
{
    public interface IBusinessRule
    {
        /// <summary>
        /// Fetches the id of the logged in user
        /// </summary>
        /// <returns>the Id of the user, otherwise empty string</returns>
        string GetLoggedInUserId();

        /// <summary>
        /// gets the email of the logged in user
        /// </summary>
        /// <returns>the email of the logged in user, otherwise empty string</returns>
        string GetCurrentLoggedinUserEmail();

        /// <summary>
        /// get the list of roles of the logged in user
        /// </summary>
        /// <returns>an array of strings, otherwise empty list</returns>
        List<string> GetCurrentLoggedinUserRole();
    }
}
