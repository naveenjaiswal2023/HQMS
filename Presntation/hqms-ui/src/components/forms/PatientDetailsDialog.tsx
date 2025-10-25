import React from 'react';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { 
  User, 
  Phone, 
  Mail, 
  Calendar, 
  MapPin, 
  Heart,
  FileText,
  Clock,
  AlertTriangle
} from 'lucide-react';

interface PatientDetailsDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  patientId?: string | number;
}

export const PatientDetailsDialog: React.FC<PatientDetailsDialogProps> = ({ open, onOpenChange, patientId }) => {
  // Mock patient data - in real app, this would be fetched based on patientId
  const patientData = {
    id: patientId || '12345',
    firstName: 'John',
    lastName: 'Smith',
    email: 'john.smith@email.com',
    phone: '+1 (555) 123-4567',
    dateOfBirth: '1985-06-15',
    gender: 'Male',
    address: '123 Main Street, Anytown, State 12345',
    emergencyContact: 'Jane Smith',
    emergencyPhone: '+1 (555) 987-6543',
    bloodType: 'O+',
    allergies: ['Penicillin', 'Peanuts'],
    lastVisit: '2024-08-15',
    nextAppointment: '2024-09-20',
    status: 'Active',
    medicalHistory: 'Hypertension, managed with medication. Previous surgery: Appendectomy (2019).',
    recentVisits: [
      { date: '2024-08-15', doctor: 'Dr. Sarah Wilson', reason: 'Annual Checkup', status: 'Completed' },
      { date: '2024-07-10', doctor: 'Dr. Michael Brown', reason: 'Follow-up', status: 'Completed' },
      { date: '2024-06-05', doctor: 'Dr. Sarah Wilson', reason: 'Consultation', status: 'Completed' }
    ],
    vitals: {
      lastRecorded: '2024-08-15',
      bloodPressure: '125/80',
      heartRate: '72 bpm',
      temperature: '98.6°F',
      weight: '175 lbs',
      height: '5\'10"'
    }
  };

  const calculateAge = (birthDate: string) => {
    const today = new Date();
    const birth = new Date(birthDate);
    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    return age;
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <User className="w-5 h-5" />
            Patient Details - {patientData.firstName} {patientData.lastName}
          </DialogTitle>
          <DialogDescription>
            Complete patient information and medical history
          </DialogDescription>
        </DialogHeader>
        
        <div className="space-y-6">
          {/* Basic Information */}
          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Basic Information</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <div className="flex items-center gap-2">
                  <User className="w-4 h-4 text-muted-foreground" />
                  <div>
                    <p className="text-sm text-muted-foreground">Full Name</p>
                    <p className="font-medium">{patientData.firstName} {patientData.lastName}</p>
                  </div>
                </div>
                
                <div className="flex items-center gap-2">
                  <Calendar className="w-4 h-4 text-muted-foreground" />
                  <div>
                    <p className="text-sm text-muted-foreground">Age</p>
                    <p className="font-medium">{calculateAge(patientData.dateOfBirth)} years old</p>
                  </div>
                </div>
                
                <div className="flex items-center gap-2">
                  <User className="w-4 h-4 text-muted-foreground" />
                  <div>
                    <p className="text-sm text-muted-foreground">Gender</p>
                    <p className="font-medium">{patientData.gender}</p>
                  </div>
                </div>
                
                <div className="flex items-center gap-2">
                  <Phone className="w-4 h-4 text-muted-foreground" />
                  <div>
                    <p className="text-sm text-muted-foreground">Phone</p>
                    <p className="font-medium">{patientData.phone}</p>
                  </div>
                </div>
                
                <div className="flex items-center gap-2">
                  <Mail className="w-4 h-4 text-muted-foreground" />
                  <div>
                    <p className="text-sm text-muted-foreground">Email</p>
                    <p className="font-medium">{patientData.email}</p>
                  </div>
                </div>
                
                <div className="flex items-center gap-2">
                  <Heart className="w-4 h-4 text-muted-foreground" />
                  <div>
                    <p className="text-sm text-muted-foreground">Blood Type</p>
                    <p className="font-medium">{patientData.bloodType}</p>
                  </div>
                </div>
              </div>
              
              <div className="mt-4">
                <div className="flex items-start gap-2">
                  <MapPin className="w-4 h-4 text-muted-foreground mt-1" />
                  <div>
                    <p className="text-sm text-muted-foreground">Address</p>
                    <p className="font-medium">{patientData.address}</p>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Emergency Contact */}
          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Emergency Contact</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-muted-foreground">Contact Name</p>
                  <p className="font-medium">{patientData.emergencyContact}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Contact Phone</p>
                  <p className="font-medium">{patientData.emergencyPhone}</p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Medical Information */}
          <Card>
            <CardHeader>
              <CardTitle className="text-lg flex items-center gap-2">
                <FileText className="w-5 h-5" />
                Medical Information
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <p className="text-sm text-muted-foreground mb-2">Allergies</p>
                <div className="flex gap-2">
                  {patientData.allergies.map((allergy, index) => (
                    <Badge key={index} variant="destructive" className="flex items-center gap-1">
                      <AlertTriangle className="w-3 h-3" />
                      {allergy}
                    </Badge>
                  ))}
                </div>
              </div>
              
              <div>
                <p className="text-sm text-muted-foreground mb-2">Medical History</p>
                <p className="text-sm bg-muted p-3 rounded-lg">{patientData.medicalHistory}</p>
              </div>
            </CardContent>
          </Card>

          {/* Latest Vitals */}
          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Latest Vitals</CardTitle>
              <CardDescription>Recorded on {patientData.vitals.lastRecorded}</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
                <div className="text-center p-3 bg-muted rounded-lg">
                  <p className="text-sm text-muted-foreground">Blood Pressure</p>
                  <p className="font-bold text-lg">{patientData.vitals.bloodPressure}</p>
                </div>
                <div className="text-center p-3 bg-muted rounded-lg">
                  <p className="text-sm text-muted-foreground">Heart Rate</p>
                  <p className="font-bold text-lg">{patientData.vitals.heartRate}</p>
                </div>
                <div className="text-center p-3 bg-muted rounded-lg">
                  <p className="text-sm text-muted-foreground">Temperature</p>
                  <p className="font-bold text-lg">{patientData.vitals.temperature}</p>
                </div>
                <div className="text-center p-3 bg-muted rounded-lg">
                  <p className="text-sm text-muted-foreground">Weight</p>
                  <p className="font-bold text-lg">{patientData.vitals.weight}</p>
                </div>
                <div className="text-center p-3 bg-muted rounded-lg">
                  <p className="text-sm text-muted-foreground">Height</p>
                  <p className="font-bold text-lg">{patientData.vitals.height}</p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Recent Visits */}
          <Card>
            <CardHeader>
              <CardTitle className="text-lg flex items-center gap-2">
                <Clock className="w-5 h-5" />
                Recent Visits
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {patientData.recentVisits.map((visit, index) => (
                  <div key={index} className="flex items-center justify-between p-3 bg-muted rounded-lg">
                    <div>
                      <p className="font-medium">{visit.reason}</p>
                      <p className="text-sm text-muted-foreground">{visit.doctor} • {visit.date}</p>
                    </div>
                    <Badge variant="secondary">{visit.status}</Badge>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>

          <div className="flex justify-end gap-3">
            <Button variant="outline" onClick={() => onOpenChange(false)}>
              Close
            </Button>
            <Button variant="medical">
              Edit Patient
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};