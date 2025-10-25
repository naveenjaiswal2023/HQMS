import React, { useState } from 'react';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { toast } from 'sonner';
import { Calendar, Clock, User, Stethoscope } from 'lucide-react';
import { useNavigate } from 'react-router-dom';  // Import useNavigate

interface NewAppointmentDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  patientId?: string; 
}

export const NewAppointmentDialog: React.FC<NewAppointmentDialogProps> = ({ open, onOpenChange }) => {
  const navigate = useNavigate();  // Initialize navigate hook

  const [formData, setFormData] = useState({
    patientName: '',
    doctorId: '',
    department: '',
    appointmentDate: '',
    appointmentTime: '',
    appointmentType: '',
    reason: '',
    notes: ''
  });

  const doctors = [
    { id: '1', name: 'Dr. Sarah Wilson', department: 'Cardiology' },
    { id: '2', name: 'Dr. Michael Brown', department: 'Neurology' },
    { id: '3', name: 'Dr. Lisa Chen', department: 'Orthopedics' },
    { id: '4', name: 'Dr. James Wilson', department: 'General Medicine' },
    { id: '5', name: 'Dr. Emily Davis', department: 'Pediatrics' }
  ];

  const appointmentTypes = [
    'Consultation',
    'Follow-up',
    'Routine Check-up',
    'Emergency',
    'Surgery Consultation',
    'Lab Results Review'
  ];

  const timeSlots = [
    '09:00', '09:30', '10:00', '10:30', '11:00', '11:30',
    '14:00', '14:30', '15:00', '15:30', '16:00', '16:30'
  ];

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.patientName || !formData.doctorId || !formData.appointmentDate || !formData.appointmentTime) {
      toast.error('Please fill in all required fields');
      return;
    }
    
    // Simulate successful appointment booking
    setTimeout(() => {
      const selectedDoctor = doctors.find(d => d.id === formData.doctorId);
      toast.success(`Appointment scheduled for ${formData.patientName} with ${selectedDoctor?.name} on ${formData.appointmentDate} at ${formData.appointmentTime}`);

      onOpenChange(false);

      // Redirect to payment checkout page with appointment data via state
      navigate('/payment-checkout', {
        state: {
          appointment: {
            patientName: formData.patientName,
            doctorName: selectedDoctor?.name || '',
            date: formData.appointmentDate,
            time: formData.appointmentTime,
            appointmentType: formData.appointmentType
          }
        }
      });

      // Reset form fields
      setFormData({
        patientName: '',
        doctorId: '',
        department: '',
        appointmentDate: '',
        appointmentTime: '',
        appointmentType: '',
        reason: '',
        notes: ''
      });
    }, 1000);
  };

  const handleInputChange = (field: string, value: string) => {
    setFormData(prev => {
      const newData = { ...prev, [field]: value };
      if (field === 'doctorId') {
        const selectedDoctor = doctors.find(d => d.id === value);
        if (selectedDoctor) newData.department = selectedDoctor.department;
      }
      return newData;
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Calendar className="w-5 h-5" />
            Schedule New Appointment
          </DialogTitle>
          <DialogDescription>
            Book a new appointment for a patient
          </DialogDescription>
        </DialogHeader>
        
        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Patient Name */}
          <div className="space-y-2">
            <Label htmlFor="patientName">Patient Name *</Label>
            <div className="relative">
              <User className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
              <Input
                id="patientName"
                value={formData.patientName}
                onChange={(e) => handleInputChange('patientName', e.target.value)}
                placeholder="Enter patient name"
                className="pl-10"
                required
              />
            </div>
          </div>

          {/* Doctor and Department */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="doctorId">Doctor *</Label>
              <Select value={formData.doctorId} onValueChange={(value) => handleInputChange('doctorId', value)}>
                <SelectTrigger>
                  <SelectValue placeholder="Select doctor" />
                </SelectTrigger>
                <SelectContent>
                  {doctors.map(doctor => (
                    <SelectItem key={doctor.id} value={doctor.id}>
                      <div className="flex items-center gap-2">
                        <Stethoscope className="w-4 h-4" />
                        {doctor.name} - {doctor.department}
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="department">Department</Label>
              <Input
                id="department"
                value={formData.department}
                readOnly
                placeholder="Auto-filled based on doctor"
                className="bg-muted"
              />
            </div>
          </div>

          {/* Appointment Date and Time */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="appointmentDate">Date *</Label>
              <div className="relative">
                <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
                <Input
                  id="appointmentDate"
                  type="date"
                  value={formData.appointmentDate}
                  onChange={(e) => handleInputChange('appointmentDate', e.target.value)}
                  className="pl-10"
                  min={new Date().toISOString().split('T')[0]}
                  required
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="appointmentTime">Time *</Label>
              <Select value={formData.appointmentTime} onValueChange={(value) => handleInputChange('appointmentTime', value)}>
                <SelectTrigger>
                  <SelectValue placeholder="Select time slot" />
                </SelectTrigger>
                <SelectContent>
                  {timeSlots.map(time => (
                    <SelectItem key={time} value={time}>
                      <div className="flex items-center gap-2">
                        <Clock className="w-4 h-4" />
                        {time}
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Appointment Type */}
          <div className="space-y-2">
            <Label htmlFor="appointmentType">Appointment Type</Label>
            <Select value={formData.appointmentType} onValueChange={(value) => handleInputChange('appointmentType', value)}>
              <SelectTrigger>
                <SelectValue placeholder="Select appointment type" />
              </SelectTrigger>
              <SelectContent>
                {appointmentTypes.map(type => (
                  <SelectItem key={type} value={type}>
                    {type}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Reason and Notes */}
          <div className="space-y-2">
            <Label htmlFor="reason">Reason for Visit</Label>
            <Textarea
              id="reason"
              value={formData.reason}
              onChange={(e) => handleInputChange('reason', e.target.value)}
              placeholder="Brief description of the reason for this appointment"
              rows={2}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="notes">Additional Notes</Label>
            <Textarea
              id="notes"
              value={formData.notes}
              onChange={(e) => handleInputChange('notes', e.target.value)}
              placeholder="Any additional notes or special requirements"
              rows={2}
            />
          </div>

          {/* Action Buttons */}
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              Cancel
            </Button>
            <Button type="submit" variant="medical">
              Schedule Appointment
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
};
