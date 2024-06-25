import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './CreatePostModal.css';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import { toast } from "react-toastify";
import FileUpload from '../FileUpload';
import RefreshLogin from '../../util/RefreshLogin';
import { ListItemButton, ListItemIcon, ListItemText } from '@mui/material';

export default function CreatePostModal() {  
    const [modal, setModal] = useState(false);
    const [error, setError] = useState('');

    const [uploadedFiles, setUploadedFiles] = useState([]);
    const [caption, setCaption] = useState('');
    const [hashtag, setHashtag] = useState('');
    const fileLimit = 4;
    const [isPublishing, setIsPublishing] = useState(false);
    const url = JSON.parse(localStorage.getItem('url')) || null;
    const navigate = useNavigate();

    //Modal logic
    const toggleModal = () => {
        setModal(!modal)
    }

    if(modal) {
        document.body.classList.add('create-active-modal')
    } else {
        document.body.classList.remove('create-active-modal')
    }
  
    //Handle file change in FileUpload component
    const handleFilesChange = (newFiles) => {
        setUploadedFiles(() => {
            const updatedFiles = [...newFiles];

            return updatedFiles;
        });
    };

    //Handle publish post
    const handlePublishPost = async () => {
        
        if (uploadedFiles.length <= 0 || caption.trim() === '') {
            setError("Must have a caption and files added");
            return;
        }
        //Ensure token is valid
        if(!await RefreshLogin(navigate)) return;
       
        setIsPublishing(true);

        const authorId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
        const formFiles = new FormData();

        formFiles.append('AuthorId', authorId);
        formFiles.append('Caption', caption);
        formFiles.append('Hashtag', hashtag);
        uploadedFiles.forEach((file, index) =>{
            formFiles.append('Files', file);
        });

        try{
            //Fetch data
            const response = await fetch(url + '/baseaggregate/post', {
                method: 'PUT',
                mode: 'cors',
                credentials: 'include',
                body: formFiles,
            });

            //Handle different responses
            if (response.status === 200) {
                setUploadedFiles([]);
                setCaption('');
                setHashtag('');
                setError('');
                toggleModal();

                displaySuccess("Post created!");

                if(window.location.href.startsWith('https://socdyn.com/p/')) window.location.reload();

            }else if (response.status === 400){
                setError(response.title);
            }else if(response.status === 413){
                setError("Files are too large, try compressing them or removing some.");                
            }
            setIsPublishing(false);
        }catch(error){
            setError("Unexpected error occurred, please try again.");
            setIsPublishing(false);
        }
    };
    
    //Handle cancel post
    const handleCancelPost = () => {
        setUploadedFiles([]);
        setCaption('');
        setHashtag('');
        setError('');
        toggleModal();
    };

    const displaySuccess = (message) => {
        toast.success(message);
    }

    //Render
    return( 
        <>
        <ListItemButton onClick={toggleModal}>
            <ListItemIcon style={{ color: 'white' }}><AddCircleOutlineIcon /></ListItemIcon>
            <ListItemText primary="Create" style={{ color: 'white' }} />
        </ListItemButton>
        {modal && (
            <div className='create-modal'>
                <div onClick={toggleModal} className="create-overlay"></div>
                <div className='create-modal-body'>
                    <h1>Create Post</h1>
                    <div className='post-details'>
                        <textarea
                            placeholder="Post caption or description"
                            className='caption-textarea'
                            rows='3'
                            onChange={({ target }) => setCaption(target.value)}
                            value={caption}
                        />
                        <input
                            type="text"
                            placeholder="Hashtag"
                            className="hashtag-input"
                            pattern='/#[a-z0-9]+/'
                            onChange={({ target }) => setHashtag(target.value.replace(/\s/g, ''))}
                            value={hashtag}
                        />
                    </div>
                    <div className="create-modal-upload">
                        <FileUpload fileLimit={fileLimit} onFilesChange={handleFilesChange} setError={setError} />
                    </div>
                    <div className='create-modal-footer'>
                        {error && <p className="error-message">{error}</p>}
                        <button className="cancel-post" onClick={handleCancelPost} disabled={isPublishing}>
                            Cancel
                        </button>
                        <button className="publish-post" onClick={handlePublishPost} disabled={isPublishing}>
                            Post
                        </button>                        
                    </div>
                        
                    <button className="close-modal" onClick={toggleModal} disabled={isPublishing}>
                        <CancelOutlinedIcon className='closeIcon'/>
                    </button>
                </div>                
            </div>
        )}
    </>)   
} 
