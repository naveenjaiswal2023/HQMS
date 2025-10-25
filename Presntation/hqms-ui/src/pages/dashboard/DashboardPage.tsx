import React from 'react';
import { Calendar, Users, Clock, CreditCard, TrendingUp, Activity, AlertCircle, CheckCircle } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { useAuth } from '@/context/AuthContext';
import { useNavigate } from 'react-router-dom';

const statsCards = [
  {
    title: 'Total Patients',
    value: '2,847',
    change: '+12%',
    icon: Users,
    color: 'text-primary',
    bgColor: 'bg-primary/10',
  },
  {
    title: "Today's Appointments",
    value: '24',
    change: '+5%',
    icon: Calendar,
    color: 'text-success',
    bgColor: 'bg-success/10',
  },
  {
    title: 'Queue Wait Time',
    value: '18 min',
    change: '-8%',
    icon: Clock,
    color: 'text-warning',
    bgColor: 'bg-warning/10',
  },
  {
    title: 'Monthly Revenue',
    value: '$45,720',
    change: '+23%',
    icon: CreditCard,
    color: 'text-secondary',
    bgColor: 'bg-secondary/10',
  },
];

const recentAppointments = [
  {
    id: '1',
    patient: 'Sarah Johnson',
    doctor: 'Dr. Smith',
    time: '09:30 AM',
    status: 'confirmed',
    type: 'Consultation',
  },
  {
    id: '2',
    patient: 'Michael Chen',
    doctor: 'Dr. Brown',
    time: '10:15 AM',
    status: 'in-progress',
    type: 'Follow-up',
  },
  {
    id: '3',
    patient: 'Emily Davis',
    doctor: 'Dr. Wilson',
    time: '11:00 AM',
    status: 'scheduled',
    type: 'Check-up',
  },
  {
    id: '4',
    patient: 'Robert Taylor',
    doctor: 'Dr. Johnson',
    time: '11:45 AM',
    status: 'completed',
    type: 'Emergency',
  },
];

const notifications = [
  {
    id: '1',
    title: 'New patient registered',
    message: 'Sarah Johnson has completed registration',
    time: '5 minutes ago',
    type: 'info',
  },
  {
    id: '2',
    title: 'Payment received',
    message: '$250 payment processed for appointment #1234',
    time: '15 minutes ago',
    type: 'success',
  },
  {
    id: '3',
    title: 'Appointment cancelled',
    message: 'Michael Brown cancelled 3:00 PM appointment',
    time: '1 hour ago',
    type: 'warning',
  },
];

const getStatusBadge = (status: string) => {
  const variants = {
    scheduled: 'default',
    confirmed: 'secondary', 
    'in-progress': 'warning',
    completed: 'success',
    cancelled: 'destructive',
  } as const;

  return (
    <Badge variant={variants[status as keyof typeof variants] || 'default'} className="capitalize">
      {status.replace('-', ' ')}
    </Badge>
  );
};

