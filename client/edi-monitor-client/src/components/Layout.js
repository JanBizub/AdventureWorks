import React from 'react';
import { Outlet, Link as RouterLink, useLocation } from 'react-router-dom';
import {
  AppBar,
  Toolbar,
  Typography,
  Box,
  Button,
  Container
} from '@mui/material';
import {
  Dashboard as DashboardIcon,
  Message as MessageIcon,
  MonitorHeart as MonitorIcon
} from '@mui/icons-material';

function Layout() {
  const location = useLocation();

  const isActive = (path) => location.pathname === path;

  return (
    <Box sx={{ flexGrow: 1, minHeight: '100vh' }}>
      <AppBar position="static" elevation={0}>
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            EDI System Monitor
          </Typography>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button
              component={RouterLink}
              to="/"
              color="inherit"
              startIcon={<DashboardIcon />}
              variant={isActive('/') ? 'outlined' : 'text'}
              sx={{ 
                borderColor: isActive('/') ? 'white' : 'transparent',
                '&:hover': { backgroundColor: 'rgba(255, 255, 255, 0.1)' }
              }}
            >
              Dashboard
            </Button>
            <Button
              component={RouterLink}
              to="/messages"
              color="inherit"
              startIcon={<MessageIcon />}
              variant={isActive('/messages') ? 'outlined' : 'text'}
              sx={{ 
                borderColor: isActive('/messages') ? 'white' : 'transparent',
                '&:hover': { backgroundColor: 'rgba(255, 255, 255, 0.1)' }
              }}
            >
              Messages
            </Button>
            <Button
              component={RouterLink}
              to="/services"
              color="inherit"
              startIcon={<MonitorIcon />}
              variant={isActive('/services') ? 'outlined' : 'text'}
              sx={{ 
                borderColor: isActive('/services') ? 'white' : 'transparent',
                '&:hover': { backgroundColor: 'rgba(255, 255, 255, 0.1)' }
              }}
            >
              Services
            </Button>
          </Box>
        </Toolbar>
      </AppBar>

      <Box sx={{ mx: 2, mt: 4, mb: 4 }}>
        <Outlet />
      </Box>
    </Box>
  );
}

export default Layout;
