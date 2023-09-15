import React from 'react';
import { Link } from 'react-router-dom'; 
import { Avatar } from "@mui/material";
import BlobToImage from '../util/blob-to-image';
import "./user-profile-header.css";

export default function UserProfileHeader({profilepictureblob, name, userid}) {

    const imageUrl = BlobToImage(profilepictureblob);

    return (
        <div className="header">
            <Link to={`/p/${userid}`} style={{ textDecoration: 'none' }}>
                <div className="header-user">
                    {imageUrl ? (
                        <Avatar style={{ marginRight: '10px' }} src={imageUrl} /> // Use imageUrl as Avatar src
                    ) : (
                        <Avatar style={{ marginRight: '10px' }}>
                            {name.charAt(0).toUpperCase()}
                        </Avatar>
                    )}
                    {name}
                </div>
            </Link>
        </div>
    )
}