import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom'; 
import { Avatar } from "@mui/material";
import "./UserHeader.css";

export default function UserHeader({profilePicture, name, userId}) {
  const [imageSrc, setImageSrc] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchData = async () => {
      const img = `data:image/jpeg;base64,${profilePicture}`;
      setImageSrc(img);   
    };

    fetchData();
  }, [profilePicture]);

  const redirectToProfile = async (event) => {
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
  }

  return (
    <div className="header">
      <div className='header-link' onClick={redirectToProfile} style={{ textDecoration: 'none', cursor: 'pointer' }}>
        <div className="header-user">        
          <Avatar style={{ marginRight: '10px' }} src={imageSrc} alt={name.charAt(0).toUpperCase()} />          
          {name}
        </div>
      </div>
    </div>
  );
}