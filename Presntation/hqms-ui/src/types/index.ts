// HQMS Type Definitions

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  profileImage?: string;
  phone?: string;
  createdAt: string;
  isActive: boolean;
}

export type UserRole = 'admin' | 'doctor' | 'patient' | 'pod';

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  phone?: string;
}

export interface Patient {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  gender: 'male' | 'female' | 'other';
  address: {
    street: string;
    city: string;
    state: string;
    zipCode: string;
  };
  emergencyContact: {
    name: string;
    phone: string;
    relation: string;
  };
  medicalHistory?: string;
  allergies?: string[];
  medications?: string[];
  insuranceInfo?: {
    provider: string;
    policyNumber: string;
  };
  createdAt: string;
  updatedAt: string;
}

export interface Doctor {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  specialization: string;
  licenseNumber: string;
  availability: DoctorAvailability[];
  isAvailable: boolean;
}

export interface DoctorAvailability {
  dayOfWeek: number; // 0-6 (Sunday to Saturday)
  startTime: string;
  endTime: string;
}

export interface Appointment {
  id: string;
  patientId: string;
  doctorId: string;
  patient?: Patient;
  doctor?: Doctor;
  appointmentDate: string;
  startTime: string;
  endTime: string;
  status: AppointmentStatus;
  type: AppointmentType;
  notes?: string;
  symptoms?: string;
  diagnosis?: string;
  createdAt: string;
  updatedAt: string;
}

export type AppointmentStatus = 'scheduled' | 'confirmed' | 'in-progress' | 'completed' | 'cancelled' | 'no-show';
export type AppointmentType = 'consultation' | 'follow-up' | 'emergency' | 'routine-checkup' | 'specialist';

export interface QueueItem {
  id: string;
  appointmentId: string;
  patientName: string;
  doctorId: string;
  estimatedWaitTime: number; // minutes
  queuePosition: number;
  status: QueueStatus;
  checkInTime: string;
}

export type QueueStatus = 'waiting' | 'called' | 'in-session' | 'completed';

export interface Notification {
  id: string;
  userId: string;
  title: string;
  message: string;
  type: NotificationType;
  isRead: boolean;
  createdAt: string;
  data?: any; // Additional data for the notification
}

export type NotificationType = 'appointment' | 'payment' | 'system' | 'reminder' | 'alert';

export interface Payment {
  id: string;
  patientId: string;
  appointmentId?: string;
  amount: number;
  currency: string;
  status: PaymentStatus;
  paymentMethod: PaymentMethod;
  transactionId?: string;
  description: string;
  dueDate?: string;
  paidAt?: string;
  createdAt: string;
}

export type PaymentStatus = 'pending' | 'paid' | 'failed' | 'refunded' | 'cancelled';
export type PaymentMethod = 'cash' | 'card' | 'insurance' | 'bank-transfer' | 'digital-wallet';

export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
  pagination?: {
    page: number;
    limit: number;
    total: number;
    totalPages: number;
  };
}

export interface ApiError {
  message: string;
  status?: number;
  errors?: string[];
}

// Dashboard Statistics
export interface DashboardStats {
  totalPatients: number;
  todayAppointments: number;
  pendingPayments: number;
  activeQueue: number;
  monthlyRevenue: number;
  patientGrowth: number;
  appointmentStats: {
    completed: number;
    cancelled: number;
    noShow: number;
  };
}