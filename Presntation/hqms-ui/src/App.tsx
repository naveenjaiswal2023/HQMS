// src/App.tsx

import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import AppNavbar from './components/common/Navbar';
import LoginPage from './components/auth/LoginPage';
import DoctorDashboard from './components/doctor/DoctorDashboard';
import PodAdminDashboard from './components/pod/PodDashboard';
import PatientDashboard from './components/patient/PatientDashboard';
import ProtectedRoute from './components/common/ProtectedRoute';

const App: React.FC = () => {
  return (
    <Router>
      <AppNavbar />

      <Routes>
        {/* 🔐 Default route goes to login */}
        <Route path="/" element={<Navigate to="/login" replace />} />

        {/* 🔑 Public route */}
        <Route path="/login" element={<LoginPage />} />

        {/* 👨‍⚕️ Doctor Dashboard */}
        <Route
          path="/doctor"
          element={
            <ProtectedRoute role="Doctor">
              <DoctorDashboard />
            </ProtectedRoute>
          }
        />

        {/* 🧑‍💼 POD Admin Dashboard */}
        <Route
          path="/pod-admin"
          element={
            <ProtectedRoute role="POD">
              <PodAdminDashboard />
            </ProtectedRoute>
          }
        />

        {/* 🧑‍⚕️ Patient Dashboard */}
        <Route
          path="/patient"
          element={
            <ProtectedRoute role="Patient">
              <PatientDashboard />
            </ProtectedRoute>
          }
        />

        {/* 🌐 Fallback for unmatched routes */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </Router>
  );
};

export default App;
