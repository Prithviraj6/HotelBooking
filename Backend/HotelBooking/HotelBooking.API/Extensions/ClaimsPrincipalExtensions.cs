using System.Security.Claims;
using HotelBooking.Domain.Enums;

namespace HotelBooking.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        public static UserRole GetRole(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.Role);
            return claim != null && Enum.TryParse<UserRole>(claim.Value, out var role) ? role : UserRole.Customer;
        }

        public static int? GetManagedHotelId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("HotelId");
            if (claim != null && int.TryParse(claim.Value, out var id))
                return id;
            return null;
        }

        public static bool IsSuperAdmin(this ClaimsPrincipal user)
        {
            return GetRole(user) == UserRole.SuperAdmin;
        }

        public static bool IsHotelAdmin(this ClaimsPrincipal user)
        {
            return GetRole(user) == UserRole.HotelAdmin;
        }
    }
}
