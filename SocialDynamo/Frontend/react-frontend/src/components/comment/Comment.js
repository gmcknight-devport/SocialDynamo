import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import UserHeader from '../UserHeader';
import RefreshLogin from '../../util/RefreshLogin';
import ConfirmDelete from '../ConfirmDelete';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import { toast } from "react-toastify";
import './Comment.css';

const Comment = ({ data, onDelete, onLike }) => {
  const [liked, setLiked] = useState(data.hasUserLiked);
  const [likes, setLikes] = useState(data.likes);
  const loggedInUserId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
  const isLoggedInUserProfile = data.userId === loggedInUserId ? true : false;
  const url = JSON.parse(localStorage.getItem('url')) || null;
  const navigate = useNavigate();
  let debounceTimeout;

  const handleLikeClick = async () => {
    if (debounceTimeout) {
      clearTimeout(debounceTimeout);
    }

    debounceTimeout = setTimeout(async () => {            
        //Call API to handle liking/unliking. Delay accounts for accidental clicks 
        const commentId = data.commentId;
        const likeUserId = loggedInUserId;
        const objMap = { commentId, likeUserId };
        const finalBody = JSON.stringify(objMap);

        try{
          //Send data
          const response = await fetch(url + '/posts/likecomment', {
              method: 'PUT',
              mode: 'cors',
              credentials: 'include',
              body: finalBody,
              headers: {
                  'Content-Type': 'application/json',
              },
          });

          if(response.ok) {
            setLiked(liked => !liked);
            const newLikes = liked ? likes - 1 : likes + 1;
            setLikes(newLikes);

            //Update parent likes state
            await onLike(data.commentId);
          }
          if(response.status === 401){
            if(!await RefreshLogin(navigate)) return true;
            await handleLikeClick();
          }else if(response.status === 400){
            displayError(response.title);
          }
        }catch(error){
          console.log(error);
          displayError("Unexpected error occurred, please try again.")
        }
    }, 300);    
};

const handleDeleteComment = async () => {
  await onDelete(data.commentId);
}

const displayError = (message) => {
  toast.error(message);
}

  return (
    <div>
        <div className='comment-profile-header'>
            <UserHeader
                profilePicture={data.profilePicture}
                name={data.name}
                userid={data.userId}
            />
        </div>
      <div className='comment-body'>{data.comment}</div>
      <div className='comment-footer' >
        <div className='footer-left' style={{display: 'inline-block'}}>
            {likes} likes {data.postedAt}  
        </div>
        <div className='footer-right'>
        <div style={{display: 'inline-block'}}>
            {isLoggedInUserProfile && (  
              <ConfirmDelete handleConfirmDelete={handleDeleteComment}/>
            )}
          </div>
          <div style={{display: 'inline-block'}}>
            <FavoriteBorderIcon 
                onClick={handleLikeClick} 
                style={{ color: 'white', fill: liked ? 'red' : 'white' }}
            />
          </div>
        </div>
      </div>      
    </div>
  );
}

export default Comment;
