using System;
using System.Collections.Generic;

namespace ElectronNETVueDemon.Authentication
{
    public interface IAccessTokenGenerator
    {
        string GenerateToken(string userId, List<string> permissions, DateTime expires);
    }
}