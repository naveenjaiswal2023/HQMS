import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "@/contexts/AuthContext";
import ProtectedRoute from "@/components/ProtectedRoute";
import Navbar from "@/components/layout/Navbar";
import Auth from "./pages/Auth";
import Dashboard from "./pages/Dashboard";
import Patients from "./pages/Patients";
import Queue from "./pages/Queue";
import Appointments from "./pages/Appointments";
import Doctors from "./pages/Doctors"
import Payments from "./pages/Payments";
import Pod from "./pages/Pod";
import Nursing from "./pages/Nursing";
import Unauthorized from "./pages/Unauthorized";
import NotFound from "./pages/NotFound";
import PaymentCheckout from '@/components/forms/PaymentCheckout'; 

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <AuthProvider>
        <Toaster />
        <Sonner />
        <BrowserRouter>
          <div className="min-h-screen bg-background">
            <Navbar />
            <Routes>
              <Route path="/" element={<Auth />} />
              <Route path="/auth" element={<Auth />} />
              <Route path="/unauthorized" element={<Unauthorized />} />
              
              <Route path="/dashboard" element={
                <ProtectedRoute>
                  <Dashboard />
                </ProtectedRoute>
              } />
              
              <Route path="/patients" element={
                <ProtectedRoute allowedRoles={['admin', 'doctor', 'receptionist']}>
                  <Patients />
                </ProtectedRoute>
              } />
              
              <Route path="/queue" element={
                <ProtectedRoute>
                  <Queue />
                </ProtectedRoute>
              } />
              
              <Route path="/appointments" element={
                <ProtectedRoute>
                  <Appointments />
                </ProtectedRoute>
              } />
              
              <Route path="/payments" element={
                <ProtectedRoute>
                  <Payments />
                </ProtectedRoute>
              } />

              <Route path="/pod" element={
                <ProtectedRoute allowedRoles={['admin', 'doctor']}>
                  <Pod />
                </ProtectedRoute>
              } />

              <Route path="/nursing" element={
                <ProtectedRoute allowedRoles={['admin', 'doctor']}>
                  <Nursing />
                </ProtectedRoute>
              } />
              
              <Route path="/doctors" element={
                <ProtectedRoute allowedRoles={['admin']}>
                  <Doctors />
                </ProtectedRoute>

                // <ProtectedRoute allowedRoles={['admin']}>
                //   <div className="p-6 text-center">
                //     <h1 className="text-2xl font-bold mb-4">Doctor Management</h1>
                //     <p className="text-muted-foreground">Coming soon...</p>
                //   </div>
                // </ProtectedRoute>
              } />
              
              {/* ADD ALL CUSTOM ROUTES ABOVE THE CATCH-ALL "*" ROUTE */}
              <Route path="*" element={<NotFound />} />

              <Route path="/payment-checkout" element={
                <ProtectedRoute>
                  <PaymentCheckout />
                </ProtectedRoute>
              } />


            </Routes>
          </div>
        </BrowserRouter>
      </AuthProvider>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
