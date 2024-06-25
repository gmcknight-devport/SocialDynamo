import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useMediaQuery, Drawer, IconButton, List, ListItemButton, ListItemIcon, ListItemText, Box } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import HomeIcon from '@mui/icons-material/Home';
import SearchIcon from '@mui/icons-material/Search';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import LogoutIcon from '@mui/icons-material/Logout';
import { toast } from "react-toastify";
import CreatePostModal from '../modals/CreatePostModal';
import * as ROUTES from '../../constants/Routes';
import SocialDynamoLogo from './social-dynamo-logo.png';
import './SideNav.css';

function SideNav() {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const isMobile = useMediaQuery('(max-width: 600px)');
  const userId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
  const url = JSON.parse(localStorage.getItem('url')) || null;
  const navigate = useNavigate();

  const handleLogout = async () => {
    const objMap = { userId };
    const finalBody = JSON.stringify(objMap);

    try {
      const response = await fetch(url + '/authentication/logout', {
        method: 'PUT',
        credentials: 'include',
        mode: 'cors',
        body: finalBody,
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (response.status === 200) {
        localStorage.clear();
        navigate(ROUTES.LOGIN);
      } else {
        displayError("Unexpected error occurred, unable to logout.");
      }
    } catch (error) {
      displayError("Unexpected error occurred, please try again.");
      console.log(error);
    }
  };

  const displayError = (message) => {
    toast.error(message);
  };

  const toggleDrawer = (open) => (event) => {
    if (event.type === 'keydown' && (event.key === 'Tab' || event.key === 'Shift')) {
      return;
    }
    setDrawerOpen(open);
  };

  const stopPropagation = (event) => {
    event.stopPropagation();
  };

  const handleProfileClick = (event) => {
    const profilePath = `/p/${userId}`;
    const currentUrl = window.location.href;
    const profileUrlRegex = /^https:\/\/socdyn.com\/p\//;

    if (profileUrlRegex.test(currentUrl)) {
      event.preventDefault();
      navigate(profilePath);
      window.location.reload();
    } else {
      navigate(profilePath);
    }
  };

  const menuItems = (
    <>
      <Link to={`${ROUTES.DASHBOARD}`} style={{ textDecoration: 'none' }}>
        <ListItemButton>
          <ListItemIcon style={{ color: 'white' }}><HomeIcon /></ListItemIcon>
          <ListItemText primary="Home" style={{ color: 'white' }} />
        </ListItemButton>
      </Link>

      <Link to={`${ROUTES.SEARCH}`} style={{ textDecoration: 'none' }}>
        <ListItemButton>
          <ListItemIcon style={{ color: 'white' }}><SearchIcon /></ListItemIcon>
          <ListItemText primary="Search" style={{ color: 'white' }} />
        </ListItemButton>
      </Link>

      <ListItemButton onClick={handleProfileClick} style={{ textDecoration: 'none' }}>
        <ListItemIcon style={{ color: 'white' }}><AccountCircleIcon /></ListItemIcon>
        <ListItemText primary="Profile" style={{ color: 'white' }} />
      </ListItemButton>

      <div onClick={stopPropagation}>
        <CreatePostModal />
      </div>

      <ListItemButton onClick={handleLogout}>
        <ListItemIcon style={{ color: 'white' }}><LogoutIcon /></ListItemIcon>
        <ListItemText primary="Logout" style={{ color: 'white' }} />
      </ListItemButton>
    </>
  );

  return (
    <div className='sidenav'>
      {isMobile ? (
        <>
          <IconButton 
            onClick={toggleDrawer(true)} 
            style={{ position: 'fixed', top: 10, left: 10, color: 'white', zIndex: 3 }}
          >
            <MenuIcon />
          </IconButton>
          <Drawer 
            anchor='left' 
            open={drawerOpen} 
            onClose={toggleDrawer(false)}
            PaperProps={{ style: { backgroundColor: 'black', color: 'white' } }}
          >
            <Box 
              sx={{ width: 250 }}
              role="presentation"
              onClick={toggleDrawer(false)}
              onKeyDown={toggleDrawer(false)}
            >
              <List onClick={stopPropagation}>
                {menuItems}
              </List>
            </Box>
          </Drawer>
        </>
      ) : (
        <>
          <Link to={`${ROUTES.DASHBOARD}`} style={{ textDecoration: 'none' }}>
            <img className='sidenav-logo' src={SocialDynamoLogo} alt='Image Not Found' />
          </Link>
          <div className="sidenav-buttons">
            {menuItems}
          </div>
        </>
      )}
    </div>
  );
}

export default SideNav;
