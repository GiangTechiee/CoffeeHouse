export interface ApiResponse<T> {
    success: boolean;
    data: T;
    error?: {
        code: string;
        message: string;
        details?: any;
    };
    metadata?: {
        timestamp: string;
        requestId?: string;
    };
    // Legacy support
    message?: string;
    errorCode?: string;
    errors?: any;
}

export interface PaginatedResponse<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}


export interface Category {
    id: number;
    name: string;
    description?: string;
    image?: string;
    icon?: string;
}

export interface Product {
    id: number;
    name: string;
    price: number;
    description?: string;
    imageUrl?: string;
    image?: string; // Legacy support or alias
    categoryId: number;
    categoryName?: string;
    isFeatured?: boolean;
    size?: string[];
    toppings?: string[];
}

export interface User {
    id: string;
    name: string;
    email: string;
    role: string;
}

export interface CartItem {
    id: string;
    productId: string;
    name: string;
    price: number;
    image: string;
    quantity: number;
    size?: string;
    toppings?: string[];
}

export interface Order {
    id: string;
    orderDate: string;
    totalAmount: number;
    status: string;
    items: OrderItem[];
}

export interface OrderItem {
    id: string;
    productId: string;
    name: string;
    price: number;
    quantity: number;
}

export interface CafeStore {
    id: number;
    storeName: string;
    address: string;
    phoneNumber: string;
    openingTime: string;
    closingTime: string;
    status: string;
}

export interface Customer {
    customerId: number;
    customerName: string;
    phoneNumber: string;
    address: string;
}
