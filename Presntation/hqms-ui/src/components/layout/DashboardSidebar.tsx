import React from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import { 
  LayoutDashboard, 
  Users, 
  Calendar, 
  Clock, 
  Bell, 
  CreditCard, 
  Settings,
  Activity,
  FileText,
  User
} from 'lucide-react';
import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  useSidebar,
} from '@/components/ui/sidebar';
import { useAuth } from '@/context/AuthContext';
import { cn } from '@/lib/utils';

const navigationItems = [
  {
    title: 'Dashboard',
    url: '/dashboard',
    icon: LayoutDashboard,
    roles: ['admin', 'doctor', 'patient', 'pod']
  },
  {
    title: 'Patients',
    url: '/patients',
    icon: Users,
    roles: ['admin', 'doctor', 'pod']
  },
  {
    title: 'Appointments',
    url: '/appointments',
    icon: Calendar,
    roles: ['admin', 'doctor', 'patient', 'pod']
  },
  {
    title: 'Queue',
    url: '/queue',
    icon: Clock,
    roles: ['admin', 'doctor', 'pod']
  },
  {
    title: 'Notifications',
    url: '/notifications',
    icon: Bell,
    roles: ['admin', 'doctor', 'patient', 'pod']
  },
  {
    title: 'Payments',
    url: '/payments',
    icon: CreditCard,
    roles: ['admin', 'patient', 'pod']
  },
  {
    title: 'Reports',
    url: '/reports',
    icon: FileText,
    roles: ['admin', 'doctor']
  }
];

const bottomNavItems = [
  {
    title: 'Profile',
    url: '/profile',
    icon: User,
    roles: ['admin', 'doctor', 'patient', 'pod']
  },
  {
    title: 'Settings',
    url: '/settings',
    icon: Settings,
    roles: ['admin', 'doctor', 'patient', 'pod']
  }
];

export const DashboardSidebar: React.FC = () => {
  const { user, hasRole } = useAuth();
  const { state } = useSidebar();
  const location = useLocation();
  
  const isCollapsed = state === 'collapsed';

  const getNavItemClass = (url: string) => {
    const isActive = location.pathname === url || 
                    (url !== '/dashboard' && location.pathname.startsWith(url));
    
    return cn(
      "flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-all duration-200",
      isActive 
        ? "bg-primary text-primary-foreground shadow-medical" 
        : "text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground"
    );
  };

  const filteredNavItems = navigationItems.filter(item => 
    item.roles.some(role => hasRole(role))
  );

  const filteredBottomItems = bottomNavItems.filter(item => 
    item.roles.some(role => hasRole(role))
  );

  return (
    <Sidebar className="border-r border-sidebar-border bg-sidebar shadow-card">
      <SidebarContent className="flex flex-col h-full">
        {/* Header */}
        <div className="p-6 border-b border-sidebar-border">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-gradient-medical rounded-lg flex items-center justify-center">
              <Activity className="w-6 h-6 text-white" />
            </div>
            {!isCollapsed && (
              <div>
                <h1 className="text-lg font-bold text-sidebar-foreground">HQMS</h1>
                <p className="text-xs text-sidebar-foreground/70">Healthcare Quality</p>
              </div>
            )}
          </div>
        </div>

        {/* Main Navigation */}
        <div className="flex-1 py-4">
          <SidebarGroup>
            <SidebarGroupLabel className="text-xs font-semibold text-sidebar-foreground/60 px-6 mb-2">
              Main Menu
            </SidebarGroupLabel>
            <SidebarGroupContent className="px-3">
              <SidebarMenu className="space-y-1">
                {filteredNavItems.map((item) => (
                  <SidebarMenuItem key={item.title}>
                    <SidebarMenuButton asChild>
                      <NavLink to={item.url} className={getNavItemClass(item.url)}>
                        <item.icon className="w-5 h-5" />
                        {!isCollapsed && <span>{item.title}</span>}
                      </NavLink>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                ))}
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        </div>

        {/* User Info & Bottom Navigation */}
        <div className="mt-auto">
          {/* User Profile Section */}
          {user && !isCollapsed && (
            <div className="px-6 py-4 border-t border-sidebar-border">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-gradient-wellness rounded-full flex items-center justify-center">
                  <span className="text-sm font-semibold text-white">
                    {user.firstName[0]}{user.lastName[0]}
                  </span>
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-sidebar-foreground truncate">
                    {user.firstName} {user.lastName}
                  </p>
                  <p className="text-xs text-sidebar-foreground/70 capitalize truncate">
                    {user.role}
                  </p>
                </div>
              </div>
            </div>
          )}

          {/* Bottom Navigation */}
          <SidebarGroup>
            <SidebarGroupContent className="px-3 pb-4">
              <SidebarMenu className="space-y-1">
                {filteredBottomItems.map((item) => (
                  <SidebarMenuItem key={item.title}>
                    <SidebarMenuButton asChild>
                      <NavLink to={item.url} className={getNavItemClass(item.url)}>
                        <item.icon className="w-5 h-5" />
                        {!isCollapsed && <span>{item.title}</span>}
                      </NavLink>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                ))}
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        </div>
      </SidebarContent>
    </Sidebar>
  );
};