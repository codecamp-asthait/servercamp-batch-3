export interface CustomerProfileDto {
  id: string;
  firstName: string;
  lastName: string;
  phone: string | null;
  email: string;
}

export interface UpdateCustomerProfileData {
  firstName: string;
  lastName: string;
  phone: string | null;
}
