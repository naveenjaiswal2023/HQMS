import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';
import { NewPatientDialog } from '@/components/forms/NewPatientDialog';
import { NewAppointmentDialog } from '@/components/forms/NewAppointmentDialog';
import { PatientDetailsDialog } from '@/components/forms/PatientDetailsDialog';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { 
  Users, 
  Calendar, 
  Clock, 
  Phone,
  UserPlus,
  Activity,
  CreditCard,
  CheckCircle
} from 'lucide-react';

const ReceptionistDashboard = () => {
  const navigate = useNavigate();
  const [showNewPatientDialog, setShowNewPatientDialog] = useState(false);
  const [showAppointmentDialog, setShowAppointmentDialog] = useState(false);
  const [showPatientDetailsDialog, setShowPatientDetailsDialog] = useState(false);
  const [selectedPatientId, setSelectedPatientId] = useState<number | null>(null);

  const handleCallPatient = (patientName?: string) => {
    if (patientName) {
      toast.success(`Calling ${patientName}...`);
    } else {
      toast.info('Call patient feature activated');
    }
  };

  const handleNewPatient = () => {
    setShowNewPatientDialog(true);
  };

  const handleViewPatientDetails = (patientId: number) => {
    setSelectedPatientId(patientId);
    setShowPatientDetailsDialog(true);
  };

  const handleCheckIn = () => {
    toast.success('Patient check-in initiated');
    // Handle check-in logic
  };

  const handleScheduleAppointment = () => {
    setShowAppointmentDialog(true);
  };

  const handleProcessPayment = () => {
    navigate('/payments');
    toast.success('Redirecting to payment processing...');
  };
  const todayStats = [
    {
      title: 'Check-ins Today',
      value: '47',
      change: '+8',
      icon: CheckCircle,
      color: 'text-success',
    },
    {
      title: 'New Registrations',
      value: '12',
      change: '+3',
      icon: UserPlus,
      color: 'text-primary',
    },
    {
      title: 'Queue Length',
      value: '23',
      change: '-5',
      icon: Activity,
      color: 'text-warning',
    },
    {
      title: 'Payments Processed',
      value: '$2,840',
      change: '+$320',
      icon: CreditCard,
      color: 'text-secondary',
    },
  ];

  const waitingPatients = [
    {
      id: 1,
      name: 'Sarah Johnson',
      appointment: '10:30 AM',
      doctor: 'Dr. Smith',
      department: 'Cardiology',
      waitTime: '15 min',
      status: 'waiting',
      priority: 'normal'
    },
    {
      id: 2,
      name: 'Michael Chen',
      appointment: '10:45 AM',
      doctor: 'Dr. Wilson',
      department: 'Orthopedics',
      waitTime: '8 min',
      status: 'called',
      priority: 'normal'
    },
    {
      id: 3,
      name: 'Emma Davis',
      appointment: 'Walk-in',
      doctor: 'Dr. Johnson',
      department: 'Emergency',
      waitTime: '3 min',
      status: 'urgent',
      priority: 'high'
    },
    {
      id: 4,
      name: 'Robert Wilson',
      appointment: '11:00 AM',
      doctor: 'Dr. Brown',
      department: 'General',
      waitTime: '22 min',
      status: 'waiting',
      priority: 'normal'
    },
  ];

  const recentActivities = [
    {
      id: 1,
      action: 'Patient checked in',
      patient: 'Sarah Johnson',
      time: '2 min ago',
      type: 'checkin'
    },
    {
      id: 2,
      action: 'New patient registered',
      patient: 'Michael Chen',
      time: '5 min ago',
      type: 'registration'
    },
    {
      id: 3,
      action: 'Payment processed',
      patient: 'Emma Davis',
      time: '8 min ago',
      type: 'payment'
    },
    {
      id: 4,
      action: 'Appointment rescheduled',
      patient: 'Robert Wilson',
      time: '12 min ago',
      type: 'appointment'
    },
  ];

  const todaysAppointments = [
    {
      time: '9:00 AM',
      patient: 'Alice Brown',
      doctor: 'Dr. Smith',
      type: 'Consultation',
      status: 'completed'
    },
    {
      time: '9:30 AM',
      patient: 'John Martinez',
      doctor: 'Dr. Wilson',
      type: 'Follow-up',
      status: 'completed'
    },
    {
      time: '10:00 AM',
      patient: 'Linda Garcia',
      doctor: 'Dr. Johnson',
      type: 'Routine',
      status: 'in-progress'
    },
    {
      time: '10:30 AM',
      patient: 'Sarah Johnson',
      doctor: 'Dr. Smith',
      type: 'Consultation',
      status: 'waiting'
    },
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'waiting': return 'bg-yellow-100 text-yellow-800';
      case 'called': return 'bg-blue-100 text-blue-800';
      case 'urgent': return 'bg-red-100 text-red-800';
      case 'completed': return 'bg-green-100 text-green-800';
      case 'in-progress': return 'bg-purple-100 text-purple-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getPriorityIcon = (priority: string) => {
    return priority === 'high' ? '🔴' : priority === 'medium' ? '🟡' : '🟢';
  };

  return (
    <div className="p-6 space-y-6 animate-fade-in">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Reception Dashboard</h1>
          <p className="text-muted-foreground">
            Manage patient flow and front desk operations
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" onClick={() => handleCallPatient()}>
            <Phone className="w-4 h-4 mr-2" />
            Call Patient
          </Button>
          <Button variant="medical" size="sm" onClick={handleNewPatient}>
            <UserPlus className="w-4 h-4 mr-2" />
            New Patient
          </Button>
        </div>
      </div>

      {/* Today's Stats */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {todayStats.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <Card key={index} className="medical-card hover:shadow-hover transition-medical">
              <CardContent className="p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      {stat.title}
                    </p>
                    <div className="flex items-center gap-2">
                      <p className="text-2xl font-bold text-foreground">
                        {stat.value}
                      </p>
                      <span className="text-xs text-muted-foreground">
                        {stat.change}
                      </span>
                    </div>
                  </div>
                  <Icon className={`w-8 h-8 ${stat.color}`} />
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Waiting Queue */}
        <Card className="medical-card">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Activity className="w-5 h-5" />
              Current Queue ({waitingPatients.length})
            </CardTitle>
            <CardDescription>
              Patients currently waiting or being seen
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {waitingPatients.map((patient) => (
                <div key={patient.id} className="flex items-center justify-between p-3 bg-muted rounded-lg hover:bg-muted/80 transition-medical">
                  <div className="flex items-center gap-3">
                    <span className="text-lg">{getPriorityIcon(patient.priority)}</span>
                    <div>
                      <div className="flex items-center gap-2">
                        <h4 className="font-medium text-foreground">{patient.name}</h4>
                        <Badge className={getStatusColor(patient.status)}>
                          {patient.status}
                        </Badge>
                      </div>
                      <p className="text-sm text-muted-foreground">
                        {patient.doctor} • {patient.department}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        {patient.appointment} • Waiting: {patient.waitTime}
                      </p>
                    </div>
                  </div>
                  <div className="flex gap-1">
                    <Button variant="ghost" size="sm" onClick={() => handleCallPatient(patient.name)}>
                      Call
                    </Button>
                    <Button variant="outline" size="sm" onClick={() => handleViewPatientDetails(patient.id)}>
                      Details
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Today's Schedule */}
        <Card className="medical-card">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Calendar className="w-5 h-5" />
              Today's Appointments
            </CardTitle>
            <CardDescription>
              Schedule overview for today
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {todaysAppointments.map((appointment, index) => (
                <div key={index} className="flex items-center justify-between p-3 bg-muted rounded-lg">
                  <div className="flex items-center gap-3">
                    <div className="text-center">
                      <p className="text-sm font-medium text-foreground">
                        {appointment.time}
                      </p>
                    </div>
                    <div>
                      <h4 className="font-medium text-foreground">{appointment.patient}</h4>
                      <p className="text-sm text-muted-foreground">
                        {appointment.doctor} • {appointment.type}
                      </p>
                    </div>
                  </div>
                  <Badge className={getStatusColor(appointment.status)}>
                    {appointment.status}
                  </Badge>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Recent Activities */}
      <Card className="medical-card">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Clock className="w-5 h-5" />
            Recent Activities
          </CardTitle>
          <CardDescription>
            Latest front desk activities and updates
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {recentActivities.map((activity) => (
              <div key={activity.id} className="flex items-start gap-3 p-3 bg-muted rounded-lg">
                <div className={`w-2 h-2 rounded-full mt-2 ${
                  activity.type === 'checkin' ? 'bg-success' :
                  activity.type === 'registration' ? 'bg-primary' :
                  activity.type === 'payment' ? 'bg-secondary' : 'bg-warning'
                }`} />
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-foreground">
                    {activity.action}
                  </p>
                  <p className="text-sm text-muted-foreground">
                    {activity.patient} • {activity.time}
                  </p>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Button variant="medical" className="h-20 flex-col" onClick={handleNewPatient}>
          <UserPlus className="w-6 h-6 mb-2" />
          Register Patient
        </Button>
        <Button variant="outline" className="h-20 flex-col" onClick={handleCheckIn}>
          <CheckCircle className="w-6 h-6 mb-2" />
          Check In
        </Button>
        <Button variant="outline" className="h-20 flex-col" onClick={handleScheduleAppointment}>
          <Calendar className="w-6 h-6 mb-2" />
          Schedule Appointment
        </Button>
        <Button variant="outline" className="h-20 flex-col" onClick={handleProcessPayment}>
          <CreditCard className="w-6 h-6 mb-2" />
          Process Payment
        </Button>
      </div>

      {/* Dialogs */}
      <NewPatientDialog 
        open={showNewPatientDialog} 
        onOpenChange={setShowNewPatientDialog} 
      />
      <NewAppointmentDialog 
        open={showAppointmentDialog} 
        onOpenChange={setShowAppointmentDialog} 
      />
      <PatientDetailsDialog 
        open={showPatientDetailsDialog} 
        onOpenChange={setShowPatientDetailsDialog}
        patientId={selectedPatientId}
      />
    </div>
  );
};

export default ReceptionistDashboard;