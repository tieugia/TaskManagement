using System.IdentityModel.Tokens.Jwt;

namespace TaskManagement.Common.Helpers;

public static class JwtDecode
{
    public static string GetUserIdFromToken(string jwt)
    {
        var token = jwt.Replace("\"", "").Trim();
        var handler = new JwtSecurityTokenHandler();
        var decoded = handler.ReadJwtToken(token);
        return decoded.Claims.First(c => c.Type == "sub").Value;
    }
}
