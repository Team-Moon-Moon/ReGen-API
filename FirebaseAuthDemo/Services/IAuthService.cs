using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public interface IAuthService
    {
        Task<string> GetUid(string accessToken);
    }
}
