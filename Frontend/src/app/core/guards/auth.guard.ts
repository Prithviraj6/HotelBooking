import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  // Check roles if required by the route
  const expectedRoles = route.data['roles'] as Array<string>;
  if (expectedRoles && expectedRoles.length > 0) {
    const userRole = authService.userRole();
    if (!userRole || !expectedRoles.includes(userRole)) {
      router.navigate(['/']); // Redirect to home or unauthorized page
      return false;
    }
  }

  return true;
};
