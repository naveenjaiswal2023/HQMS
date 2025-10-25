import React from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { 
  Calendar, 
  Users, 
  Clock, 
  Stethoscope,
  FileText,
  AlertCircle,
  CheckCircle2
} from 'lucide-react';
import doctorTablet from '@/assets/doctor-tablet.jpg';

const DoctorDashboard = () => {
  const todayStats = [
    {
      title: 'Today\'s Patients',
      value: '12',
      icon: Users,
      color: 'text-primary',
    },
    {
      title: 'Appointments',
      value: '8',
      icon: Calendar,
      color: 'text-secondary',
    },
    {
      title: 'Avg Consultation',
      value: '18 min',
      icon: Clock,
      color: 'text-accent',
    },
    {
      title: 'Queue Position',
      value: '3 waiting',
      icon: Stethoscope,
      color: 'text-warning',
    },
  ];

  const upcomingAppointments = [
    {
      id: 1,
      patient: 'Sarah Johnson',
      time: '10:30 AM',
      type: 'Follow-up',
      status: 'confirmed',
      reason: 'Post-surgery checkup'
    },
    {
      id: 2,
      patient: 'Michael Chen',
      time: '11:00 AM',
      type: 'Consultation',
      status: 'waiting',
      reason: 'Back pain assessment'
    },
    {
      id: 3,
      patient: 'Emma Wilson',
      time: '11:30 AM',
      type: 'Emergency',
      status: 'urgent',
      reason: 'Acute chest pain'
    },
    {
      id: 4,
      patient: 'Robert Davis',
      time: '2:00 PM',
      type: 'Routine',
      status: 'scheduled',
      reason: 'Annual physical exam'
    },
  ];

  const recentPatients = [
    {
      id: 1,
      name: 'Alice Brown',
      lastVisit: '2 days ago',
      condition: 'Hypertension',
      status: 'stable'
    },
    {
      id: 2,
      name: 'John Martinez',
      lastVisit: '1 week ago',
      condition: 'Diabetes',
      status: 'monitoring'
    },
    {
      id: 3,
      name: 'Linda Garcia',
      lastVisit: '3 days ago',
      condition: 'Recovery',
      status: 'improving'
    },
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'confirmed': return 'bg-success text-success-foreground';
      case 'waiting': return 'bg-warning text-warning-foreground';
      case 'urgent': return 'bg-destructive text-destructive-foreground';
      case 'scheduled': return 'bg-primary text-primary-foreground';
      default: return 'bg-muted text-muted-foreground';
    }
  };

  return (
    <div className="p-6 space-y-6 animate-fade-in">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Good morning, Dr. Smith</h1>
          <p className="text-muted-foreground">
            You have 8 appointments scheduled for today
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" size="sm">
            <FileText className="w-4 h-4 mr-2" />
            Notes
          </Button>
          <Button variant="medical" size="sm">
            <AlertCircle className="w-4 h-4 mr-2" />
            Urgent Cases
          </Button>
        </div>
      </div>

      {/* Quick Stats */}
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
                    <p className="text-2xl font-bold text-foreground">
                      {stat.value}
                    </p>
                  </div>
                  <Icon className={`w-8 h-8 ${stat.color}`} />
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Today's Schedule */}
        <Card className="medical-card">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Calendar className="w-5 h-5" />
              Today's Schedule
            </CardTitle>
            <CardDescription>
              Your appointments for today
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {upcomingAppointments.map((appointment) => (
                <div key={appointment.id} className="flex items-center justify-between p-4 bg-muted rounded-lg hover:bg-muted/80 transition-medical">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <h4 className="font-medium text-foreground">{appointment.patient}</h4>
                      <Badge className={getStatusColor(appointment.status)}>
                        {appointment.status}
                      </Badge>
                    </div>
                    <p className="text-sm text-muted-foreground mb-1">
                      {appointment.time} • {appointment.type}
                    </p>
                    <p className="text-sm text-muted-foreground">
                      {appointment.reason}
                    </p>
                  </div>
                  <Button variant="ghost" size="sm">
                    View
                  </Button>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Recent Patients */}
        <Card className="medical-card">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Users className="w-5 h-5" />
              Recent Patients
            </CardTitle>
            <CardDescription>
              Your recently seen patients
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {recentPatients.map((patient) => (
                <div key={patient.id} className="flex items-center justify-between p-4 bg-muted rounded-lg">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 bg-gradient-secondary rounded-full flex items-center justify-center">
                      <span className="text-white font-medium text-sm">
                        {patient.name.split(' ').map(n => n[0]).join('')}
                      </span>
                    </div>
                    <div>
                      <h4 className="font-medium text-foreground">{patient.name}</h4>
                      <p className="text-sm text-muted-foreground">
                        {patient.condition} • {patient.lastVisit}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    <CheckCircle2 className={`w-4 h-4 ${
                      patient.status === 'stable' ? 'text-success' :
                      patient.status === 'improving' ? 'text-primary' : 'text-warning'
                    }`} />
                    <span className="text-sm font-medium capitalize">{patient.status}</span>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Quick Actions */}
      <Card className="medical-card">
        <CardHeader>
          <CardTitle>Quick Actions</CardTitle>
          <CardDescription>
            Common tasks and medical tools
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col lg:flex-row gap-6">
            <div className="flex-1">
              <img
                src={doctorTablet}
                alt="Doctor using tablet"
                className="w-full h-48 object-cover rounded-lg shadow-card"
              />
            </div>
            <div className="flex-1 grid grid-cols-2 gap-4">
              <Button variant="outline" className="h-20 flex-col">
                <FileText className="w-6 h-6 mb-2" />
                New Prescription
              </Button>
              <Button variant="outline" className="h-20 flex-col">
                <Users className="w-6 h-6 mb-2" />
                Patient History
              </Button>
              <Button variant="outline" className="h-20 flex-col">
                <Calendar className="w-6 h-6 mb-2" />
                Schedule Appointment
              </Button>
              <Button variant="outline" className="h-20 flex-col">
                <Stethoscope className="w-6 h-6 mb-2" />
                Call Next Patient
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default DoctorDashboard;