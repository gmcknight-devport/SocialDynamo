import React, {useState, useEffect} from "react";
import { useNavigate, Link } from 'react-router-dom'; 
import "./Post.css";
import CommentsModal from "../modals/CommentsModal";
import UserModal from "../modals/UserModal";
import UserHeader from "../UserHeader";
import CreateImageObject from "../../util/CreateImageObject";
import RefreshLogin from "../../util/RefreshLogin";
import "./ImageGallery.css";
import ImageGallery from "react-image-gallery";
import ConfirmDelete from "../ConfirmDelete";
import styled from 'styled-components'
import { toast } from "react-toastify";

import ArrowCircleLeftIcon from '@mui/icons-material/ArrowCircleLeft';
import ArrowCircleRightIcon from '@mui/icons-material/ArrowCircleRight';
import FavoriteBorderIcon from "@mui/icons-material/FavoriteBorder";

export default function Post({ post, onDelete }) {
  const loggedInUserId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
  const isLoggedInUserProfile = post.userId === loggedInUserId ? true : false;
  const url = JSON.parse(localStorage.getItem('url')) || null;
  const profileRegex = new RegExp(`^https://socdyn.com/p/${loggedInUserId}(?:/.*)?$`, 'i');
  const locationBeginsProfileString = profileRegex.test(window.location.href);//Allow delete action only on profile page
  const [userLiked, setUserLiked] = useState(false);  
  const [numberLikes, setNumberLikes] = useState(post.likes.length);
  const [likesData, setLikesData] = useState([]);
  
  const [hasMultipleFiles, setHasMultipleFiles] = useState(false)
  const navigate = useNavigate();

  const maxWidth = window.innerWidth * 0.4;
  const maxHeight = (maxWidth / 3) * 4;
  
  const [images, setImages] = useState([]);
  const [largestImageHeight, setLargestImageHeight] = useState(maxHeight);
  const [ImageContainer, setImageContainer] =  useState(null);
  const [imageContainerStyle, setImageContainerStyle] = useState({});

  let debounceTimeout;

  //Initialise multiple files const & images
  useEffect(async () => {
    setHasMultipleFiles(post.files.length > 1);
    const fetchImages = async () => {
      const result = await CreateImageObject(post.files);
      setImages(result);
    };

    const hasUserLiked = async () => {
      for(const like of post.likes){
        if(like.likeUserId === loggedInUserId){
          setUserLiked(true);
        }
      }
    }

    await fetchImages();
    await hasUserLiked();

  }, [post.files.length]);

  //Separate useEffect to ensure images fetched before calculations
  useEffect(() => {
      let maxAdjustedHeight = 0;

      if (images.length > 0) {
            
          // Calculate the adjusted height for each image based on the original aspect ratio
          const adjustedHeights = images.map(image => {
          const aspectRatio = image.width / (image.height || 1);          
          const adjustedHeight = Math.min(maxHeight, parseInt(maxWidth / aspectRatio));

          return isFinite(adjustedHeight) ? adjustedHeight : 0;
        });
    
        // Filter out NaN or infinite values
        const validHeights = adjustedHeights.filter(height => isFinite(height));
    
        if (validHeights.length > 0) {
          // Find the maximum adjusted height among all valid heights
          maxAdjustedHeight = Math.max(...validHeights);
          setLargestImageHeight(`${maxAdjustedHeight}px`);
        } else {
          setLargestImageHeight(0); // Reset to 0 if there are no valid heights
        }
      } else {
        setLargestImageHeight(0); // Reset to 0 if there are no images
      }

      //Ensure correct styling
      const imgCont = styled.div`    
      .image-gallery-content,
      .image-gallery-image,
      .image-gallery-slides {
        width: 100%;
        aspect-ratio: 4/3;
        max-width: ${({ maxWidth }) => maxWidth}px !important; /* Ensure image takes up full width */
        object-fit: contain;
      }
      .image-gallery-slides {
        border-radius: 5px;
      }
    
      @media (max-width: 600px) {
        .image-gallery-content,
        .image-gallery-image,
        .image-gallery-slides {
          max-width: 100% !important; /* Ensure image takes up full width on mobile */
          width: 100%;
          height: auto;
        }
      }
    `;

      //Styling if no images
      const isMobile = window.innerWidth <= 600;
      const containerStyle = images.length === 0
      ? {
          width: `${maxWidth}px`
        }
      : {
          maxWidth: isMobile ? '100%' : `${maxWidth}px`,
          width: isMobile ? '100%' : `${maxWidth}px`,
          maxHeight: `auto`,
          objectFit: 'contain'
      };
      setImageContainerStyle(containerStyle);      
      setImageContainer(imgCont);

  }, [images]);

  //Handle post like
  const handleLikeClick = async () => {

    
    
    if (debounceTimeout) {
      clearTimeout(debounceTimeout);
    }

    debounceTimeout = setTimeout(async () => {            
      const postId = post.postId;
      const likeUserId = loggedInUserId;
      
      const objMap = { postId, likeUserId };
      const finalBody = JSON.stringify(objMap);

      likePost(finalBody);
    }, 300);
  };

  const likePost = async (finalBody) => {
    try{
      //Send data
      const response = await fetch(url + '/posts/likepost', {
        method: 'PUT',
        mode: 'cors',
        credentials: 'include',
        body: finalBody,
        headers: {
            'Content-Type': 'application/json',
        }
      });

      if(response.ok) {        
        // Update the local state of likes
        setUserLiked(userLiked => !userLiked);
        const newLikes = userLiked ? numberLikes - 1 : numberLikes + 1;
        setNumberLikes(newLikes);
        
        return;
      }
      else if(response.status === 401){
        if(!await RefreshLogin(navigate)) return true;
        handleLikeClick();
      }else{
        displayError("Unexpected error occurred, please try again.");
        console.log(response.title);
      }
    }catch(error){
      displayError("Unexpected error occurred, please try again.");
      console.log(error);
    }
  }

  //Fetch current likes from API
  const fetchPostLikes = async () => {
    const userIds = post.likes.map((like) => like.likeUserId);
    if (userLiked && !userIds.includes(loggedInUserId)) {
      userIds.push(loggedInUserId);
    }

    const finalBody = JSON.stringify(userIds);
    
    //Ensure token is valid
    if(!await RefreshLogin(navigate)) return;
    
    try{
      //Send data           
      const response = await fetch(url + '/account/Profiles', {
        method: 'PUT',
        mode: 'cors',
        credentials: 'include',
        body: finalBody,
        headers: {
          'Content-Type': 'application/json',
        }
      });

      if(response.ok){
        if(!response.headers.get('content-type').includes('application/json')) {
          displayError("Couldn't find likes");
          return;           
        }

        const data = await response.json();

        //Map response and update
        const processedData = data.map((item, index) => ({
          userId: userIds[index],
          forename: item.forename,
          surname: item.surname,
          profilePicture: item.profilePicture
        }));  

        setLikesData(processedData);
      }
    }catch(error){
      displayError("Unexpected error occurred, unable to fetch post likes.");
      console.log(error);
    }
  };

  //Handle delete post
  const DeletePost = async () => {
    await onDelete(post.postId);
  }

  //Custom rendering for left nav icon
  function renderLeftNav(onClick, disabled) {
    return (
      <button
        className='image-gallery-left-nav'
        disabled={disabled}
        onClick={onClick}
        style={{outline: 'none'}}>
        <ArrowCircleLeftIcon className='left-icon' fontSize="large"/>
      </button>
    )
  };

  //Custom render for right nav icon
  function renderRightNav(onClick, disabled) {
    return (
      <button        
        className='image-gallery-right-nav'
        disabled={disabled}
        onClick={onClick}
        style={{outline: 'none'}}>          
        <ArrowCircleRightIcon className='right-icon' fontSize="large"/>        
      </button>
    )
  }
    
  const displayError = (message) => {
    toast.error(message);
  }

  const hashtagLink = async (hashtag) => {
    const searchPath = `/search`;
    const currentUrl = window.location.href;
    const searchUrlRegex = /^https:\/\/socdyn\.com\/search(?:\/|$)/;

    if (searchUrlRegex.test(currentUrl)) {
      navigate(searchPath, {state: { hashtag } });
      window.location.reload();
    } else {
      navigate(searchPath, {state: { hashtag } });
    }
  }

  return (
    <div className="post">
      <div className="post-header">  
        <div className="post-header-left">         
          <UserHeader profilePicture={post.profilePicture} name={post.usersName} userId={post.userId}/> 
        </div>  
        <div className="post-header-right">
          <div className='delete-button-container'>
            {isLoggedInUserProfile && locationBeginsProfileString && (  
              <ConfirmDelete handleConfirmDelete={DeletePost}/>
            )}    
          </div>
        </div>          
      </div>
      <div className="cont">
        {hasMultipleFiles ? (
          <ImageContainer>
            <ImageGallery items={images} 
                renderLeftNav={renderLeftNav}
                renderRightNav={renderRightNav}
                showPlayButton={false}
                showFullscreenButton={false}
                showThumbnails={false}
                autoPlay={false}
                disableKeyDown={true}
                showBullets={true}
                disableThumbnailScroll={true}
                disableThumbnailSwipe={true}
                infinite={false}
                useBrowserFullscreen={false}
                />
            </ImageContainer>
        ) : (
          <img
            className="image"
            src={images.length > 0 ? images[0].original : ''}
            alt={images.length > 0 ? '' : 'Image not found'}
            style={imageContainerStyle}
          />     
        )}
      </div>     
      <div className="post-body">
          <h3 className="post-body-caption">{post.caption}</h3>
          <div className="post-hashtag" onClick={() => hashtagLink(post.hashtag.substring(1))} 
            style={{ textDecoration: 'underline', 
            color: '#005c98',
            cursor: 'pointer'}}>
            {post.hashtag}
          </div>          
      </div>
      <div className="post-footer">
        <div className="post-footer-icons">  
          <div style={{display: "inline-block"}}>    
            <FavoriteBorderIcon 
              onClick={handleLikeClick} 
              style={{ color: 'white', fill: userLiked ? 'red' : 'white' }} 
            />
          </div>
          <div style={{display: "inline-block"}}>
            <CommentsModal postId={post.postId} commentData={post.comments}/> 
          </div>                    
        </div>     
        <div className="post-footer-info">
          <div className="post-footer-left">
            <UserModal
              title="Likes"
              data={likesData}
              buttonComponent={<span style={{ color: '#005c98' }} onClick={fetchPostLikes}>{numberLikes > 0 ? numberLikes : 0} likes</span>}            
            />
          </div>
          <div className="post-footer-right">
            {post.postedAt.slice(0, 16).replace("T", " ")}
          </div>
        </div>
      </div>    
    </div>
  );  
}