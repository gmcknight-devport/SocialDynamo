import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom';
import moment from 'moment'
import Comment from '../comment/Comment';
import RefreshLogin from '../../util/RefreshLogin';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import SendIcon from '@mui/icons-material/Send';
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import "./CommentsModal.css"
import { toast } from "react-toastify";

export default function CommentsModal({postId, commentData}) {
    const [combinedData, setCombinedData] = useState([]);
    const [modal, setModal] = useState(false);
    const [comment, setComment] = useState('');
    const url = JSON.parse(localStorage.getItem('url')) || null;
    const navigate = useNavigate();
    
    //Modal logic
    const toggleModal = () => {
        setModal(!modal)
    }

    useEffect(() => {
        if(modal) {
            document.body.classList.add('comments-active-modal')
        } else {
            document.body.classList.remove('comments-active-modal')
        }
    }, [modal])
    
    //Load initial comments data
    useEffect(async() =>{       
        const fetchData = async () => {
            try{
                //Get user data           
                const userIds = commentData.map((comment) => comment.authorId);
                const finalUserBody = JSON.stringify(userIds); 
                const userResponse = await fetch(url + '/account/Profiles', {
                    method: 'PUT',
                    mode: 'cors',
                    credentials: 'include',
                    body: finalUserBody,
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                //Get likes data 
                const commentIds = commentData.map((comment) => comment.commentId);
                const finalLikesBody = JSON.stringify(commentIds);                           
                const likesResponse = await fetch(url + '/posts/commentslikes', {
                    method: 'PUT',
                    mode: 'cors',
                    credentials: 'include',
                    body: finalLikesBody,
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                if (userResponse.ok && likesResponse.ok){
                    if(!userResponse.headers.get('content-type').includes('application/json')){
                        displayError("Couldn't load comments.");
                        return;
                    }              

                    const userData = await userResponse.json();
                    const likeData = await likesResponse.json();
                    
                    //Map response and update
                    const userId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
                    const combined = commentData.map((item, index) => ({
                        userId: item.authorId,
                        name: userData[index].forename + ' ' + userData[index].surname,
                        profilePicture: userData[index].profilePicture,
                        commentId: item.commentId,
                        comment: item.commentText,
                        postedAt: item.postedAt.slice(0, 16).replace("T", " "),
                        likes: likeData && likeData[index].length > 0 ? likeData[index].length : 0,
                        hasUserLiked: likeData && likeData[index].some(like => like.likeUserId === userId)
                    }));        

                    setCombinedData(combined);

                }else if(userResponse.status === 401){
                    if(!await RefreshLogin(navigate)) return true;
                    fetchData();
                }else if(userResponse.status === 400){
                    displayError(userResponse.title);
                }else{
                    displayError("Unexpected error occurred, please try again.");
                }
            }catch(error){
                displayError("Unexpected error occurred, couldn't fetch comments.");
                console.log(error);
            }
        }
        if(commentData !== undefined || commentData !== null) fetchData();
    }, []);


    const handleInputChange = (e) => {
        setComment(e.target.value);
    };

    const handleKeyPress = (event) => {
        if (event.key === 'Enter') {
          handleSubmit(event);
        }
      };

    //Handle submitting comment
    const handleSubmit = async (event) => {
        event.preventDefault();

        const authorId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
        
        const objMap = { postId, authorId, comment };
        const finalBody = JSON.stringify(objMap);

        try{
            //Send data
            const responseComment = await fetch(url + '/posts/comment', {
                method: 'PUT',
                mode: 'cors',
                credentials: 'include',
                body: finalBody,
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            if(responseComment.ok){
                const responseJson = await responseComment.json();
                await fetchUserCommentData(responseJson);
            }else if(responseComment.status === 401){
                if(!await RefreshLogin(navigate)) return true;
                handleSubmit(event);
            }else if(responseComment.status === 400){
                displayError(responseComment.title);
                return;
            }else{
                displayError("Unexpected error occurred.");
                return;
            }
        }catch(error){
            console.log(error);
            displayError("Unexpected error occurred, please try again.")
        }            
    };

    const fetchUserCommentData = async(commentId) => {
        const userId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
        try{
            //Call to retrieve user data      
            const responseUser = await fetch(url + `/account/Profile/${userId}`, {
                method: 'GET',
                mode: 'cors',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            if(responseUser.ok){
                const data = await responseUser.json();
                const newData = {
                    userId: userId,
                    name:  data.forename + ' ' + data.surname,
                    profilePicture: data.profilePicture,
                    comment: comment,
                    commentId: commentId,
                    postedAt: moment().local().format('yyyy/MM/DD HH:mm'),
                    likes: 0,
                    hasUserLiked: false
                };
                setCombinedData(prevData => [...prevData, newData]);
                setComment('');
            }else if(responseComment.status === 401){
                if(!await RefreshLogin(navigate)) return true;
                handleSubmit(event);
            }else if(responseComment.status === 400){
                displayError(responseComment.title);
            }
        }catch(error){
            console.log(error);
            displayError("Unexpected error occurred, please try again.")
        }
    }

    const handleDeleteComment = async(commentId) =>{
        const objMap = { commentId };
        const finalBody = JSON.stringify(objMap);
        
        try{
          //Fetch data
          const response = await fetch(url + '/posts/deletecomment', {
            method: 'DELETE',
            mode: 'cors',
            credentials: 'include',
            body: finalBody,
            headers: {
                'Content-Type': 'application/json',
            },
          });
          
          //Handle different responses
          if (response.ok) {
            setCombinedData(combinedData.filter(comment => comment.commentId !== commentId));
          } else if (response.status === 401){
            if(!await RefreshLogin(navigate)) return true;
            handleDeleteComment(commentId);
          }else if (response.status === 400){            
            displayError(response.title)
          }else{
            displayError("Unexpected error occurred, please try again.");
          }
        }catch(error){
          displayError("Unexpected error occurred, please try again.");
          console.log(error);
        } 
    }
 
    const displayError = (message) => {
        toast.error(message);
    }

    //Handle comment liked updates
    const updateCommentLiked = (commentId) => {
        setCombinedData(prevData =>
            prevData.map(comment =>
                comment.commentId === commentId
                ? {
                    ...comment,
                    hasUserLiked: !comment.hasUserLiked,
                    likes: comment.hasUserLiked ? comment.likes - 1 : comment.likes + 1
                }
                : comment
            )
        );
    }

  return (
    <>
      <ChatBubbleOutlineIcon onClick={toggleModal}/>

      {modal && (
        <div className="comments-modal">
          <div onClick={toggleModal} className="comments-overlay"></div>
            <div className="comments-modal-content">
                <h1>Comments</h1>                               
                {combinedData.map((data, index) => {
                    return (
                        <Comment key={index} data={data} onDelete={handleDeleteComment} onLike={updateCommentLiked} />                        
                    );
                })}

                <div className='comments-modal-footer'>
                    <input
                        className="comment-input"
                        type="text"
                        value={comment}
                        onChange={handleInputChange}
                        placeholder='Enter a comment'
                        onKeyDown={handleKeyPress}
                    />                    
                    <button className="submit-modal" onClick={handleSubmit}>
                        <SendIcon className='sendIcon'/>
                    </button>
                </div>
                <button className="close-modal" onClick={toggleModal}>
                    <CancelOutlinedIcon className='closeIcon'/>
                </button>
            </div>
        </div>
      )}
    </>
  )
}