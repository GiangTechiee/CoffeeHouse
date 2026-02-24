import axios from 'axios';

// Create axios instance with security defaults
const api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:7001/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true, // Important for HttpOnly cookies
});

// Request Interceptor for Auth
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Response Interceptor for Global Error Handling (Security & UX)
api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            // Unauthorized - clear token and redirect
            localStorage.removeItem('token');
            window.location.href = '/login';
        }

        // Log error securely (not in production)
        if (import.meta.env.DEV) {
            console.error('API Error:', error.response?.data || error.message);
        }

        return Promise.reject(error);
    }
);

export default api;
