import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/contexts/AuthContext';
import { 
  User, 
  LogOut, 
  Settings, 
  Bell,
  Activity,
  Users,
  Calendar,
  CreditCard,
  BarChart3,
  Bed,
  Heart
} from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Badge } from '@/components/ui/badge';

const Navbar = () => {
  const { user, logout } = useAuth();
  const location = useLocation();

  const getNavigationItems = () => {
    if (!user) return [];

    const baseItems = [
      { name: 'Dashboard', href: '/dashboard', icon: BarChart3 },
    ];

    switch (user.role) {
      case 'admin':
        return [
          ...baseItems,
          { name: 'Patients', href: '/patients', icon: Users },
          { name: 'Doctors', href: '/doctors', icon: User },
          { name: 'Appointments', href: '/appointments', icon: Calendar },
          { name: 'Queue', href: '/queue', icon: Activity },
          { name: 'Pod', href: '/pod', icon: Bed },
          { name: 'Nursing', href: '/nursing', icon: Heart },
          { name: 'Payments', href: '/payments', icon: CreditCard },
        ];
      case 'doctor':
        return [
          ...baseItems,
          { name: 'My Patients', href: '/patients', icon: Users },
          { name: 'Appointments', href: '/appointments', icon: Calendar },
          { name: 'Queue', href: '/queue', icon: Activity },
          { name: 'Pod', href: '/pod', icon: Bed },
          { name: 'Nursing', href: '/nursing', icon: Heart },
        ];
      case 'receptionist':
        return [
          ...baseItems,
          { name: 'Patients', href: '/patients', icon: Users },
          { name: 'Appointments', href: '/appointments', icon: Calendar },
          { name: 'Queue', href: '/queue', icon: Activity },
          { name: 'Payments', href: '/payments', icon: CreditCard },
        ];
      case 'patient':
        return [
          ...baseItems,
          { name: 'My Appointments', href: '/appointments', icon: Calendar },
          { name: 'Queue Status', href: '/queue', icon: Activity },
          { name: 'Payments', href: '/payments', icon: CreditCard },
        ];
      default:
        return baseItems;
    }
  };

  const navigationItems = getNavigationItems();

  if (!user) return null;

  return (
    <nav className="bg-card border-b border-border sticky top-0 z-50 medical-card">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center">
            <Link to="/dashboard" className="flex items-center space-x-2">
              <div className="w-8 h-8 bg-gradient-primary rounded-lg flex items-center justify-center">
                <Activity className="w-5 h-5 text-white" />
              </div>
              <span className="text-xl font-bold text-foreground">HQMS</span>
            </Link>
            
            <div className="hidden md:ml-8 md:flex md:space-x-1">
              {navigationItems.map((item) => {
                const Icon = item.icon;
                const isActive = location.pathname === item.href;
                
                return (
                  <Link
                    key={item.name}
                    to={item.href}
                    className={`flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium transition-medical ${
                      isActive
                        ? 'bg-primary text-primary-foreground'
                        : 'text-muted-foreground hover:text-foreground hover:bg-accent'
                    }`}
                  >
                    <Icon className="w-4 h-4" />
                    <span>{item.name}</span>
                  </Link>
                );
              })}
            </div>
          </div>

          <div className="flex items-center space-x-4">
            <Button variant="ghost" size="icon" className="relative">
              <Bell className="w-5 h-5" />
              <Badge className="absolute -top-1 -right-1 w-5 h-5 flex items-center justify-center p-0 text-xs bg-destructive">
                3
              </Badge>
            </Button>

            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" className="flex items-center space-x-2">
                  <div className="w-8 h-8 bg-gradient-secondary rounded-full flex items-center justify-center">
                    <User className="w-4 h-4 text-white" />
                  </div>
                  <div className="hidden md:block text-left">
                    <div className="text-sm font-medium text-foreground">
                      {user.firstName} {user.lastName}
                    </div>
                    <div className="text-xs text-muted-foreground capitalize">
                      {user.role}
                    </div>
                  </div>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-56">
                <DropdownMenuLabel>My Account</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem>
                  <User className="mr-2 h-4 w-4" />
                  <span>Profile</span>
                </DropdownMenuItem>
                <DropdownMenuItem>
                  <Settings className="mr-2 h-4 w-4" />
                  <span>Settings</span>
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem onClick={logout} className="text-destructive">
                  <LogOut className="mr-2 h-4 w-4" />
                  <span>Logout</span>
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;