export const DashboardPage: React.FC = () => {
  const { user } = useAuth();
  const navigate = useNavigate();

  const welcomeMessage = () => {
    const hour = new Date().getHours();
    if (hour < 12) return 'Good morning';
    if (hour < 18) return 'Good afternoon';
    return 'Good evening';
  };

  return (
    <div className="space-y-6">
      {/* Welcome Header */}
      <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">
            {welcomeMessage()}, {user?.firstName}!
          </h1>
          <p className="text-muted-foreground">
            Here's what's happening in your healthcare system today.
          </p>
        </div>
        <div className="flex gap-3">
          <Button 
            onClick={() => navigate('/appointments/new')} 
            className="bg-gradient-medical hover:opacity-90"
          >
            <Calendar className="mr-2 h-4 w-4" />
            Book Appointment
          </Button>
          <Button 
            variant="outline" 
            onClick={() => navigate('/patients/new')}
          >
            <Users className="mr-2 h-4 w-4" />
            Add Patient
          </Button>
        </div>
      </div>

      {/* Stats Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {statsCards.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <Card key={index} className="shadow-card border-0">
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  {stat.title}
                </CardTitle>
                <div className={`p-2 rounded-lg ${stat.bgColor}`}>
                  <Icon className={`h-4 w-4 ${stat.color}`} />
                </div>
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold text-foreground">{stat.value}</div>
                <p className="text-xs text-muted-foreground">
                  <span className={stat.change.startsWith('+') ? 'text-success' : 'text-destructive'}>
                    {stat.change}
                  </span>{' '}
                  from last month
                </p>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Main Content Grid */}
      <div className="grid gap-6 lg:grid-cols-3">
        {/* Recent Appointments */}
        <div className="lg:col-span-2">
          <Card className="shadow-card border-0">
            <CardHeader>
              <div className="flex items-center justify-between">
                <div>
                  <CardTitle className="text-foreground">Recent Appointments</CardTitle>
                  <CardDescription>
                    Today's scheduled and ongoing appointments
                  </CardDescription>
                </div>
                <Button 
                  variant="outline" 
                  size="sm" 
                  onClick={() => navigate('/appointments')}
                >
                  View All
                </Button>
              </div>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {recentAppointments.map((appointment) => (
                  <div
                    key={appointment.id}
                    className="flex items-center justify-between p-4 rounded-lg border border-muted hover:bg-muted/50 transition-colors"
                  >
                    <div className="flex items-center gap-4">
                      <div className="w-10 h-10 bg-gradient-wellness rounded-full flex items-center justify-center">
                        <span className="text-sm font-semibold text-white">
                          {appointment.patient.split(' ').map(n => n[0]).join('')}
                        </span>
                      </div>
                      <div>
                        <p className="font-medium text-foreground">{appointment.patient}</p>
                        <p className="text-sm text-muted-foreground">
                          {appointment.doctor} • {appointment.type}
                        </p>
                      </div>
                    </div>
                    <div className="flex items-center gap-4">
                      <span className="text-sm text-muted-foreground">{appointment.time}</span>
                      {getStatusBadge(appointment.status)}
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Notifications & Quick Actions */}
        <div className="space-y-6">
          {/* Notifications */}
          <Card className="shadow-card border-0">
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="text-foreground">Notifications</CardTitle>
                <Button 
                  variant="outline" 
                  size="sm" 
                  onClick={() => navigate('/notifications')}
                >
                  View All
                </Button>
              </div>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {notifications.map((notification) => (
                  <div
                    key={notification.id}
                    className="flex gap-3 p-3 rounded-lg border border-muted hover:bg-muted/50 transition-colors"
                  >
                    <div className="flex-shrink-0 mt-1">
                      {notification.type === 'success' && (
                        <CheckCircle className="w-4 h-4 text-success" />
                      )}
                      {notification.type === 'warning' && (
                        <AlertCircle className="w-4 h-4 text-warning" />
                      )}
                      {notification.type === 'info' && (
                        <Activity className="w-4 h-4 text-primary" />
                      )}
                    </div>
                    <div className="flex-1 space-y-1">
                      <p className="text-sm font-medium text-foreground">
                        {notification.title}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        {notification.message}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        {notification.time}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>

          {/* Quick Actions */}
          <Card className="shadow-card border-0">
            <CardHeader>
              <CardTitle className="text-foreground">Quick Actions</CardTitle>
            </CardHeader>
            <CardContent className="space-y-2">
              <Button 
                variant="outline" 
                className="w-full justify-start" 
                onClick={() => navigate('/queue')}
              >
                <Clock className="mr-2 h-4 w-4" />
                View Queue Status
              </Button>
              <Button 
                variant="outline" 
                className="w-full justify-start" 
                onClick={() => navigate('/patients')}
              >
                <Users className="mr-2 h-4 w-4" />
                Patient Directory
              </Button>
              <Button 
                variant="outline" 
                className="w-full justify-start" 
                onClick={() => navigate('/payments')}
              >
                <CreditCard className="mr-2 h-4 w-4" />
                Payment Records
              </Button>
              <Button 
                variant="outline" 
                className="w-full justify-start" 
                onClick={() => navigate('/reports')}
              >
                <TrendingUp className="mr-2 h-4 w-4" />
                Generate Reports
              </Button>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
};