import React from 'react';
import {
  Typography,
  Paper,
  Box,
  Alert
} from '@mui/material';
import {
  Build as BuildIcon
} from '@mui/icons-material';

function ServicesStatus() {
  return (
    <Paper elevation={3} sx={{ p: 3 }}>
      <Box sx={{ mb: 3 }}>
        <Typography variant="h4" component="h2" gutterBottom>
          Services Status
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Monitor the health and status of all EDI system services
        </Typography>
      </Box>

      <Box sx={{ textAlign: 'center', py: 8 }}>
        <BuildIcon sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
        <Alert severity="info" sx={{ maxWidth: 600, mx: 'auto' }}>
          <Typography variant="h6" gutterBottom>
            Services Status Dashboard
          </Typography>
          <Typography variant="body2">
            This section will display the real-time status of all EDI services including:
          </Typography>
          <Box component="ul" sx={{ textAlign: 'left', mt: 2, mb: 0 }}>
            <li>EDI Gateway API</li>
            <li>EDI Monitor API</li>
            <li>Database Connections</li>
            <li>SignalR Hub Status</li>
            <li>Message Queue Status</li>
          </Box>
        </Alert>
      </Box>
    </Paper>
  );
}

export default ServicesStatus;
