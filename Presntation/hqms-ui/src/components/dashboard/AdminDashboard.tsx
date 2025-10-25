import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { 
  Users, 
  Calendar, 
  Activity, 
  CreditCard, 
  TrendingUp, 
  Clock,
  AlertTriangle,
  CheckCircle
} from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import dashboardPreview from '@/assets/dashboard-preview.jpg';

const AdminDashboard = () => {
  const navigate = useNavigate();

  const handleAnalytics = () => {
    console.log('Opening analytics dashboard...');
    // Navigate to analytics or show analytics modal
  };

  const handleAlerts = () => {
    console.log('Opening alerts panel...');
    // Show alerts modal or navigate to alerts page
  };
  const stats = [
    {
      title: 'Total Patients',
      value: '2,847',
      change: '+12%',
      icon: Users,
      color: 'text-primary',
      bgColor: 'bg-primary-light',
    },
    {
      title: 'Today\'s Appointments',
      value: '47',
      change: '+8%',
      icon: Calendar,
      color: 'text-secondary',
      bgColor: 'bg-secondary-light',
    },
    {
      title: 'Queue Length',
      value: '23',
      change: '-15%',
      icon: Activity,
      color: 'text-accent',
      bgColor: 'bg-accent-light',
    },
    {
      title: 'Revenue Today',
      value: '$12,450',
      change: '+22%',
      icon: CreditCard,
      color: 'text-success',
      bgColor: 'bg-success/10',
    },
  ];

  const recentActivity = [
    { id: 1, action: 'New patient registered', user: 'John Smith', time: '5 min ago', type: 'success' },
    { id: 2, action: 'Appointment scheduled', user: 'Dr. Johnson', time: '12 min ago', type: 'info' },
    { id: 3, action: 'Payment processed', user: 'Mary Wilson', time: '18 min ago', type: 'success' },
    { id: 4, action: 'Queue position updated', user: 'System', time: '25 min ago', type: 'warning' },
  ];

  const departmentStats = [
    { name: 'Cardiology', patients: 12, waiting: 3, avgWait: '25 min' },
    { name: 'Orthopedics', patients: 8, waiting: 5, avgWait: '18 min' },
    { name: 'Pediatrics', patients: 15, waiting: 2, avgWait: '12 min' },
    { name: 'General Medicine', patients: 20, waiting: 8, avgWait: '30 min' },
  ];

  return (
    <div className="p-6 space-y-6 animate-fade-in">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Admin Dashboard</h1>
          <p className="text-muted-foreground">
            Hospital overview and management controls
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" onClick={handleAnalytics}>
            <TrendingUp className="w-4 h-4 mr-2" />
            Analytics
          </Button>
          <Button variant="medical" size="sm" onClick={handleAlerts}>
            <AlertTriangle className="w-4 h-4 mr-2" />
            Alerts
          </Button>
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <Card key={index} className="medical-card hover:shadow-hover transition-medical">
              <CardContent className="p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      {stat.title}
                    </p>
                    <div className="flex items-center gap-2">
                      <p className="text-2xl font-bold text-foreground">
                        {stat.value}
                      </p>
                      <Badge 
                        variant={stat.change.startsWith('+') ? 'default' : 'secondary'}
                        className="text-xs"
                      >
                        {stat.change}
                      </Badge>
                    </div>
                  </div>
                  <div className={`p-3 rounded-full ${stat.bgColor}`}>
                    <Icon className={`w-6 h-6 ${stat.color}`} />
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Department Overview */}
        <Card className="medical-card">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Activity className="w-5 h-5" />
              Department Overview
            </CardTitle>
            <CardDescription>
              Real-time status across all departments
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {departmentStats.map((dept, index) => (
                <div key={index} className="flex items-center justify-between p-3 bg-muted rounded-lg">
                  <div>
                    <h4 className="font-medium text-foreground">{dept.name}</h4>
                    <p className="text-sm text-muted-foreground">
                      {dept.patients} patients • {dept.waiting} waiting
                    </p>
                  </div>
                  <div className="text-right">
                    <div className="flex items-center gap-2">
                      <Clock className="w-4 h-4 text-muted-foreground" />
                      <span className="text-sm font-medium">{dept.avgWait}</span>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Recent Activity */}
        <Card className="medical-card">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <CheckCircle className="w-5 h-5" />
              Recent Activity
            </CardTitle>
            <CardDescription>
              Latest system activities and updates
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {recentActivity.map((activity) => (
                <div key={activity.id} className="flex items-start gap-3">
                  <div className={`w-2 h-2 rounded-full mt-2 ${
                    activity.type === 'success' ? 'bg-success' :
                    activity.type === 'warning' ? 'bg-warning' : 'bg-primary'
                  }`} />
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-foreground">
                      {activity.action}
                    </p>
                    <p className="text-sm text-muted-foreground">
                      {activity.user} • {activity.time}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* System Overview */}
      <Card className="medical-card">
        <CardHeader>
          <CardTitle>System Performance</CardTitle>
          <CardDescription>
            Dashboard metrics and system health overview
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col lg:flex-row gap-6">
            <div className="flex-1">
              <img
                src={dashboardPreview}
                alt="Dashboard Analytics"
                className="w-full h-48 object-cover rounded-lg shadow-card"
              />
            </div>
            <div className="flex-1 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="text-center p-4 bg-muted rounded-lg">
                  <p className="text-2xl font-bold text-primary">98.5%</p>
                  <p className="text-sm text-muted-foreground">System Uptime</p>
                </div>
                <div className="text-center p-4 bg-muted rounded-lg">
                  <p className="text-2xl font-bold text-secondary">47ms</p>
                  <p className="text-sm text-muted-foreground">Avg Response</p>
                </div>
              </div>
              <div className="text-center p-4 bg-muted rounded-lg">
                <p className="text-2xl font-bold text-accent">15,234</p>
                <p className="text-sm text-muted-foreground">Total Appointments This Month</p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default AdminDashboard;