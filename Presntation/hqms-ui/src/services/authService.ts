import { LoginRequest, RegisterRequest, User, AuthTokens, ApiResponse } from '@/types';
import { apiService } from './api';

class AuthService {
  async login(credentials: LoginRequest): Promise<ApiResponse<{ user: User; tokens: AuthTokens }>> {
    return apiService.post('/auth/login', credentials);
  }

  async register(data: RegisterRequest): Promise<ApiResponse<{ user: User; tokens: AuthTokens }>> {
    return apiService.post('/auth/register', data);
  }

  async logout(): Promise<ApiResponse> {
    const refreshToken = localStorage.getItem('refreshToken');
    return apiService.post('/auth/logout', { refreshToken });
  }

  async forgotPassword(email: string): Promise<ApiResponse> {
    return apiService.post('/auth/forgot-password', { email });
  }

  async resetPassword(token: string, newPassword: string): Promise<ApiResponse> {
    return apiService.post('/auth/reset-password', { token, newPassword });
  }

  async verifyToken(): Promise<boolean> {
    try {
      const response = await apiService.get('/auth/verify');
      return response.success;
    } catch (error) {
      return false;
    }
  }

  async getCurrentUser(): Promise<ApiResponse<User>> {
    return apiService.get('/auth/me');
  }

  async updateProfile(data: Partial<User>): Promise<ApiResponse<User>> {
    return apiService.put('/auth/profile', data);
  }

  async changePassword(currentPassword: string, newPassword: string): Promise<ApiResponse> {
    return apiService.post('/auth/change-password', {
      currentPassword,
      newPassword,
    });
  }
}

export const authService = new AuthService();