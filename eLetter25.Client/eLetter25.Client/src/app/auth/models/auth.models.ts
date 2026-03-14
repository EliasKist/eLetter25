export type { ApiError, ApiErrorResponse } from '../../core/models/api-error.models';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  enableNotifications: boolean;
}

export interface RegisterResponse {
  userId: string;
  message: string;
}
