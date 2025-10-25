import React, { useState } from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { useAuth } from '@/contexts/AuthContext';
import { 
  CreditCard, 
  DollarSign, 
  Receipt, 
  Search,
  Download,
  Eye,
  Clock,
  CheckCircle,
  AlertTriangle
} from 'lucide-react';

const Payments = () => {
  const { user } = useAuth();
  const [searchTerm, setSearchTerm] = useState('');

  // Mock payment data
  const payments = [
    {
      id: 'PAY001',
      patientName: 'John Smith',
      amount: 250.00,
      service: 'Cardiology Consultation',
      date: '2024-09-05',
      status: 'paid',
      method: 'Card',
      invoiceId: 'INV001'
    },
    {
      id: 'PAY002',
      patientName: 'Emily Johnson',
      amount: 180.00,
      service: 'Neurology Follow-up',
      date: '2024-09-06',
      status: 'pending',
      method: 'Insurance',
      invoiceId: 'INV002'
    },
    {
      id: 'PAY003',
      patientName: 'Robert Davis',
      amount: 1200.00,
      service: 'Orthopedic Surgery',
      date: '2024-09-04',
      status: 'paid',
      method: 'Bank Transfer',
      invoiceId: 'INV003'
    },
    {
      id: 'PAY004',
      patientName: 'Maria Garcia',
      amount: 75.00,
      service: 'General Check-up',
      date: '2024-09-03',
      status: 'overdue',
      method: 'Cash',
      invoiceId: 'INV004'
    }
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'paid': return 'bg-success text-success-foreground';
      case 'pending': return 'bg-warning text-warning-foreground';
      case 'overdue': return 'bg-destructive text-destructive-foreground';
      default: return 'bg-secondary text-secondary-foreground';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'paid': return <CheckCircle className="w-4 h-4" />;
      case 'pending': return <Clock className="w-4 h-4" />;
      case 'overdue': return <AlertTriangle className="w-4 h-4" />;
      default: return <Receipt className="w-4 h-4" />;
    }
  };

  const filteredPayments = payments.filter(payment =>
    payment.patientName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    payment.service.toLowerCase().includes(searchTerm.toLowerCase()) ||
    payment.invoiceId.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const totalRevenue = payments.filter(p => p.status === 'paid').reduce((sum, p) => sum + p.amount, 0);
  const pendingAmount = payments.filter(p => p.status === 'pending').reduce((sum, p) => sum + p.amount, 0);
  const overdueAmount = payments.filter(p => p.status === 'overdue').reduce((sum, p) => sum + p.amount, 0);

  return (
    <div className="container mx-auto p-6 space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Payments</h1>
          <p className="text-muted-foreground">
            {user?.role === 'patient' ? 'Your payment history and invoices' : 'Manage all payments and billing'}
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
            <Input
              placeholder="Search payments..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 w-64"
            />
          </div>
          <Button variant="outline">
            <Download className="w-4 h-4 mr-2" />
            Export
          </Button>
        </div>
      </div>

      {/* Payment Summary Cards */}
      {(user?.role === 'admin' || user?.role === 'receptionist') && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <Card className="medical-card">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
              <DollarSign className="h-4 w-4 text-success" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-success">${totalRevenue.toFixed(2)}</div>
              <p className="text-xs text-muted-foreground">Collected payments</p>
            </CardContent>
          </Card>

          <Card className="medical-card">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Pending Payments</CardTitle>
              <Clock className="h-4 w-4 text-warning" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-warning">${pendingAmount.toFixed(2)}</div>
              <p className="text-xs text-muted-foreground">Awaiting payment</p>
            </CardContent>
          </Card>

          <Card className="medical-card">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Overdue Amount</CardTitle>
              <AlertTriangle className="h-4 w-4 text-destructive" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-destructive">${overdueAmount.toFixed(2)}</div>
              <p className="text-xs text-muted-foreground">Past due invoices</p>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Payment History */}
      <Card className="medical-card">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Receipt className="w-5 h-5" />
            Payment History
          </CardTitle>
          <CardDescription>
            {filteredPayments.length} payment records found
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {filteredPayments.map((payment) => (
              <div key={payment.id} className="flex items-center justify-between p-4 border border-border rounded-lg hover:bg-accent/50 transition-colors">
                <div className="flex items-center gap-4">
                  <div className="flex items-center justify-center w-10 h-10 bg-gradient-secondary rounded-full">
                    <CreditCard className="w-5 h-5 text-white" />
                  </div>
                  <div>
                    <p className="font-medium text-foreground">{payment.patientName}</p>
                    <p className="text-sm text-muted-foreground">{payment.service}</p>
                    <p className="text-xs text-muted-foreground">
                      {payment.date} • {payment.method} • {payment.invoiceId}
                    </p>
                  </div>
                </div>
                <div className="flex items-center gap-4">
                  <div className="text-right">
                    <p className="font-bold text-lg">${payment.amount.toFixed(2)}</p>
                    <Badge className={getStatusColor(payment.status)}>
                      {getStatusIcon(payment.status)}
                      <span className="ml-1 capitalize">{payment.status}</span>
                    </Badge>
                  </div>
                  <div className="flex gap-2">
                    <Button variant="outline" size="sm">
                      <Eye className="w-4 h-4 mr-1" />
                      View
                    </Button>
                    <Button variant="outline" size="sm">
                      <Download className="w-4 h-4 mr-1" />
                      Invoice
                    </Button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default Payments;