import React, { useState } from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { 
  Search, 
  Plus, 
  Filter,
  Phone,
  Mail,
  Calendar,
  MapPin,
  Edit,
  Eye
} from 'lucide-react';
import { NewPatientDialog } from '@/components/forms/NewPatientDialog';  // Import NewPatientDialog
import { NewAppointmentDialog } from '@/components/forms/NewAppointmentDialog';  // Import NewAppointmentDialog

const Patients = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [showNewPatientDialog, setShowNewPatientDialog] = useState(false);
  const [showNewAppointmentDialog, setShowNewAppointmentDialog] = useState(false);
  const [selectedPatientId, setSelectedPatientId] = useState<string | null>(null);

  const patients = [
    {
      id: '1',
      firstName: 'Sarah',
      lastName: 'Johnson',
      email: 'sarah.johnson@email.com',
      phone: '+1 (555) 123-4567',
      dateOfBirth: '1985-03-15',
      gender: 'female',
      lastVisit: '2024-03-10',
      status: 'active',
      condition: 'Hypertension'
    },
    {
      id: '2',
      firstName: 'Michael',
      lastName: 'Chen',
      email: 'michael.chen@email.com',
      phone: '+1 (555) 234-5678',
      dateOfBirth: '1978-08-22',
      gender: 'male',
      lastVisit: '2024-03-08',
      status: 'active',
      condition: 'Diabetes Type 2'
    },
    {
      id: '3',
      firstName: 'Emma',
      lastName: 'Wilson',
      email: 'emma.wilson@email.com',
      phone: '+1 (555) 345-6789',
      dateOfBirth: '1992-11-07',
      gender: 'female',
      lastVisit: '2024-03-05',
      status: 'inactive',
      condition: 'Routine Checkup'
    },
    {
      id: '4',
      firstName: 'Robert',
      lastName: 'Davis',
      email: 'robert.davis@email.com',
      phone: '+1 (555) 456-7890',
      dateOfBirth: '1965-06-30',
      gender: 'male',
      lastVisit: '2024-03-12',
      status: 'active',
      condition: 'Cardiology Follow-up'
    },
    {
      id: '5',
      firstName: 'Linda',
      lastName: 'Garcia',
      email: 'linda.garcia@email.com',
      phone: '+1 (555) 567-8901',
      dateOfBirth: '1990-12-18',
      gender: 'female',
      lastVisit: '2024-03-07',
      status: 'active',
      condition: 'Pregnancy Care'
    }
  ];

  const filteredPatients = patients.filter(patient =>
    `${patient.firstName} ${patient.lastName}`.toLowerCase().includes(searchTerm.toLowerCase()) ||
    patient.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
    patient.phone.includes(searchTerm)
  );

  const calculateAge = (dateOfBirth: string) => {
    const today = new Date();
    const birthDate = new Date(dateOfBirth);
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();

    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }

    return age;
  };

  const getStatusColor = (status: string) => {
    return status === 'active' ? 'bg-success text-success-foreground' : 'bg-muted text-muted-foreground';
  };

  // Handler for Schedule button to open NewAppointmentDialog with patient context
  const handleSchedule = (patientId: string) => {
    setSelectedPatientId(patientId);
    setShowNewAppointmentDialog(true);
  };

  return (
    <div className="p-6 space-y-6 animate-fade-in">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Patient Management</h1>
          <p className="text-muted-foreground">
            Manage patient records and information
          </p>
        </div>
        <Button variant="medical" onClick={() => setShowNewPatientDialog(true)}>
          <Plus className="w-4 h-4 mr-2" />
          Add New Patient
        </Button>
      </div>

      {/* Search and Filters */}
      <Card className="medical-card">
        <CardContent className="p-6">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1 relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
              <Input
                placeholder="Search patients by name, email, or phone..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10"
              />
            </div>
            <Button variant="outline">
              <Filter className="w-4 h-4 mr-2" />
              Filters
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Patients List */}
      <div className="grid gap-4">
        {filteredPatients.map((patient) => (
          <Card key={patient.id} className="medical-card hover:shadow-hover transition-medical">
            <CardContent className="p-6">
              <div className="flex flex-col lg:flex-row lg:items-center justify-between gap-4">
                <div className="flex items-center gap-4">
                  <div className="w-12 h-12 bg-gradient-primary rounded-full flex items-center justify-center">
                    <span className="text-white font-semibold text-lg">
                      {patient.firstName[0]}{patient.lastName[0]}
                    </span>
                  </div>
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <h3 className="text-lg font-semibold text-foreground">
                        {patient.firstName} {patient.lastName}
                      </h3>
                      <Badge className={getStatusColor(patient.status)}>
                        {patient.status}
                      </Badge>
                    </div>
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-2 text-sm text-muted-foreground">
                      <div className="flex items-center gap-1">
                        <Mail className="w-4 h-4" />
                        <span>{patient.email}</span>
                      </div>
                      <div className="flex items-center gap-1">
                        <Phone className="w-4 h-4" />
                        <span>{patient.phone}</span>
                      </div>
                      <div className="flex items-center gap-1">
                        <Calendar className="w-4 h-4" />
                        <span>{calculateAge(patient.dateOfBirth)} years old</span>
                      </div>
                      <div className="flex items-center gap-1">
                        <MapPin className="w-4 h-4" />
                        <span>Last visit: {new Date(patient.lastVisit).toLocaleDateString()}</span>
                      </div>
                    </div>
                    {patient.condition && (
                      <div className="mt-2">
                        <Badge variant="outline" className="text-xs">
                          {patient.condition}
                        </Badge>
                      </div>
                    )}
                  </div>
                </div>

                <div className="flex gap-2">
                  <Button variant="outline" size="sm">
                    <Eye className="w-4 h-4 mr-2" />
                    View
                  </Button>
                  <Button variant="outline" size="sm">
                    <Edit className="w-4 h-4 mr-2" />
                    Edit
                  </Button>
                  {/* Updated "Schedule" button to open NewAppointmentDialog */}
                  <Button variant="medical" size="sm" onClick={() => handleSchedule(patient.id)}>
                    <Calendar className="w-4 h-4 mr-2" />
                    Schedule
                  </Button>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {filteredPatients.length === 0 && (
        <Card className="medical-card">
          <CardContent className="p-12 text-center">
            <div className="w-16 h-16 bg-muted rounded-full flex items-center justify-center mx-auto mb-4">
              <Search className="w-8 h-8 text-muted-foreground" />
            </div>
            <h3 className="text-lg font-semibold text-foreground mb-2">No patients found</h3>
            <p className="text-muted-foreground mb-4">
              Try adjusting your search terms or add a new patient.
            </p>
            <Button variant="medical" onClick={() => setShowNewPatientDialog(true)}>
              <Plus className="w-4 h-4 mr-2" />
              Add New Patient
            </Button>
          </CardContent>
        </Card>
      )}

      {/* Summary Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card className="medical-card">
          <CardContent className="p-4 text-center">
            <p className="text-2xl font-bold text-primary">{patients.length}</p>
            <p className="text-sm text-muted-foreground">Total Patients</p>
          </CardContent>
        </Card>
        <Card className="medical-card">
          <CardContent className="p-4 text-center">
            <p className="text-2xl font-bold text-success">
              {patients.filter(p => p.status === 'active').length}
            </p>
            <p className="text-sm text-muted-foreground">Active Patients</p>
          </CardContent>
        </Card>
        <Card className="medical-card">
          <CardContent className="p-4 text-center">
            <p className="text-2xl font-bold text-secondary">
              {patients.filter(p => new Date(p.lastVisit) >= new Date(Date.now() - 7 * 24 * 60 * 60 * 1000)).length}
            </p>
            <p className="text-sm text-muted-foreground">Recent Visits</p>
          </CardContent>
        </Card>
        <Card className="medical-card">
          <CardContent className="p-4 text-center">
            <p className="text-2xl font-bold text-accent">
              {Math.round(patients.reduce((sum, p) => sum + calculateAge(p.dateOfBirth), 0) / patients.length)}
            </p>
            <p className="text-sm text-muted-foreground">Average Age</p>
          </CardContent>
        </Card>
      </div>

      {/* NewPatientDialog */}
      <NewPatientDialog 
        open={showNewPatientDialog} 
        onOpenChange={setShowNewPatientDialog} 
      />

      {/* NewAppointmentDialog with selectedPatientId for context */}
      <NewAppointmentDialog 
        open={showNewAppointmentDialog} 
        onOpenChange={setShowNewAppointmentDialog} 
        patientId={selectedPatientId}  // Pass selected patient ID if needed
      />
    </div>
  );
};

export default Patients;
