import React, { useState } from 'react';
import { Card, CardContent } from '@/components/ui/card';
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
import { NewDoctorDialog } from '@/components/forms/NewDoctorDialog'; // Assume this dialog exists

const Doctors = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [showNewDoctorDialog, setShowNewDoctorDialog] = useState(false);

  // Mock doctors data
  const doctors = [
    {
      id: '1',
      firstName: 'Sarah',
      lastName: 'Williams',
      email: 'sarah.williams@email.com',
      phone: '+1 (555) 111-2222',
      department: 'Cardiology',
      specialization: 'Interventional Cardiology',
      experience: 12, // years
      status: 'active',
      lastActive: '2024-08-25'
    },
    {
      id: '2',
      firstName: 'Michael',
      lastName: 'Brown',
      email: 'michael.brown@email.com',
      phone: '+1 (555) 222-3333',
      department: 'Neurology',
      specialization: 'Pediatric Neurology',
      experience: 8,
      status: 'active',
      lastActive: '2024-08-20'
    },
    {
      id: '3',
      firstName: 'Lisa',
      lastName: 'Chen',
      email: 'lisa.chen@email.com',
      phone: '+1 (555) 333-4444',
      department: 'Orthopedics',
      specialization: 'Spine Surgery',
      experience: 15,
      status: 'inactive',
      lastActive: '2024-06-15'
    },
    {
      id: '4',
      firstName: 'James',
      lastName: 'Lee',
      email: 'james.lee@email.com',
      phone: '+1 (555) 444-5555',
      department: 'General Medicine',
      specialization: 'Internal Medicine',
      experience: 10,
      status: 'active',
      lastActive: '2024-08-28'
    }
  ];

  // Filter doctors by first/last name, email, or department
  const filteredDoctors = doctors.filter(doctor =>
    `${doctor.firstName} ${doctor.lastName}`.toLowerCase().includes(searchTerm.toLowerCase()) ||
    doctor.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
    doctor.department.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getStatusColor = (status: string) => {
    return status === 'active' ? 'bg-success text-success-foreground' : 'bg-muted text-muted-foreground';
  };

  return (
    <div className="p-6 space-y-6 animate-fade-in">
      {/* Header + Add Doctor button */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Doctor Management</h1>
          <p className="text-muted-foreground">
            Manage doctor profiles and information
          </p>
        </div>
        <Button variant="medical" onClick={() => setShowNewDoctorDialog(true)}>
          <Plus className="w-4 h-4 mr-2" />
          Add New Doctor
        </Button>
      </div>

      {/* Search and Filters */}
      <Card className="medical-card">
        <CardContent className="p-6">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1 relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
              <Input
                placeholder="Search doctors by name, email, or department..."
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

      {/* Doctors List */}
      <div className="grid gap-4">
        {filteredDoctors.map((doctor) => (
          <Card key={doctor.id} className="medical-card hover:shadow-hover transition-medical">
            <CardContent className="p-6">
              <div className="flex flex-col lg:flex-row lg:items-center justify-between gap-4">
                <div className="flex items-center gap-4">
                  <div className="w-12 h-12 bg-gradient-primary rounded-full flex items-center justify-center">
                    <span className="text-white font-semibold text-lg">
                      {doctor.firstName[0]}{doctor.lastName[0]}
                    </span>
                  </div>
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <h3 className="text-lg font-semibold text-foreground">
                        {doctor.firstName} {doctor.lastName}
                      </h3>
                      <Badge className={getStatusColor(doctor.status)}>
                        {doctor.status}
                      </Badge>
                    </div>
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-2 text-sm text-muted-foreground">
                      <div className="flex items-center gap-1">
                        <Mail className="w-4 h-4" />
                        <span>{doctor.email}</span>
                      </div>
                      <div className="flex items-center gap-1">
                        <Phone className="w-4 h-4" />
                        <span>{doctor.phone}</span>
                      </div>
                      <div className="flex items-center gap-1">
                        <MapPin className="w-4 h-4" />
                        <span>{doctor.department}</span>
                      </div>
                      <div className="flex items-center gap-1">
                        <Calendar className="w-4 h-4" />
                        <span>{doctor.experience} years experience</span>
                      </div>
                    </div>
                    <div className="mt-2 text-sm text-muted-foreground italic">
                      Specialization: {doctor.specialization}
                    </div>
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
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {filteredDoctors.length === 0 && (
        <Card className="medical-card">
          <CardContent className="p-12 text-center">
            <div className="w-16 h-16 bg-muted rounded-full flex items-center justify-center mx-auto mb-4">
              <Search className="w-8 h-8 text-muted-foreground" />
            </div>
            <h3 className="text-lg font-semibold text-foreground mb-2">No doctors found</h3>
            <p className="text-muted-foreground mb-4">
              Try adjusting your search terms or add a new doctor.
            </p>
            <Button variant="medical" onClick={() => setShowNewDoctorDialog(true)}>
              <Plus className="w-4 h-4 mr-2" />
              Add New Doctor
            </Button>
          </CardContent>
        </Card>
      )}

      {/* NewDoctorDialog for adding new doctors */}
      <NewDoctorDialog 
        open={showNewDoctorDialog} 
        onOpenChange={setShowNewDoctorDialog} 
      />
    </div>
  );
};

export default Doctors;
