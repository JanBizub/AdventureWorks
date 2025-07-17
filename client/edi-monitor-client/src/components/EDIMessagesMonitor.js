import React, { useState, useEffect, useMemo } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import {
    Typography,
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
import { AgGridReact } from 'ag-grid-react';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-material.css';
import '../AgGridCustom.css';

function EDIMessagesMonitor() {
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
            sort: 'desc',
            flex: 0
        },
        {
            headerName: 'Message Type',
            field: 'messageType',
            cellRenderer: MessageTypeCellRenderer,
            flex: 1,
            minWidth: 150
        },
        {
            headerName: 'Status',
            field: 'status',
            cellRenderer: StatusCellRenderer,
            flex: 1,
            minWidth: 130
        },
        {
            headerName: 'Received At',
            field: 'receivedAt',
            cellRenderer: DateCellRenderer,
            flex: 1.5,
            minWidth: 180
        },
        {
            headerName: 'Length',
            field: 'length',
            cellRenderer: (params) => `${params.value} chars`,
            flex: 0.8,
            minWidth: 120
        },
        {
            headerName: 'Source',
            field: 'sourceIdentifier',
            cellRenderer: (params) => params.value || 'N/A',
            flex: 1.2,
            minWidth: 150
        }
    ], []);

    // Default column definition
    const defaultColDef = useMemo(() => ({
        sortable: true,
        filter: true,
        resizable: true
    }), []);

    return (
        <Paper elevation={3} sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Box>
                    <Typography variant="h4" component="h2" gutterBottom>
                        EDI Messages Monitor
                    </Typography>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                        <Typography variant="body1" color="text.secondary">
                            Real-time monitoring of EDI message processing ({messages.length} messages)
                        </Typography>
                        <Chip
                            icon={isConnected ? <CheckCircleIcon /> : <ErrorIcon />}
                            label={isConnected ? 'Connected' : 'Disconnected'}
                            color={isConnected ? 'success' : 'error'}
                            variant="filled"
                            size="small"
                        />
                    </Box>
                </Box>
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
                <Box sx={{ height: 'calc(100vh - 320px)', width: '100%', minHeight: 500 }}>
                    <div className="ag-theme-material" style={{ height: '100%', width: '100%' }}>
                        <AgGridReact
                            rowData={messages}
                            columnDefs={columnDefs}
                            defaultColDef={defaultColDef}
                            pagination={true}
                            paginationPageSize={25}
                            animateRows={true}
                            enableCellTextSelection={true}
                            suppressRowClickSelection={true}
                            getRowId={(params) => params.data.id}
                        />
                    </div>
                </Box>
            )}
        </Paper>
    );
}

export default EDIMessagesMonitor;
