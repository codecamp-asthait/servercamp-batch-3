export interface MerchantOrderSummary {
  id: string;
  orderNumber: string;
  status: string;
  total: number;
  itemCount: number;
  customerName: string;
  createdAt: string;
}

export interface MerchantOrder {
  id: string;
  orderNumber: string;
  status: string;
  billingAddress: AddressSnapshot;
  deliveryAddress: AddressSnapshot;
  subtotal: number;
  total: number;
  createdAt: string;
  customerName: string;
  items: OrderItem[];
}

export interface AddressSnapshot {
  street: string;
  city: string;
  district: string;
  postalCode: string;
  phone: string;
}

export interface OrderItem {
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
}

export interface UpdateOrderStatusRequest {
  newStatus: string;
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
