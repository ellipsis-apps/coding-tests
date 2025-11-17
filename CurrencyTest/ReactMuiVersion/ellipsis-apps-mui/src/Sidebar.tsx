import Drawer from '@mui/material/Drawer'
import List from '@mui/material/List'
import ListItem from '@mui/material/ListItem'
import ListItemButton from '@mui/material/ListItemButton'
import ListItemIcon from '@mui/material/ListItemIcon'
import ListItemText from '@mui/material/ListItemText'
import HomeIcon from '@mui/icons-material/Home'
import ReportIcon from '@mui/icons-material/Assessment'
import { Link } from 'react-router-dom'
import { APP_DRAWER_WIDTH } from "./constants"

// const drawerWidth = 240

export default function Sidebar() {
  return (
    <Drawer
      variant="permanent"
      sx={{
        width: APP_DRAWER_WIDTH,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: { width: APP_DRAWER_WIDTH, boxSizing: 'border-box' },
      }}
    >
      <List>
        <ListItem disablePadding>
          <ListItemButton component={Link} to="/">
            <ListItemIcon><HomeIcon /></ListItemIcon>
            <ListItemText primary="Home" />
          </ListItemButton>
        </ListItem>
        <ListItem disablePadding>
          <ListItemButton component={Link} to="/purchase">
            <ListItemIcon><ReportIcon /></ListItemIcon>
            <ListItemText primary="Purchase" />
          </ListItemButton>
        </ListItem>
      </List>
    </Drawer>
  )
}
