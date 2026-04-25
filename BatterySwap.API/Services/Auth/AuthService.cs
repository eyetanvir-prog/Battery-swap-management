using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BatterySwap.API.Data;
using BatterySwap.API.DTOs.Auth;
using BatterySwap.API.Options;
using BatterySwap.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BatterySwap.API.Services.Auth;

public class AuthService(AppDbContext dbContext, IOptions<JwtOptions> jwtOptionsAccessor)
{
    private readonly JwtOptions _jwtOptions = jwtOptionsAccessor.Value;

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedIdentity = request.UsernameOrEmail.Trim();
        if (string.IsNullOrWhiteSpace(normalizedIdentity) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var admin = await dbContext.Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == normalizedIdentity, cancellationToken);

        if (admin is not null && BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
        {
            return BuildResponse(admin.Id, admin.Name, Roles.Admin, null);
        }

        var employee = await dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == normalizedIdentity && x.Status == "Active", cancellationToken);

        if (employee is not null && BCrypt.Net.BCrypt.Verify(request.Password, employee.PasswordHash))
        {
            return BuildResponse(employee.Id, employee.Name, Roles.Employee, employee.StationId);
        }

        return null;
    }

    private LoginResponse BuildResponse(int userId, string displayName, string role, int? stationId)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, displayName),
            new(ClaimTypes.Name, displayName),
            new(ClaimTypes.Role, role)
        };

        if (stationId.HasValue)
        {
            claims.Add(new Claim("station_id", stationId.Value.ToString()));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return new LoginResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            Role = role,
            UserId = userId,
            DisplayName = displayName
        };
    }
}
