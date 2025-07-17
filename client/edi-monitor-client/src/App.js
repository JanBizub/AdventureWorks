import React, { useState, useEffect, useMemo } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import {
  AppBar,
  Toolbar,
  Typography,
  Container,
  Paper,
  Box,
  Chip,
  IconButton,
  Alert,
  Divider
} from '@mui/material';
import {
  Refresh as RefreshIcon,
  CheckCircle as CheckCircleIcon,
  Error as ErrorIcon,
  Schedule as ScheduleIcon,
  Info as InfoIcon
} from '@mui/icons-material';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { AgGridReact } from 'ag-grid-react';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-material.css';
import './AgGridCustom.css';

import { ModuleRegistry, AllCommunityModule } from 'ag-grid-community';

ModuleRegistry.registerModules([AllCommunityModule]);

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
    background: {
      default: '#f5f5f5',
    },
  },
});

function App() {
  const [messages, setMessages] = useState([]);
  const [connection, setConnection] = useState(null);
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    // Create SignalR connection
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7236/ediMonitorHub')
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);

    // Start the connection
    newConnection.start()
      .then(() => {
        console.log('SignalR Connected');
        setIsConnected(true);

        // Listen for new messages
        newConnection.on('MessageReceived', (message) => {
          console.log('New message received:', message);
          setMessages(prevMessages => [message, ...prevMessages.slice(0, 49)]); // Keep only 50 messages
        });

        // Load initial messages
        loadMessages();
      })
      .catch(err => {
        console.error('SignalR Connection Error:', err);
        setIsConnected(false);
      });

    return () => {
      if (newConnection) {
        newConnection.stop();
      }
    };
  }, []);

  const loadMessages = async () => {
    try {
      const response = await axios.get('https://localhost:7236/api/EdiMonitor/messages');
      setMessages(response.data);
    } catch (error) {
      console.error('Error loading messages:', error);
    }
  };

  const formatDateTime = (dateTimeString) => {
    return new Date(dateTimeString).toLocaleString();
  };

  const getStatusColor = (status) => {
    switch (status.toLowerCase()) {
      case 'received': return 'success';
      case 'processing': return 'warning';
      case 'completed': return 'info';
      case 'error': return 'error';
      default: return 'default';
    }
  };

  const getStatusIcon = (status) => {
    switch (status.toLowerCase()) {
      case 'received': return <CheckCircleIcon />;
      case 'processing': return <ScheduleIcon />;
      case 'completed': return <InfoIcon />;
      case 'error': return <ErrorIcon />;
      default: return <InfoIcon />;
    }
  };

  // Status cell renderer for AG Grid
  const StatusCellRenderer = (params) => {
    const status = params.value;
    return (
      <Chip
        icon={getStatusIcon(status)}
        label={status}
        color={getStatusColor(status)}
        variant="filled"
        size="small"
        sx={{ fontSize: '0.75rem' }}
      />
    );
  };

  // Message Type cell renderer for AG Grid
  const MessageTypeCellRenderer = (params) => {
    return (
      <Chip 
        label={params.value} 
        variant="outlined" 
        size="small"
        color="primary"
        sx={{ fontSize: '0.75rem' }}
      />
    );
  };

  // Date formatter for AG Grid
  const DateCellRenderer = (params) => {
    return formatDateTime(params.value);
  };

  // Column definitions for AG Grid
  const columnDefs = useMemo(() => [
    { 
      headerName: 'ID', 
      field: 'id', 
      width: 80,
      cellRenderer: (params) => `#${params.value}`,
      sort: 'desc'
    },
    { 
      headerName: 'Message Type', 
      field: 'messageType', 
      width: 150,
      cellRenderer: MessageTypeCellRenderer
    },
    { 
      headerName: 'Status', 
      field: 'status', 
      width: 130,
      cellRenderer: StatusCellRenderer
    },
    { 
      headerName: 'Received At', 
      field: 'receivedAt', 
      width: 180,
      cellRenderer: DateCellRenderer
    },
    { 
      headerName: 'Length', 
      field: 'length', 
      width: 120,
      cellRenderer: (params) => `${params.value} chars`
    },
    { 
      headerName: 'Source', 
      field: 'sourceIdentifier', 
      width: 200,
      cellRenderer: (params) => params.value || 'N/A'
    }
  ], []);

  // Default column definition
  const defaultColDef = useMemo(() => ({
    sortable: true,
    filter: true,
    resizable: true,
    flex: 1
  }), []);

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Box sx={{ flexGrow: 1, minHeight: '100vh' }}>
        <AppBar position="static" elevation={0}>
          <Toolbar>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
              EDI Message Monitor
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <Typography variant="body2">
                Status:
              </Typography>
              <Chip
                icon={isConnected ? <CheckCircleIcon /> : <ErrorIcon />}
                label={isConnected ? 'Connected' : 'Disconnected'}
                color={isConnected ? 'success' : 'error'}
                variant="filled"
                size="small"
              />
            </Box>
          </Toolbar>
        </AppBar>

        <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
          <Paper elevation={3} sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
              <Typography variant="h4" component="h2">
                Recent EDI Messages ({messages.length})
              </Typography>
              <IconButton 
                onClick={loadMessages} 
                color="primary"
                disabled={!isConnected}
                sx={{ '&:hover': { backgroundColor: 'action.hover' } }}
              >
                <RefreshIcon />
              </IconButton>
            </Box>

            <Divider sx={{ mb: 3 }} />

            {messages.length === 0 ? (
              <Box sx={{ textAlign: 'center', py: 8 }}>
                <Alert severity="info" sx={{ maxWidth: 600, mx: 'auto' }}>
                  <Typography variant="h6" gutterBottom>
                    No EDI messages received yet.
                  </Typography>
                  <Typography variant="body2">
                    Send a message to the EDI Gateway to see it appear here in real-time.
                  </Typography>
                </Alert>
              </Box>
            ) : (
              <Box sx={{ height: 600, width: '100%' }}>
                <div className="ag-theme-material" style={{ height: '100%', width: '100%' }}>
                  <AgGridReact
                    rowData={messages}
                    columnDefs={columnDefs}
                    defaultColDef={defaultColDef}
                    pagination={true}
                    paginationPageSize={20}
                    animateRows={true}
                    enableCellTextSelection={true}
                    suppressRowClickSelection={true}
                    getRowId={(params) => params.data.id}
                  />
                </div>
              </Box>
            )}
          </Paper>
        </Container>
      </Box>
    </ThemeProvider>
  );
}

export default App;
