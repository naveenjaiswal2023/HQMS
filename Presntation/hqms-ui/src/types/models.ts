// src/types/models.ts

// Status of a patient in the queue
export type QueueStatus = 'Waiting' | 'Called' | 'Completed';

// Represents a patient in the queue
export interface Patient {
  patientId: string;
  token: string;
  name: string;
  status: QueueStatus;
  appointmentTime: string;
  doctorName?: string;
  departmentName?: string;
  estimatedWaitTime?: number; // in minutes
}

// Summary statistics for a doctor's queue
export interface QueueStats {
  total: number;
  waiting: number;
  completed: number;
}

// Doctor entity
export interface Doctor {
  id: string;
  name: string;
  departmentId: string;
}

// Department entity
export interface Department {
  id: string;
  name: string;
}

// Display-friendly info about the patient's queue status
export interface QueueInfo {
  token: string;
  doctorName: string;
  departmentName: string;
  estimatedWaitTime: number;
  status: QueueStatus;
}
