import { useState, useContext, useEffect } from 'react'
import moment from 'moment'
import Comment from './comment';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import SendIcon from '@mui/icons-material/Send';
import "./comments-popup.css"

export default function CommentsPopup({title, postId, commentData}) {
    const [combinedData, setCombinedData] = useState([]);
    const [modal, setModal] = useState(false);
    const [comment, setComment] = useState('');

    //Modal logic
    const toggleModal = () => {
        setModal(!modal)
    }

    if(modal) {
        document.body.classList.add('active-modal')
    } else {
        document.body.classList.remove('active-modal')
    }
    
    //Load initial comments data
    useEffect(async() =>{        

        const userIds = commentData.map((comment) => comment.authorId);
        
        const objMap = { userIds };
        const finalBody = JSON.stringify(objMap);

        //Send data              
        const response = await fetch('http://20.49.168.20:80/account/Profiles', {
            method: 'PUT',
            body: finalBody,
            headers: {
                'Content-Type': 'application/json',
            },
        });

        const data = await response.json();

        //Map response and update
        const userId = JSON.parse(sessionStorage.getItem('userId'))?.UserId || null;
        const combined = commentData.map((item, index) => ({
            userId: item.authorId,
            name: data[index].forename + ' ' + data[index].surname,
            profilePictureBlob: data[index].profilePictureBlob,
            commentId: item.commentId,
            comment: item.comment,
            postedAt: item.postedAt,
            likes: item.likes ? item.likes.length : 0,
            hasUserLiked: item.likes && item.likes.some(like => like.userId === userId)
        }));        
        setCombinedData(combined);
    }, []);


    const handleInputChange = (e) => {
        setComment(e.target.value);
    };

    //Handle submitting comment
    const handleSubmit = async (event) => {
        event.preventDefault();

        const authorId = JSON.parse(sessionStorage.getItem('userId'))?.UserId || null;
        
        const objMap = { postId, authorId, comment };
        const finalBody = JSON.stringify(objMap);

        //Send data
        const response = await fetch('http://20.49.168.20:80/posts/comment', {
            method: 'PUT',
            body: finalBody,
            headers: {
                'Content-Type': 'application/json',
            },
        });

        // Retrieve the data from localStorage        
        const userDataString = localStorage.getItem('userData');
        const userData = JSON.parse(userDataString);
        
        const newData = {
            userId: authorId,
            name:  userData.name,
            profilePictureBlob: userData.profilePictureBlob,
            comment: comment,
            postedAt: moment().local().format('yyyy/MM/DD HH:mm'),
            likes: 0,
            hasUserLiked: false
        };
        setCombinedData(prevData => [...prevData, newData]);

        setComment('');
      };
 
  return (
    <>
      <button onClick={toggleModal} className="btn-modal">
        Open
      </button>

      {modal && (
        <div className="modal">
          <div onClick={toggleModal} className="overlay"></div>
            <div className="modal-content">
                <h1>{title}</h1>                               
                {combinedData.map((data, index) => {
                    return (
                        <Comment key={index} data={data} />
                    );
                })}

                <div className='modal-footer'>
                    <input
                        className="comment-input"
                        type="text"
                        value={comment}
                        onChange={handleInputChange}
                        placeholder='Enter a comment'
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