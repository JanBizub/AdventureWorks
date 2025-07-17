import React from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  Typography,
  Paper,
  Box,
  Grid,
  Card,
  CardContent,
  CardActions,
  Button,
  Chip
} from '@mui/material';
import {
  MonitorHeart as MonitorIcon,
  Settings as SettingsIcon,
  Message as MessageIcon,
  Speed as SpeedIcon
} from '@mui/icons-material';

function Dashboard() {
  return (
    <Box>
      <Paper elevation={3} sx={{ p: 4, mb: 4 }}>
        <Typography variant="h3" component="h1" gutterBottom>
          EDI System Dashboard
        </Typography>
        <Typography variant="h6" color="text.secondary">
          Welcome to the Electronic Data Interchange monitoring and management system
        </Typography>
      </Paper>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card elevation={2} sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
            <CardContent sx={{ flexGrow: 1 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <MessageIcon sx={{ fontSize: 40, color: 'primary.main', mr: 2 }} />
                <Box>
                  <Typography variant="h5" component="h2">
                    EDI Messages Monitor
                  </Typography>
                  <Chip label="Real-time" color="success" size="small" />
                </Box>
              </Box>
              <Typography variant="body1" color="text.secondary" paragraph>
                Monitor incoming and outgoing EDI messages in real-time. View message status, 
                processing details, and track message flow through the system.
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Features: Live updates, message filtering, status tracking, and detailed message information.
              </Typography>
            </CardContent>
            <CardActions>
              <Button 
                component={RouterLink} 
                to="/messages" 
                variant="contained" 
                startIcon={<SpeedIcon />}
                fullWidth
              >
                View Messages
              </Button>
            </CardActions>
          </Card>
        </Grid>

        <Grid item xs={12} md={6}>
          <Card elevation={2} sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
            <CardContent sx={{ flexGrow: 1 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <MonitorIcon sx={{ fontSize: 40, color: 'secondary.main', mr: 2 }} />
                <Box>
                  <Typography variant="h5" component="h2">
                    Services Status
                  </Typography>
                  <Chip label="Coming Soon" color="warning" size="small" />
                </Box>
              </Box>
              <Typography variant="body1" color="text.secondary" paragraph>
                Monitor the health and status of all EDI system services including APIs, 
                databases, and external connections.
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Features: Service health monitoring, uptime tracking, performance metrics, and alerts.
              </Typography>
            </CardContent>
            <CardActions>
              <Button 
                component={RouterLink} 
                to="/services" 
                variant="outlined" 
                startIcon={<SettingsIcon />}
                fullWidth
              >
                View Services
              </Button>
            </CardActions>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}

export default Dashboard;
