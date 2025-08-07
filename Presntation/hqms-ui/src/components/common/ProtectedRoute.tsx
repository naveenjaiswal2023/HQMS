import React from 'react';
import { Navigate } from 'react-router-dom';

interface Props {
  children: React.ReactNode;
  role?: string;
}

const ProtectedRoute: React.FC<Props> = ({ children, role }) => {
  const token = localStorage.getItem('token');
  const userRole = localStorage.getItem('role');

  // 🔐 If no token, redirect to login
  if (!token) {
    return <Navigate to="/login" replace />;
  }

  // 🚫 If role is provided and doesn't match, deny access
  if (role && userRole !== role) {
    return <Navigate to="/login" replace />;
  }

  // ✅ Authorized
  return <>{children}</>;
};

export default ProtectedRoute;
