export interface OrderDto {
  id: string;
  orderNumber: string;
  status: "Pending" | "Confirmed" | "Cancelled";
  billingAddress: AddressSnapshotDto;
  deliveryAddress: AddressSnapshotDto;
  subtotal: number;
  total: number;
  createdAt: string;
  items: OrderItemDto[];
}

export interface OrderItemDto {
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
}

export interface AddressSnapshotDto {
  street: string;
  city: string;
  district: string;
  postalCode: string;
  phone: string;
}

export interface AddressDto {
  id: string;
  label: string;
  type: "Billing" | "Delivery";
  street: string;
  city: string;
  district: string;
  postalCode: string;
  phone: string;
  isDefault: boolean;
}

export interface OrderSummaryDto {
  id: string;
  orderNumber: string;
  status: "Pending" | "Confirmed" | "Cancelled";
  total: number;
  itemCount: number;
  createdAt: string;
}

export interface CreateAddressData {
  label: string;
  type: "Billing" | "Delivery";
  street: string;
  city: string;
  district: string;
  postalCode: string;
  phone: string;
  isDefault: boolean;
}

export interface UpdateAddressData {
  label: string;
  street: string;
  city: string;
  district: string;
  postalCode: string;
  phone: string;
}

export interface PlaceOrderRequest {
  billingAddressId: string;
  deliveryAddressId: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
