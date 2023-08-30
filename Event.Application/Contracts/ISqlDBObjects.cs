using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Application.Contracts
{
    public interface ISqlDBObjects
    {
        IStoredProcedures StoredProcedures { get; }
        ITableFunctions TableFunctions { get; }
    }

    public interface IStoredProcedures
    {
        Task LogSystemErrorAsync(string error_message, string request_uri);
    }

    public interface ITableFunctions
    {

    }
}
