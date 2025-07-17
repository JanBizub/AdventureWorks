import React, { useState, useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import './App.css';

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
      case 'received': return '#28a745';
      case 'processing': return '#ffc107';
      case 'completed': return '#17a2b8';
      case 'error': return '#dc3545';
      default: return '#6c757d';
    }
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>EDI Message Monitor</h1>
        <div className="connection-status">
          Status: 
          <span 
            className={`status-indicator ${isConnected ? 'connected' : 'disconnected'}`}
          >
            {isConnected ? 'Connected' : 'Disconnected'}
          </span>
        </div>
      </header>

      <main className="App-main">
        <div className="messages-container">
          <div className="messages-header">
            <h2>Recent EDI Messages ({messages.length})</h2>
            <button onClick={loadMessages} className="refresh-btn">
              Refresh
            </button>
          </div>

          {messages.length === 0 ? (
            <div className="no-messages">
              <p>No EDI messages received yet.</p>
              <p>Send a message to the EDI Gateway to see it appear here in real-time.</p>
            </div>
          ) : (
            <div className="messages-list">
              {messages.map((message) => (
                <div key={message.id} className="message-item">
                  <div className="message-header">
                    <span className="message-id">#{message.id}</span>
                    <span className="message-type">{message.messageType}</span>
                    <span 
                      className="message-status"
                      style={{ backgroundColor: getStatusColor(message.status) }}
                    >
                      {message.status}
                    </span>
                  </div>
                  <div className="message-details">
                    <div className="message-time">
                      Received: {formatDateTime(message.receivedAt)}
                    </div>
                    <div className="message-length">
                      Length: {message.length} characters
                    </div>
                    {message.sourceIdentifier && (
                      <div className="message-source">
                        Source: {message.sourceIdentifier}
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

export default App;
