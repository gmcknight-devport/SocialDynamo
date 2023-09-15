import React, { useState } from 'react';
import UserProfileHeader from '../user-profile-header';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import "./comment.css"

const Comment = ({ data }) => {
    const [liked, setLiked] = useState(data.hasUserLiked);
    const [likes, setLikes] = useState(data.likes);
    let debounceTimeout;

    const handleLikeClick = () => {
        console.log('Before:', likes, liked);
        setLiked(!liked);

        // Update the local state of likes
        const newLikes = liked ? likes - 1 : likes + 1;
        console.log('After', newLikes)
        setLikes(newLikes);
        
        if (debounceTimeout) {
            clearTimeout(debounceTimeout);
        }
    
        debounceTimeout = setTimeout(async () => {            
                        
            const CommentId = data.commentId;
            const LikeUserId = JSON.parse(sessionStorage.getItem('userId'))?.UserId || null;
            const objMap = { CommentId, LikeUserId };
            const finalBody = JSON.stringify(objMap);

            //Send data
            const response = await fetch('http://20.49.168.20:80/posts/likecomment', {
                method: 'PUT',
                body: finalBody,
                headers: {
                    'Content-Type': 'application/json',
                },
            });
        }, 300);
    };

    return (
        <div className='profile-header'>            
            <UserProfileHeader
                profilepictureblob={data.profilePictureBlob}
                name={data.name}
                userid={data.userId}
            />            
        <div className='comment-body'>{data.comment}</div>
        <div className='comment-footer'>
            <div className='footer-left'>
                {data.postedAt} <span style={{ marginLeft: '10px' }}>{likes} likes </span>
            </div>
            <div footer-right>
                <FavoriteBorderIcon 
                onClick={handleLikeClick} 
                style={{ color: liked ? 'red' : 'inherit' }} 
                />
            </div>         
        </div>
        </div>
    );    
}

export default Comment;