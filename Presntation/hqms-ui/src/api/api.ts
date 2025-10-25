import axios from "axios";
import type { AxiosInstance } from "axios";

// Auth Service
export const authApi: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_AUTH_API_BASE_URL,
  headers: { 'Content-Type': 'application/json' }
});

// Queue Service
export const queueApi: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_QUEUE_BASE_URL,
  headers: { 'Content-Type': 'application/json' }
});

// Patient Service
export const appointmentApi: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_APPOINTMENT_BASE_URL,
  headers: { 'Content-Type': 'application/json' }
});

// Attach token interceptor for all services
const apis: AxiosInstance[] = [authApi, queueApi, appointmentApi];

apis.forEach(api => {
  api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });
});

// Optional: export a default object if some components import 'api'
const api = { authApi, queueApi, appointmentApi };
export default api;
