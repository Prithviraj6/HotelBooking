export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: 'SuperAdmin' | 'HotelAdmin' | 'Customer';
  managedHotelId?: number | null;
}

export interface AuthResponse {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: 'SuperAdmin' | 'HotelAdmin' | 'Customer';
  token: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
}
