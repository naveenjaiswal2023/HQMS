import React, { useState } from 'react';
import { toast } from 'sonner';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { useAuth } from '@/contexts/AuthContext';
import { 
  Bed, 
  Monitor, 
  User, 
  Search,
  Plus,
  AlertCircle,
  CheckCircle,
  Clock,
  Settings
} from 'lucide-react';

const Pod = () => {
  const { user } = useAuth();
  const [searchTerm, setSearchTerm] = useState('');

  const handleAssignPatient = (podId?: string) => {
    if (podId) {
      toast.success(`Assigning patient to pod ${podId}...`);
    } else {
      toast.info('Opening patient assignment form...');
    }
    // Show patient assignment modal
  };

  const handleViewDetails = (podId: string) => {
    toast.info(`Viewing details for pod ${podId}`);
    // Show pod details modal with full patient information
  };

  const handleUpdateVitals = (podId: string) => {
    toast.success(`Opening vitals update form for pod ${podId}...`);
    // Show vitals update modal or form
  };

  // Mock pod data
  const pods = [
    {
      id: 'POD-001',
      roomNumber: 'ICU-101',
      patientName: 'John Smith',
      condition: 'Post-Surgery Recovery',
      status: 'occupied',
      admissionDate: '2024-09-05',
      assignedDoctor: 'Dr. Sarah Wilson',
      vitals: {
        heartRate: 78,
        bloodPressure: '120/80',
        temperature: 98.6,
        oxygenSaturation: 98
      }
    },
    {
      id: 'POD-002',
      roomNumber: 'ICU-102',
      patientName: 'Emily Johnson',
      condition: 'Cardiac Monitoring',
      status: 'occupied',
      admissionDate: '2024-09-04',
      assignedDoctor: 'Dr. Michael Brown',
      vitals: {
        heartRate: 82,
        bloodPressure: '118/75',
        temperature: 99.1,
        oxygenSaturation: 96
      }
    },
    {
      id: 'POD-003',
      roomNumber: 'ICU-103',
      patientName: null,
      condition: null,
      status: 'available',
      admissionDate: null,
      assignedDoctor: null,
      vitals: null
    },
    {
      id: 'POD-004',
      roomNumber: 'ICU-104',
      patientName: 'Robert Davis',
      condition: 'Neurological Observation',
      status: 'occupied',
      admissionDate: '2024-09-06',
      assignedDoctor: 'Dr. Lisa Chen',
      vitals: {
        heartRate: 76,
        bloodPressure: '115/70',
        temperature: 98.4,
        oxygenSaturation: 99
      }
    },
    {
      id: 'POD-005',
      roomNumber: 'ICU-105',
      patientName: null,
      condition: null,
      status: 'maintenance',
      admissionDate: null,
      assignedDoctor: null,
      vitals: null
    }
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'occupied': return 'bg-warning text-warning-foreground';
      case 'available': return 'bg-success text-success-foreground';
      case 'maintenance': return 'bg-destructive text-destructive-foreground';
      default: return 'bg-secondary text-secondary-foreground';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'occupied': return <User className="w-4 h-4" />;
      case 'available': return <CheckCircle className="w-4 h-4" />;
      case 'maintenance': return <Settings className="w-4 h-4" />;
      default: return <AlertCircle className="w-4 h-4" />;
    }
  };

  const filteredPods = pods.filter(pod =>
    pod.roomNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (pod.patientName && pod.patientName.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (pod.assignedDoctor && pod.assignedDoctor.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const occupiedPods = pods.filter(p => p.status === 'occupied').length;
  const availablePods = pods.filter(p => p.status === 'available').length;
  const maintenancePods = pods.filter(p => p.status === 'maintenance').length;

  return (
    <div className="container mx-auto p-6 space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Pod Management</h1>
          <p className="text-muted-foreground">
            Monitor and manage intensive care pods and patient occupancy
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
            <Input
              placeholder="Search pods..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 w-64"
            />
          </div>
          {(user?.role === 'admin' || user?.role === 'doctor') && (
            <Button onClick={() => handleAssignPatient()}>
              <Plus className="w-4 h-4 mr-2" />
              Assign Patient
            </Button>
          )}
        </div>
      </div>

      {/* Pod Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card className="medical-card">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Occupied Pods</CardTitle>
            <User className="h-4 w-4 text-warning" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-warning">{occupiedPods}</div>
            <p className="text-xs text-muted-foreground">Currently in use</p>
          </CardContent>
        </Card>

        <Card className="medical-card">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Available Pods</CardTitle>
            <CheckCircle className="h-4 w-4 text-success" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-success">{availablePods}</div>
            <p className="text-xs text-muted-foreground">Ready for patients</p>
          </CardContent>
        </Card>

        <Card className="medical-card">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Maintenance</CardTitle>
            <Settings className="h-4 w-4 text-destructive" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-destructive">{maintenancePods}</div>
            <p className="text-xs text-muted-foreground">Under maintenance</p>
          </CardContent>
        </Card>
      </div>

      {/* Pod Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredPods.map((pod) => (
          <Card key={pod.id} className="medical-card">
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="flex items-center gap-2">
                  <Bed className="w-5 h-5" />
                  {pod.roomNumber}
                </CardTitle>
                <Badge className={getStatusColor(pod.status)}>
                  {getStatusIcon(pod.status)}
                  <span className="ml-1 capitalize">{pod.status}</span>
                </Badge>
              </div>
            </CardHeader>
            <CardContent className="space-y-4">
              {pod.status === 'occupied' && pod.patientName ? (
                <>
                  <div>
                    <p className="font-medium text-foreground">{pod.patientName}</p>
                    <p className="text-sm text-muted-foreground">{pod.condition}</p>
                    <p className="text-xs text-muted-foreground">
                      Admitted: {pod.admissionDate}
                    </p>
                    <p className="text-xs text-muted-foreground">
                      Doctor: {pod.assignedDoctor}
                    </p>
                  </div>
                  
                  {pod.vitals && (
                    <div className="space-y-2">
                      <h4 className="text-sm font-medium flex items-center gap-1">
                        <Monitor className="w-4 h-4" />
                        Vital Signs
                      </h4>
                      <div className="grid grid-cols-2 gap-2 text-xs">
                        <div>
                          <span className="text-muted-foreground">HR:</span>
                          <span className="ml-1 font-medium">{pod.vitals.heartRate} bpm</span>
                        </div>
                        <div>
                          <span className="text-muted-foreground">BP:</span>
                          <span className="ml-1 font-medium">{pod.vitals.bloodPressure}</span>
                        </div>
                        <div>
                          <span className="text-muted-foreground">Temp:</span>
                          <span className="ml-1 font-medium">{pod.vitals.temperature}°F</span>
                        </div>
                        <div>
                          <span className="text-muted-foreground">O2:</span>
                          <span className="ml-1 font-medium">{pod.vitals.oxygenSaturation}%</span>
                        </div>
                      </div>
                    </div>
                  )}
                  
                  <div className="flex gap-2">
                    <Button variant="outline" size="sm" className="flex-1" onClick={() => handleViewDetails(pod.id)}>
                      View Details
                    </Button>
                    <Button variant="outline" size="sm" className="flex-1" onClick={() => handleUpdateVitals(pod.id)}>
                      Update Vitals
                    </Button>
                  </div>
                </>
              ) : (
                <div className="text-center py-8 text-muted-foreground">
                  {pod.status === 'available' ? (
                    <>
                      <Bed className="w-8 h-8 mx-auto mb-2 opacity-50" />
                      <p>Pod available for new patient</p>
                      <Button variant="outline" size="sm" className="mt-2" onClick={() => handleAssignPatient(pod.id)}>
                        Assign Patient
                      </Button>
                    </>
                  ) : (
                    <>
                      <Settings className="w-8 h-8 mx-auto mb-2 opacity-50" />
                      <p>Pod under maintenance</p>
                    </>
                  )}
                </div>
              )}
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
};

export default Pod;