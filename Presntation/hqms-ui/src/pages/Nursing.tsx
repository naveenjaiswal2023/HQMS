import React, { useState } from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { useAuth } from '@/contexts/AuthContext';
import { 
  Heart, 
  Pill, 
  Clock, 
  User, 
  Search,
  Plus,
  AlertCircle,
  CheckCircle,
  Calendar,
  FileText
} from 'lucide-react';

const Nursing = () => {
  const { user } = useAuth();
  const [searchTerm, setSearchTerm] = useState('');

  // Mock nursing tasks data
  const nursingTasks = [
    {
      id: 'TASK-001',
      patientName: 'John Smith',
      room: 'ICU-101',
      task: 'Medication Administration',
      description: 'Administer prescribed pain medication',
      scheduledTime: '09:00',
      status: 'completed',
      priority: 'high',
      assignedNurse: 'Nurse Sarah Johnson',
      completedTime: '09:05'
    },
    {
      id: 'TASK-002',
      patientName: 'Emily Johnson',
      room: 'ICU-102',
      task: 'Vital Signs Check',
      description: 'Monitor blood pressure and heart rate',
      scheduledTime: '10:00',
      status: 'pending',
      priority: 'medium',
      assignedNurse: 'Nurse Michael Chen',
      completedTime: null
    },
    {
      id: 'TASK-003',
      patientName: 'Robert Davis',
      room: 'ICU-104',
      task: 'Wound Dressing',
      description: 'Change surgical wound dressing',
      scheduledTime: '11:30',
      status: 'overdue',
      priority: 'high',
      assignedNurse: 'Nurse Lisa Wang',
      completedTime: null
    },
    {
      id: 'TASK-004',
      patientName: 'Maria Garcia',
      room: 'Room-205',
      task: 'Patient Education',
      description: 'Explain post-discharge care instructions',
      scheduledTime: '14:00',
      status: 'scheduled',
      priority: 'low',
      assignedNurse: 'Nurse Sarah Johnson',
      completedTime: null
    }
  ];

  const nurses = [
    {
      id: 'NURSE-001',
      name: 'Nurse Sarah Johnson',
      shift: 'Morning',
      department: 'ICU',
      status: 'on-duty',
      activeTasks: 3,
      completedTasks: 8
    },
    {
      id: 'NURSE-002',
      name: 'Nurse Michael Chen',
      shift: 'Morning',
      department: 'ICU',
      status: 'on-duty',
      activeTasks: 2,
      completedTasks: 6
    },
    {
      id: 'NURSE-003',
      name: 'Nurse Lisa Wang',
      shift: 'Morning',
      department: 'General Ward',
      status: 'break',
      activeTasks: 1,
      completedTasks: 5
    }
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed': return 'bg-success text-success-foreground';
      case 'pending': return 'bg-warning text-warning-foreground';
      case 'overdue': return 'bg-destructive text-destructive-foreground';
      case 'scheduled': return 'bg-primary text-primary-foreground';
      default: return 'bg-secondary text-secondary-foreground';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'high': return 'bg-destructive text-destructive-foreground';
      case 'medium': return 'bg-warning text-warning-foreground';
      case 'low': return 'bg-success text-success-foreground';
      default: return 'bg-secondary text-secondary-foreground';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'completed': return <CheckCircle className="w-4 h-4" />;
      case 'pending': return <Clock className="w-4 h-4" />;
      case 'overdue': return <AlertCircle className="w-4 h-4" />;
      case 'scheduled': return <Calendar className="w-4 h-4" />;
      default: return <FileText className="w-4 h-4" />;
    }
  };

  const filteredTasks = nursingTasks.filter(task =>
    task.patientName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    task.task.toLowerCase().includes(searchTerm.toLowerCase()) ||
    task.room.toLowerCase().includes(searchTerm.toLowerCase()) ||
    task.assignedNurse.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const completedTasks = nursingTasks.filter(t => t.status === 'completed').length;
  const pendingTasks = nursingTasks.filter(t => t.status === 'pending').length;
  const overdueTasks = nursingTasks.filter(t => t.status === 'overdue').length;

  return (
    <div className="container mx-auto p-6 space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Nursing Management</h1>
          <p className="text-muted-foreground">
            Manage nursing tasks, schedules, and patient care activities
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
            <Input
              placeholder="Search tasks..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 w-64"
            />
          </div>
          {(user?.role === 'admin' || user?.role === 'doctor') && (
            <Button>
              <Plus className="w-4 h-4 mr-2" />
              New Task
            </Button>
          )}
        </div>
      </div>

      {/* Task Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card className="medical-card">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Completed Tasks</CardTitle>
            <CheckCircle className="h-4 w-4 text-success" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-success">{completedTasks}</div>
            <p className="text-xs text-muted-foreground">Tasks finished today</p>
          </CardContent>
        </Card>

        <Card className="medical-card">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Pending Tasks</CardTitle>
            <Clock className="h-4 w-4 text-warning" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-warning">{pendingTasks}</div>
            <p className="text-xs text-muted-foreground">Awaiting completion</p>
          </CardContent>
        </Card>

        <Card className="medical-card">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Overdue Tasks</CardTitle>
            <AlertCircle className="h-4 w-4 text-destructive" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-destructive">{overdueTasks}</div>
            <p className="text-xs text-muted-foreground">Require immediate attention</p>
          </CardContent>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Nursing Tasks */}
        <div className="lg:col-span-2">
          <Card className="medical-card">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <FileText className="w-5 h-5" />
                Nursing Tasks
              </CardTitle>
              <CardDescription>
                {filteredTasks.length} tasks scheduled for today
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {filteredTasks.map((task) => (
                  <div key={task.id} className="flex items-center justify-between p-4 border border-border rounded-lg hover:bg-accent/50 transition-colors">
                    <div className="flex items-center gap-4">
                      <div className="flex items-center gap-2">
                        <Clock className="w-4 h-4 text-muted-foreground" />
                        <span className="font-medium">{task.scheduledTime}</span>
                      </div>
                      <div className="flex-1">
                        <div className="flex items-center gap-2 mb-1">
                          <p className="font-medium text-foreground">{task.task}</p>
                          <Badge className={getPriorityColor(task.priority)}>
                            {task.priority}
                          </Badge>
                        </div>
                        <p className="text-sm text-muted-foreground">{task.description}</p>
                        <p className="text-xs text-muted-foreground">
                          {task.patientName} • {task.room} • {task.assignedNurse}
                        </p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <Badge className={getStatusColor(task.status)}>
                        {getStatusIcon(task.status)}
                        <span className="ml-1 capitalize">{task.status}</span>
                      </Badge>
                      <Button variant="outline" size="sm">
                        {task.status === 'completed' ? 'View' : 'Complete'}
                      </Button>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Nursing Staff */}
        <div>
          <Card className="medical-card">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Heart className="w-5 h-5" />
                Nursing Staff
              </CardTitle>
              <CardDescription>
                Current shift personnel
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {nurses.map((nurse) => (
                  <div key={nurse.id} className="flex items-center justify-between p-3 border border-border rounded-lg">
                    <div className="flex items-center gap-3">
                      <div className="flex items-center justify-center w-10 h-10 bg-gradient-primary rounded-full">
                        <User className="w-5 h-5 text-white" />
                      </div>
                      <div>
                        <p className="font-medium text-foreground">{nurse.name}</p>
                        <p className="text-xs text-muted-foreground">{nurse.department} • {nurse.shift}</p>
                        <div className="flex items-center gap-2 mt-1">
                          <Badge variant={nurse.status === 'on-duty' ? 'default' : 'secondary'}>
                            {nurse.status}
                          </Badge>
                        </div>
                      </div>
                    </div>
                    <div className="text-right text-xs text-muted-foreground">
                      <p>{nurse.activeTasks} active</p>
                      <p>{nurse.completedTasks} completed</p>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default Nursing;