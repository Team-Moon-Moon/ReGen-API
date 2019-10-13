using FirebaseAdmin.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public class FirebaseAuthService : IAuthService
    {
        private FirebaseAuth _auth;

        public FirebaseAuthService(FirebaseAuth auth)
        {
            _auth = auth;
        }

        public async Task<string> GetUid(string accessToken)
        {
            FirebaseToken decodedToken = await _auth.VerifyIdTokenAsync(accessToken);
            return decodedToken.Uid;
        }
    }
}
