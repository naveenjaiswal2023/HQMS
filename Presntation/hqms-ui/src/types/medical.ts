export interface Patient {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  gender: 'male' | 'female' | 'other';
  address: string;
  emergencyContact: {
    name: string;
    phone: string;
    relationship: string;
  };
  medicalHistory?: string;
  allergies?: string;
  currentMedications?: string;
  insuranceNumber?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Doctor {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  specialization: string;
  licenseNumber: string;
  department: string;
  workingHours: {
    start: string;
    end: string;
    days: string[];
  };
  isAvailable: boolean;
  avatar?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Appointment {
  id: string;
  patientId: string;
  doctorId: string;
  patient?: Patient;
  doctor?: Doctor;
  scheduledDate: string;
  scheduledTime: string;
  duration: number; // in minutes
  type: 'consultation' | 'follow-up' | 'emergency' | 'routine';
  status: 'scheduled' | 'confirmed' | 'in-progress' | 'completed' | 'cancelled' | 'no-show';
  reason: string;
  notes?: string;
  prescription?: string;
  diagnosis?: string;
  followUpRequired: boolean;
  followUpDate?: string;
  createdAt: string;
  updatedAt: string;
}

export interface QueueItem {
  id: string;
  patientId: string;
  doctorId: string;
  appointmentId?: string;
  patient: Patient;
  doctor: Doctor;
  queueNumber: number;
  priority: 'low' | 'normal' | 'high' | 'emergency';
  status: 'waiting' | 'called' | 'in-consultation' | 'completed' | 'skipped';
  estimatedWaitTime: number; // in minutes
  actualWaitTime?: number;
  joinedAt: string;
  calledAt?: string;
  completedAt?: string;
  notes?: string;
}

export interface Payment {
  id: string;
  patientId: string;
  appointmentId?: string;
  amount: number;
  currency: string;
  status: 'pending' | 'completed' | 'failed' | 'refunded';
  paymentMethod: 'stripe' | 'razorpay' | 'cash' | 'insurance';
  transactionId?: string;
  description: string;
  dueDate?: string;
  paidAt?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Notification {
  id: string;
  userId: string;
  type: 'appointment_reminder' | 'queue_update' | 'payment_due' | 'prescription_ready' | 'general';
  title: string;
  message: string;
  isRead: boolean;
  priority: 'low' | 'normal' | 'high';
  actionUrl?: string;
  createdAt: string;
}