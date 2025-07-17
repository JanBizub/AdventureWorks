import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { ModuleRegistry, AllCommunityModule } from 'ag-grid-community';

// Components
import Layout from './components/Layout';
import Dashboard from './components/Dashboard';
import EDIMessagesMonitor from './components/EDIMessagesMonitor';
import ServicesStatus from './components/ServicesStatus';

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
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Router>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<Dashboard />} />
            <Route path="messages" element={<EDIMessagesMonitor />} />
            <Route path="services" element={<ServicesStatus />} />
          </Route>
        </Routes>
      </Router>
    </ThemeProvider>
  );
}

export default App;
