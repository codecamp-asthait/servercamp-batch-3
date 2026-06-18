export interface CustomerLoginRequest {
  email: string;
  password: string;
}

export interface CustomerRegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phone: string;
}

export interface CustomerAuthResponse {
  token: string;
  email: string;
}
