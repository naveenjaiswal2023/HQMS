import React, { useState } from 'react';
import { toast } from 'sonner';
import { NewAppointmentDialog } from '@/components/forms/NewAppointmentDialog';
import { PatientDetailsDialog } from '@/components/forms/PatientDetailsDialog';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { useAuth } from '@/contexts/AuthContext';
import { 
  Calendar, 
  Clock, 
  User, 
  Search, 
  Plus,
  Filter,
  CheckCircle,
  XCircle,
  AlertCircle
} from 'lucide-react';

const Appointments = () => {
  const { user } = useAuth();
  const [searchTerm, setSearchTerm] = useState('');
  const [showNewAppointmentDialog, setShowNewAppointmentDialog] = useState(false);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);
  const [selectedAppointmentId, setSelectedAppointmentId] = useState<string | null>(null);

  const handleNewAppointment = () => {
    setShowNewAppointmentDialog(true);
  };

  const handleFilterAppointments = () => {
    toast.info('Opening appointment filters...');
    // Show filter options modal
  };

  const handleViewDetails = (appointmentId: string) => {
    setSelectedAppointmentId(appointmentId);
    setShowDetailsDialog(true);
  };

  const handleManageAppointment = (appointmentId: string) => {
    toast.info(`Managing appointment ${appointmentId}...`);
    // Show manage options (reschedule, cancel, etc.)
  };

  // Mock appointments data
  const appointments = [
    {
      id: '1',
      patientName: 'John Smith',
      doctorName: 'Dr. Sarah Wilson',
      date: '2024-09-07',
      time: '09:00',
      status: 'confirmed',
      type: 'Consultation',
      department: 'Cardiology'
    },
    {
      id: '2',
      patientName: 'Emily Johnson',
      doctorName: 'Dr. Michael Brown',
      date: '2024-09-07',
      time: '10:30',
      status: 'pending',
      type: 'Follow-up',
      department: 'Neurology'
    },
    {
      id: '3',
      patientName: 'Robert Davis',
      doctorName: 'Dr. Lisa Chen',
      date: '2024-09-07',
      time: '14:00',
      status: 'completed',
      type: 'Surgery Consultation',
      department: 'Orthopedics'
    },
    {
      id: '4',
      patientName: 'Maria Garcia',
      doctorName: 'Dr. James Wilson',
      date: '2024-09-08',
      time: '11:00',
      status: 'cancelled',
      type: 'Check-up',
      department: 'General Medicine'
    }
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'confirmed': return 'bg-success text-success-foreground';
      case 'pending': return 'bg-warning text-warning-foreground';
      case 'completed': return 'bg-primary text-primary-foreground';
      case 'cancelled': return 'bg-destructive text-destructive-foreground';
      default: return 'bg-secondary text-secondary-foreground';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'confirmed': return <CheckCircle className="w-4 h-4" />;
      case 'pending': return <AlertCircle className="w-4 h-4" />;
      case 'completed': return <CheckCircle className="w-4 h-4" />;
      case 'cancelled': return <XCircle className="w-4 h-4" />;
      default: return <Clock className="w-4 h-4" />;
    }
  };

  const filteredAppointments = appointments.filter(appointment =>
    appointment.patientName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    appointment.doctorName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    appointment.department.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const todayAppointments = filteredAppointments.filter(apt => apt.date === '2024-09-07');
  const upcomingAppointments = filteredAppointments.filter(apt => apt.date > '2024-09-07');

  return (
    <div className="container mx-auto p-6 space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Appointments</h1>
          <p className="text-muted-foreground">
            {user?.role === 'patient' ? 'Your upcoming appointments' : 'Manage all appointments'}
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
            <Input
              placeholder="Search appointments..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 w-64"
            />
          </div>
          <Button variant="outline" size="icon" onClick={handleFilterAppointments}>
            <Filter className="w-4 h-4" />
          </Button>
          {(user?.role === 'admin' || user?.role === 'receptionist') && (
            <Button onClick={handleNewAppointment}>
              <Plus className="w-4 h-4 mr-2" />
              New Appointment
            </Button>
          )}
        </div>
      </div>

      {/* Today's Appointments */}
      <Card className="medical-card">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Calendar className="w-5 h-5" />
            Today's Appointments
          </CardTitle>
          <CardDescription>
            {todayAppointments.length} appointments scheduled for today
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {todayAppointments.map((appointment) => (
              <div key={appointment.id} className="flex items-center justify-between p-4 border border-border rounded-lg hover:bg-accent/50 transition-colors">
                <div className="flex items-center gap-4">
                  <div className="flex items-center gap-2">
                    <Clock className="w-4 h-4 text-muted-foreground" />
                    <span className="font-medium">{appointment.time}</span>
                  </div>
                  <div>
                    <p className="font-medium text-foreground">{appointment.patientName}</p>
                    <p className="text-sm text-muted-foreground">{appointment.doctorName} • {appointment.department}</p>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <Badge className={getStatusColor(appointment.status)}>
                    {getStatusIcon(appointment.status)}
                    <span className="ml-1 capitalize">{appointment.status}</span>
                  </Badge>
                  <Button variant="outline" size="sm" onClick={() => handleViewDetails(appointment.id)}>
                    View Details
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Upcoming Appointments */}
      <Card className="medical-card">
        <CardHeader>
          <CardTitle>Upcoming Appointments</CardTitle>
          <CardDescription>
            {upcomingAppointments.length} upcoming appointments
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {upcomingAppointments.map((appointment) => (
              <div key={appointment.id} className="flex items-center justify-between p-4 border border-border rounded-lg hover:bg-accent/50 transition-colors">
                <div className="flex items-center gap-4">
                  <div className="flex flex-col items-center">
                    <span className="text-sm text-muted-foreground">{appointment.date}</span>
                    <span className="font-medium">{appointment.time}</span>
                  </div>
                  <div>
                    <p className="font-medium text-foreground">{appointment.patientName}</p>
                    <p className="text-sm text-muted-foreground">{appointment.doctorName} • {appointment.department}</p>
                    <p className="text-xs text-muted-foreground">{appointment.type}</p>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <Badge className={getStatusColor(appointment.status)}>
                    {getStatusIcon(appointment.status)}
                    <span className="ml-1 capitalize">{appointment.status}</span>
                  </Badge>
                  <Button variant="outline" size="sm" onClick={() => handleManageAppointment(appointment.id)}>
                    Manage
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Dialogs */}
      <NewAppointmentDialog 
        open={showNewAppointmentDialog} 
        onOpenChange={setShowNewAppointmentDialog} 
      />
      <PatientDetailsDialog 
        open={showDetailsDialog} 
        onOpenChange={setShowDetailsDialog}
        patientId={selectedAppointmentId}
      />
    </div>
  );
};

export default Appointments;