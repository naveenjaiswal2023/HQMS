import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Progress } from '@/components/ui/progress';
import { 
  Activity, 
  Clock,
  Users,
  AlertTriangle,
  CheckCircle,
  Phone,
  Play,
  Pause,
  SkipForward
} from 'lucide-react';
import { useAuth } from '@/contexts/AuthContext';

const Queue = () => {
  const { user } = useAuth();
  const [currentTime, setCurrentTime] = useState(new Date());

  // Use state for queue data to allow dynamic updates
  const [queueData, setQueueData] = useState([
    {
      id: 1,
      queueNumber: 'Q001',
      patient: {
        name: 'Sarah Johnson',
        id: 'P001',
        phone: '+1 555-0123'
      },
      doctor: {
        name: 'Dr. Smith',
        department: 'Cardiology'
      },
      appointmentTime: '10:30 AM',
      joinedAt: '10:15 AM',
      estimatedWait: 15,
      priority: 'high',
      status: 'waiting'
    },
    {
      id: 2,
      queueNumber: 'Q002',
      patient: {
        name: 'Michael Chen',
        id: 'P002',
        phone: '+1 555-0124'
      },
      doctor: {
        name: 'Dr. Wilson',
        department: 'Orthopedics'
      },
      appointmentTime: '10:45 AM',
      joinedAt: '10:30 AM',
      estimatedWait: 25,
      priority: 'normal',
      status: 'called'
    },
    {
      id: 3,
      queueNumber: 'Q003',
      patient: {
        name: 'Emma Davis',
        id: 'P003',
        phone: '+1 555-0125'
      },
      doctor: {
        name: 'Dr. Johnson',
        department: 'Emergency'
      },
      appointmentTime: 'Walk-in',
      joinedAt: '10:45 AM',
      estimatedWait: 5,
      priority: 'emergency',
      status: 'in-consultation'
    },
    {
      id: 4,
      queueNumber: 'Q004',
      patient: {
        name: 'Robert Wilson',
        id: 'P004',
        phone: '+1 555-0126'
      },
      doctor: {
        name: 'Dr. Brown',
        department: 'General Medicine'
      },
      appointmentTime: '11:00 AM',
      joinedAt: '10:50 AM',
      estimatedWait: 35,
      priority: 'normal',
      status: 'waiting'
    },
    {
      id: 5,
      queueNumber: 'Q005',
      patient: {
        name: 'Linda Garcia',
        id: 'P005',
        phone: '+1 555-0127'
      },
      doctor: {
        name: 'Dr. Martinez',
        department: 'Pediatrics'
      },
      appointmentTime: '11:15 AM',
      joinedAt: '11:00 AM',
      estimatedWait: 45,
      priority: 'normal',
      status: 'waiting'
    }
  ]);

  const departments = [
    {
      name: 'Cardiology',
      currentPatients: 3,
      avgWaitTime: 20,
      status: 'normal'
    },
    {
      name: 'Orthopedics',
      currentPatients: 2,
      avgWaitTime: 15,
      status: 'fast'
    },
    {
      name: 'Emergency',
      currentPatients: 5,
      avgWaitTime: 35,
      status: 'busy'
    },
    {
      name: 'General Medicine',
      currentPatients: 4,
      avgWaitTime: 25,
      status: 'normal'
    }
  ];

  useEffect(() => {
    const timer = setInterval(() => setCurrentTime(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'emergency': return 'bg-destructive text-destructive-foreground';
      case 'high': return 'bg-warning text-warning-foreground';
      case 'normal': return 'bg-primary text-primary-foreground';
      default: return 'bg-muted text-muted-foreground';
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'waiting': return 'bg-yellow-100 text-yellow-800';
      case 'called': return 'bg-blue-100 text-blue-800';
      case 'in-consultation': return 'bg-green-100 text-green-800';
      case 'completed': return 'bg-gray-100 text-gray-800';
      default: return 'bg-muted text-muted-foreground';
    }
  };

  const getDepartmentStatus = (status: string) => {
    switch (status) {
      case 'fast': return { color: 'text-success', bg: 'bg-success/10' };
      case 'busy': return { color: 'text-destructive', bg: 'bg-destructive/10' };
      default: return { color: 'text-primary', bg: 'bg-primary/10' };
    }
  };

  const canManageQueue = user?.role === 'admin' || user?.role === 'receptionist' || user?.role === 'doctor';

  // Function to update queue status for a given queue item id
  const updateQueueStatus = (id: number, newStatus: string) => {
    setQueueData(prevData =>
      prevData.map(item => item.id === id ? { ...item, status: newStatus } : item)
    );
  };

  // Handlers for buttons
  const handleCall = (id: number) => updateQueueStatus(id, 'called');
  const handleHold = (id: number) => updateQueueStatus(id, 'waiting');
  const handleBegin = (id: number) => updateQueueStatus(id, 'in-consultation');
  const handleComplete = (id: number) => updateQueueStatus(id, 'completed');

  return (
    <div className="p-6 space-y-6 animate-fade-in">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Queue Management</h1>
          <p className="text-muted-foreground">
            Real-time patient queue monitoring and management
          </p>
        </div>
        <div className="text-right">
          <p className="text-lg font-semibold text-foreground">{currentTime.toLocaleTimeString()}</p>
          <p className="text-sm text-muted-foreground">{currentTime.toLocaleDateString()}</p>
        </div>
      </div>

      {/* Department Overview */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {departments.map((dept, index) => {
          const statusStyle = getDepartmentStatus(dept.status);
          return (
            <Card key={index} className="medical-card">
              <CardContent className="p-4">
                <div className="flex items-center justify-between mb-2">
                  <h3 className="font-medium text-foreground">{dept.name}</h3>
                  <div className={`w-3 h-3 rounded-full ${statusStyle.bg}`}>
                    <div className={`w-full h-full rounded-full ${
                      statusStyle.color === 'text-success' ? 'bg-success' :
                      statusStyle.color === 'text-destructive' ? 'bg-destructive' : 'bg-primary'
                    }`} />
                  </div>
                </div>
                <div className="space-y-1">
                  <div className="flex justify-between text-sm">
                    <span className="text-muted-foreground">Patients:</span>
                    <span className="font-medium">{dept.currentPatients}</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-muted-foreground">Avg Wait:</span>
                    <span className="font-medium">{dept.avgWaitTime} min</span>
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Current Queue */}
      <Card className="medical-card">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Activity className="w-5 h-5" />
            Current Queue ({queueData.length} patients)
          </CardTitle>
          <CardDescription>
            Real-time patient queue status and management
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {queueData.map((item, index) => (
              <div key={item.id} className="flex items-center justify-between p-4 bg-muted rounded-lg hover:bg-muted/80 transition-medical">
                <div className="flex items-center gap-4">
                  <div className="text-center">
                    <div className="w-12 h-12 bg-gradient-primary rounded-full flex items-center justify-center">
                      <span className="text-white font-bold text-lg">{index + 1}</span>
                    </div>
                    <p className="text-xs text-muted-foreground mt-1">{item.queueNumber}</p>
                  </div>

                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <h4 className="font-medium text-foreground">{item.patient.name}</h4>
                      <Badge className={getPriorityColor(item.priority)}>{item.priority}</Badge>
                      <Badge className={getStatusColor(item.status)}>{item.status}</Badge>
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-3 gap-2 text-sm text-muted-foreground">
                      <div className="flex items-center gap-1">
                        <Users className="w-4 h-4" />
                        <span>{item.doctor.name} • {item.doctor.department}</span>
                      </div>
                      <div className="flex items-center gap-1">
                        <Clock className="w-4 h-4" />
                        <span>Appt: {item.appointmentTime} • Joined: {item.joinedAt}</span>
                      </div>
                      <div className="flex items-center gap-1">
                        <AlertTriangle className="w-4 h-4" />
                        <span>Est. wait: {item.estimatedWait} min</span>
                      </div>
                    </div>

                    {item.estimatedWait > 0 && (
                      <div className="mt-2">
                        <Progress 
                          value={Math.max(0, 100 - (item.estimatedWait * 2))} 
                          className="h-2" 
                        />
                      </div>
                    )}
                  </div>
                </div>
                
                {canManageQueue && (
                  <div className="flex gap-2">
                    {item.status === 'waiting' && (
                      <>
                        <Button variant="outline" size="sm" onClick={() => handleCall(item.id)}>
                          <Phone className="w-4 h-4 mr-2" />
                          Call
                        </Button>
                        <Button variant="medical" size="sm" onClick={() => handleBegin(item.id)}>
                          <Play className="w-4 h-4 mr-2" />
                          Start
                        </Button>
                      </>
                    )}
                    {item.status === 'called' && (
                      <>
                        <Button variant="outline" size="sm" onClick={() => handleHold(item.id)}>
                          <Pause className="w-4 h-4 mr-2" />
                          Hold
                        </Button>
                        <Button variant="success" size="sm" onClick={() => handleBegin(item.id)}>
                          <CheckCircle className="w-4 h-4 mr-2" />
                          Begin
                        </Button>
                      </>
                    )}
                    {item.status === 'in-consultation' && (
                      <Button variant="success" size="sm" onClick={() => handleComplete(item.id)}>
                        <CheckCircle className="w-4 h-4 mr-2" />
                        Complete
                      </Button>
                    )}
                    <Button variant="ghost" size="sm">
                      <SkipForward className="w-4 h-4" />
                    </Button>
                  </div>
                )}
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Queue Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card className="medical-card">
          <CardContent className="p-4 text-center">
            <div className="flex items-center justify-center gap-2 mb-2">
              <Users className="w-5 h-5 text-primary" />
              <span className="text-2xl font-bold text-primary">{queueData.length}</span>
            </div>
            <p className="text-sm text-muted-foreground">Total in Queue</p>
          </CardContent>
        </Card>

        <Card className="medical-card">
          <CardContent className="p-4 text-center">
            <div className="flex items-center justify-center gap-2 mb-2">
              <Clock className="w-5 h-5 text-warning" />
              <span className="text-2xl font-bold text-warning">
                {Math.round(queueData.reduce((sum, item) => sum + item.estimatedWait, 0) / queueData.length)}
              </span>
            </div>
            <p className="text-sm text-muted-foreground">Avg Wait (min)</p>
          </CardContent>
        </Card>

        <Card className="medical-card">
          <CardContent className="p-4 text-center">
            <div className="flex items-center justify-center gap-2 mb-2">
              <AlertTriangle className="w-5 h-5 text-destructive" />
              <span className="text-2xl font-bold text-destructive">
                {queueData.filter(item => item.priority === 'emergency' || item.priority === 'high').length}
              </span>
            </div>
            <p className="text-sm text-muted-foreground">Priority Cases</p>
          </CardContent>
        </Card>

        <Card className="medical-card">
          <CardContent className="p-4 text-center">
            <div className="flex items-center justify-center gap-2 mb-2">
              <CheckCircle className="w-5 h-5 text-success" />
              <span className="text-2xl font-bold text-success">
                {queueData.filter(item => item.status === 'in-consultation').length}
              </span>
            </div>
            <p className="text-sm text-muted-foreground">In Consultation</p>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default Queue;
