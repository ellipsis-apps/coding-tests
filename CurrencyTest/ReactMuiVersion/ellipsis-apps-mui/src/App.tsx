import Box from '@mui/material/Box'
import Sidebar from './Sidebar'
import { Routes, Route } from 'react-router-dom'
import HomePage from './pages/HomePage'
import PurchasePage from './pages/PurchasePage'
import { createTheme, ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';

const theme = createTheme({
    typography: {
        fontFamily: 'Josefin Sans, Arial, sans-serif',
    },
});

export default function App() {
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <Box sx={{ display: 'flex' }}>
                <Sidebar />
                <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
                    <Routes>
                        <Route path="/" element={<HomePage />} />
                        <Route path="/purchase" element={<PurchasePage />} />
                    </Routes>
                </Box>
            </Box>
        </ThemeProvider>
    )
}
