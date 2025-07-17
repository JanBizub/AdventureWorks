import React, { useState, useEffect } from 'react';
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
  Card,
  CardContent,
  IconButton,
  CircularProgress,
  Alert,
  Grid,
  Divider
} from '@mui/material';
import {
  Refresh as RefreshIcon,
  Circle as CircleIcon,
  CheckCircle as CheckCircleIcon,
  Error as ErrorIcon,
  Schedule as ScheduleIcon,
  Info as InfoIcon
} from '@mui/icons-material';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';

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
      default: return <CircleIcon />;
    }
  };

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
              <Grid container spacing={2}>
                {messages.map((message) => (
                  <Grid item xs={12} key={message.id}>
                    <Card elevation={2} sx={{ '&:hover': { elevation: 4 } }}>
                      <CardContent>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                          <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                            <Typography variant="h6" component="span" color="text.secondary">
                              #{message.id}
                            </Typography>
                            <Chip 
                              label={message.messageType} 
                              variant="outlined" 
                              size="small"
                              color="primary"
                            />
                          </Box>
                          <Chip
                            icon={getStatusIcon(message.status)}
                            label={message.status}
                            color={getStatusColor(message.status)}
                            variant="filled"
                            size="small"
                          />
                        </Box>
                        
                        <Grid container spacing={2}>
                          <Grid item xs={12} sm={4}>
                            <Typography variant="body2" color="text.secondary">
                              Received
                            </Typography>
                            <Typography variant="body1">
                              {formatDateTime(message.receivedAt)}
                            </Typography>
                          </Grid>
                          <Grid item xs={12} sm={4}>
                            <Typography variant="body2" color="text.secondary">
                              Length
                            </Typography>
                            <Typography variant="body1">
                              {message.length} characters
                            </Typography>
                          </Grid>
                          {message.sourceIdentifier && (
                            <Grid item xs={12} sm={4}>
                              <Typography variant="body2" color="text.secondary">
                                Source
                              </Typography>
                              <Typography variant="body1">
                                {message.sourceIdentifier}
                              </Typography>
                            </Grid>
                          )}
                        </Grid>
                      </CardContent>
                    </Card>
                  </Grid>
                ))}
              </Grid>
            )}
          </Paper>
        </Container>
      </Box>
    </ThemeProvider>
  );
}

export default App;
