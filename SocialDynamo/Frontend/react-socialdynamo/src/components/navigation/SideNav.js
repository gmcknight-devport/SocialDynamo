import React from 'react'
import "./SideNav.css"
import HomeIcon from '@mui/icons-material/Home';
import SearchIcon from '@mui/icons-material/Search';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';

function SideNav() {
  return (
    <div className='sidenav'>
      <img className='sidenav__logo' src='./social-dynamo-logo.png'/>
      <div className="sidenav__buttons">
          <button className="sidenav__button">
            <HomeIcon />
            <span>Home</span>
          </button>

          <button className="sidenav__button">
            <SearchIcon />
            <span>Search</span>
          </button>

          <button className="sidenav__button">
            <AccountCircleIcon />
            <span>Profile</span>
          </button>

          <button className="sidenav__button">
            <AddCircleOutlineIcon />
            <span>Create</span>
          </button>
      </div>
  </div>    
  )
}

export default SideNav