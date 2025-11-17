import Box from '@mui/material/Box'
import Sidebar from './Sidebar'
import { Routes, Route } from 'react-router-dom'
import HomePage from './pages/HomePage'
import PurchasePage from './pages/PurchasePage'

export default function App() {
  return (
    <Box sx={{ display: 'flex' }}>
      <Sidebar />
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/purchase" element={<PurchasePage />} />
       </Routes>
      </Box>
    </Box>
  )
}
