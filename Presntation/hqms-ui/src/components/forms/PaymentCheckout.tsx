import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { toast } from 'sonner';

const paymentMethods = [
    { id: 'cash', label: 'Cash' },
    { id: 'upi', label: 'UPI' },
    { id: 'debit', label: 'Debit Card' },
    { id: 'credit', label: 'Credit Card' }
];

const PaymentCheckout = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const appointment = location.state?.appointment;

    const [selectedMethod, setSelectedMethod] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    if (!appointment) {
        return (
            <div className="p-6 text-center">
                <p className="text-lg font-semibold text-foreground mb-4">No appointment data found.</p>
                <Button onClick={() => navigate('/')}>Go to Home</Button>
            </div>
        );
    }

    const handlePayment = () => {
        if (!selectedMethod) {
            toast.error('Please select a payment method');
            return;
        }
        setLoading(true);

        // Simulate payment processing
        setTimeout(() => {
            setLoading(false);
            toast.success(`Payment successful using ${selectedMethod.toUpperCase()}`);

            // Redirect after payment success, e.g., to payment history, appointments, or dashboard
            navigate('/');
        }, 1500);
    };

    return (
        <div className="max-w-lg mx-auto p-6 space-y-6">
            <Card>
                <CardHeader>
                    <CardTitle>Confirm Appointment Payment</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="space-y-2">
                        <p><strong>Patient Name:</strong> {appointment.patientName}</p>
                        <p><strong>Doctor:</strong> {appointment.doctorName}</p>
                        <p><strong>Date:</strong> {appointment.date}</p>
                        <p><strong>Time:</strong> {appointment.time}</p>
                        <p><strong>Appointment Type:</strong> {appointment.appointmentType || 'N/A'}</p>
                    </div>
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Select Payment Method</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="flex flex-col space-y-4">
                        {paymentMethods.map((method) => (
                            <label key={method.id} className="flex items-center gap-3 cursor-pointer">
                                <input
                                    type="radio"
                                    name="payment-method"
                                    value={method.id}
                                    checked={selectedMethod === method.id}
                                    onChange={() => setSelectedMethod(method.id)}
                                    className="cursor-pointer"
                                />
                                <span>{method.label}</span>
                            </label>
                        ))}
                    </div>
                </CardContent>
            </Card>

            <div className="flex justify-end gap-4">
                <Button variant="outline" onClick={() => navigate(-1)} disabled={loading}>
                    Cancel
                </Button>
                <Button onClick={handlePayment} disabled={!selectedMethod || loading}>
                    {loading ? 'Processing...' : 'Pay Now'}
                </Button>
            </div>
        </div>

    );
};

export default PaymentCheckout;
