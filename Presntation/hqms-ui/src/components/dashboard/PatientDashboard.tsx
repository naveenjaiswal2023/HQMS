import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';
import { NewAppointmentDialog } from '@/components/forms/NewAppointmentDialog';
import { PatientDetailsDialog } from '@/components/forms/PatientDetailsDialog';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Progress } from '@/components/ui/progress';
import { 
  Calendar, 
  Clock, 
  MapPin,
  CreditCard,
  Bell,
  Heart,
  Activity,
  FileText
} from 'lucide-react';

const PatientDashboard = () => {
  const navigate = useNavigate();
  const [showAppointmentDialog, setShowAppointmentDialog] = useState(false);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);
  const [selectedAppointmentId, setSelectedAppointmentId] = useState<number | null>(null);

  const handleBookAppointment = () => {
    setShowAppointmentDialog(true);
  };

  const handleNotifications = () => {
    toast.info('Notifications panel opened');
    // Show notifications modal
  };

  const handleReschedule = (appointmentId: number) => {
    toast.info(`Rescheduling appointment ${appointmentId}...`);
    setSelectedAppointmentId(appointmentId);
    setShowAppointmentDialog(true);
  };

  const handleViewDetails = (appointmentId: number) => {
    setSelectedAppointmentId(appointmentId);
    setShowDetailsDialog(true);
  };

  const handlePayNow = (paymentId: number) => {
    navigate('/payments');
    toast.success(`Redirecting to payment for bill ${paymentId}...`);
  };

  const handleViewMedicalRecords = () => {
    toast.info('Opening medical records...');
    // Navigate to medical records or show modal
  };

  const handleViewBills = () => {
    navigate('/payments');
    toast.success('Redirecting to billing...');
  };
  const queueStatus = {
    position: 3,
    total: 12,
    estimatedWait: 25,
    doctor: 'Dr. Sarah Johnson',
    department: 'Cardiology',
    appointmentTime: '10:30 AM'
  };

  const upcomingAppointments = [
    {
      id: 1,
      doctor: 'Dr. Sarah Johnson',
      department: 'Cardiology',
      date: 'Today',
      time: '10:30 AM',
      type: 'Follow-up',
      status: 'confirmed'
    },
    {
      id: 2,
      doctor: 'Dr. Michael Chen',
      department: 'Orthopedics',
      date: 'March 15',
      time: '2:00 PM',
      type: 'Consultation',
      status: 'scheduled'
    },
    {
      id: 3,
      doctor: 'Dr. Emily Wilson',
      department: 'General Medicine',
      date: 'March 20',
      time: '9:00 AM',
      type: 'Annual Checkup',
      status: 'scheduled'
    }
  ];

  const recentPayments = [
    {
      id: 1,
      description: 'Consultation - Dr. Sarah Johnson',
      amount: '$150.00',
      date: 'March 10, 2024',
      status: 'paid'
    },
    {
      id: 2,
      description: 'Lab Tests - Blood Work',
      amount: '$85.00',
      date: 'March 8, 2024',
      status: 'paid'
    },
    {
      id: 3,
      description: 'X-Ray - Chest',
      amount: '$120.00',
      date: 'March 5, 2024',
      status: 'pending'
    }
  ];

  const healthMetrics = [
    { label: 'Blood Pressure', value: '120/80', status: 'normal', icon: Heart },
    { label: 'Heart Rate', value: '72 bpm', status: 'normal', icon: Activity },
    { label: 'Weight', value: '165 lbs', status: 'stable', icon: Activity },
  ];

  const progressPercentage = ((queueStatus.total - queueStatus.position) / queueStatus.total) * 100;

  return (
    <div className="p-6 space-y-6 animate-fade-in">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Welcome back, John</h1>
          <p className="text-muted-foreground">
            Your next appointment is today at {queueStatus.appointmentTime}
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" onClick={handleNotifications}>
            <Bell className="w-4 h-4 mr-2" />
            Notifications
          </Button>
          <Button variant="medical" size="sm" onClick={handleBookAppointment}>
            <Calendar className="w-4 h-4 mr-2" />
            Book Appointment
          </Button>
        </div>
      </div>

      {/* Queue Status - Prominent Card */}
      <Card className="medical-card border-primary/20 bg-gradient-to-r from-primary-light to-accent-light">
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-primary">
            <Clock className="w-5 h-5" />
            Current Queue Status
          </CardTitle>
          <CardDescription>
            Your position in the queue for today's appointment
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-2xl font-bold text-foreground">
                  Position #{queueStatus.position}
                </p>
                <p className="text-sm text-muted-foreground">
                  of {queueStatus.total} patients
                </p>
              </div>
              <div className="text-right">
                <p className="text-lg font-semibold text-warning">
                  ~{queueStatus.estimatedWait} min wait
                </p>
                <p className="text-sm text-muted-foreground">
                  Estimated time
                </p>
              </div>
            </div>
            
            <Progress value={progressPercentage} className="h-3" />
            
            <div className="flex items-center justify-between text-sm">
              <div className="flex items-center gap-2">
                <MapPin className="w-4 h-4 text-muted-foreground" />
                <span>{queueStatus.department}</span>
              </div>
              <span className="font-medium">{queueStatus.doctor}</span>
            </div>
          </div>
        </CardContent>
      </Card>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Upcoming Appointments */}
        <Card className="lg:col-span-2 medical-card">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Calendar className="w-5 h-5" />
              Upcoming Appointments
            </CardTitle>
            <CardDescription>
              Your scheduled medical appointments
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {upcomingAppointments.map((appointment) => (
                <div key={appointment.id} className="flex items-center justify-between p-4 bg-muted rounded-lg hover:bg-muted/80 transition-medical">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <h4 className="font-medium text-foreground">{appointment.doctor}</h4>
                      <Badge variant={appointment.status === 'confirmed' ? 'default' : 'secondary'}>
                        {appointment.status}
                      </Badge>
                    </div>
                    <p className="text-sm text-muted-foreground mb-1">
                      {appointment.department} • {appointment.type}
                    </p>
                    <p className="text-sm text-muted-foreground">
                      {appointment.date} at {appointment.time}
                    </p>
                  </div>
                  <div className="flex gap-2">
                    <Button variant="ghost" size="sm" onClick={() => handleReschedule(appointment.id)}>
                      Reschedule
                    </Button>
                    <Button variant="outline" size="sm" onClick={() => handleViewDetails(appointment.id)}>
                      Details
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Health Summary */}
        <Card className="medical-card">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Heart className="w-5 h-5" />
              Health Summary
            </CardTitle>
            <CardDescription>
              Your recent health metrics
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {healthMetrics.map((metric, index) => {
                const Icon = metric.icon;
                return (
                  <div key={index} className="flex items-center justify-between p-3 bg-muted rounded-lg">
                    <div className="flex items-center gap-3">
                      <div className="p-2 bg-primary-light rounded-full">
                        <Icon className="w-4 h-4 text-primary" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-foreground">
                          {metric.label}
                        </p>
                        <p className="text-lg font-bold text-foreground">
                          {metric.value}
                        </p>
                      </div>
                    </div>
                    <Badge variant="secondary" className="bg-success/10 text-success">
                      {metric.status}
                    </Badge>
                  </div>
                );
              })}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Recent Payments */}
      <Card className="medical-card">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <CreditCard className="w-5 h-5" />
            Recent Payments
          </CardTitle>
          <CardDescription>
            Your payment history and pending bills
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {recentPayments.map((payment) => (
              <div key={payment.id} className="flex items-center justify-between p-4 bg-muted rounded-lg">
                <div className="flex-1">
                  <h4 className="font-medium text-foreground">{payment.description}</h4>
                  <p className="text-sm text-muted-foreground">{payment.date}</p>
                </div>
                <div className="flex items-center gap-4">
                  <span className="text-lg font-semibold text-foreground">
                    {payment.amount}
                  </span>
                  <Badge 
                    variant={payment.status === 'paid' ? 'default' : 'secondary'}
                    className={payment.status === 'paid' ? 'bg-success text-success-foreground' : 'bg-warning text-warning-foreground'}
                  >
                    {payment.status}
                  </Badge>
                  {payment.status === 'pending' && (
                    <Button variant="outline" size="sm" onClick={() => handlePayNow(payment.id)}>
                      Pay Now
                    </Button>
                  )}
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Button variant="outline" className="h-20 flex-col" onClick={handleBookAppointment}>
          <Calendar className="w-6 h-6 mb-2" />
          Book Appointment
        </Button>
        <Button variant="outline" className="h-20 flex-col" onClick={handleViewMedicalRecords}>
          <FileText className="w-6 h-6 mb-2" />
          Medical Records
        </Button>
        <Button variant="outline" className="h-20 flex-col" onClick={handleViewBills}>
          <CreditCard className="w-6 h-6 mb-2" />
          View Bills
        </Button>
        <Button variant="outline" className="h-20 flex-col" onClick={handleNotifications}>
          <Bell className="w-6 h-6 mb-2" />
          Notifications
        </Button>
      </div>

      {/* Dialogs */}
      <NewAppointmentDialog 
        open={showAppointmentDialog} 
        onOpenChange={setShowAppointmentDialog} 
      />
      <PatientDetailsDialog 
        open={showDetailsDialog} 
        onOpenChange={setShowDetailsDialog}
        patientId={selectedAppointmentId}
      />
    </div>
  );
};

export default PatientDashboard;