import React from 'react';
import { useAuth } from '@/contexts/AuthContext';
import AdminDashboard from '@/components/dashboard/AdminDashboard';
import DoctorDashboard from '@/components/dashboard/DoctorDashboard';
import PatientDashboard from '@/components/dashboard/PatientDashboard';
import ReceptionistDashboard from '@/components/dashboard/ReceptionistDashboard';

const Dashboard = () => {
  const { user } = useAuth();

  if (!user) return null;

  const renderDashboard = () => {
    switch (user.role) {
      case 'admin':
        return <AdminDashboard />;
      case 'doctor':
        return <DoctorDashboard />;
      case 'patient':
        return <PatientDashboard />;
      case 'receptionist':
        return <ReceptionistDashboard />;
      default:
        return <PatientDashboard />;
    }
  };

  return (
    <div className="min-h-screen bg-background">
      {renderDashboard()}
    </div>
  );
};

export default Dashboard;