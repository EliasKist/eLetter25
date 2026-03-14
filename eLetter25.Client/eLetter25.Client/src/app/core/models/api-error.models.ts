export interface ApiError {
  code: string;
  message: string;
}

export interface ApiErrorResponse {
  errors: ApiError[];
}

