import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom';
import './EditProfileModal.css';
import FileUpload from '../FileUpload';
import RefreshLogin from '../../util/RefreshLogin';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import { toast } from "react-toastify";

export default function EditProfileModal() {
    const [modal, setModal] = useState(false);
    const [error, setError] = useState('');
    const [completeMessage, setCompleteMessage] = useState('');
    const [isSuccess, setIsSuccess] = useState(true);
    const [isPublishing, setIsPublishing] = useState(false);
    const userId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
    const url = JSON.parse(localStorage.getItem('url')) || null;
    const navigate = useNavigate();

    const [forename, setForename] = useState('');
    const [surname, setSurname] = useState('');
    const [description, setDescription] = useState('');
    const [oldPassword, setOldPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [profilePicture, setProfilePicture] = useState(null);
    const fileLimit = 1;

    //Modal logic
    const toggleModal = () => {
        setModal(!modal)
    }

    if(modal) {
        document.body.classList.add('edit-active-modal')
    } else {
        document.body.classList.remove('edit-active-modal')
    }

    //Handle submit
    const handleSumbit = async () => {
        let allNull = true;
        setIsPublishing(true);
        var timer = setTimeout(null);
        clearTimeout(timer)

        //Ensure token is valid
        if(!await RefreshLogin(navigate)) return;

        if(forename !== '' || surname !== ''){
            await handleUserDetailsCall();
            allNull = false;
        }

        if(description !== ''){
            await handleDescriptionCall();
            allNull = false;
        }

        if(oldPassword !== '' && newPassword !== ''){
            await handlePasswordCall();
            allNull = false;
        }

        if(profilePicture && profilePicture[0] !== null){
            await handleProfilePictureCall();
            allNull = false;
        }

        allNull ? setError("No values entered, must enter at least 1 value or upload an image") : null;

        if(isSuccess){
            setError('')
            setIsPublishing(false);
            displaySuccess("Profile edited successfully");            
            toggleModal();
            
            timer = setTimeout(() => {
                window.location.reload();
            }, 2000);
            
            timer();
        } else{
            setIsPublishing(false);
            setError(completeMessage); 
        }
    }

    //Handle update user details
    const handleUserDetailsCall = async () => {

        if(surname === "" && forename === ""){
            setError("Failed to update user details - no new values");
            return;
        }

        const objMap = {userId, forename, surname};
        const finalBody = JSON.stringify(objMap);
        
        try{
            //Fetch data
            const response = await fetch(url + '/account/updateuserdetails', {
                method: 'PUT',
                mode: 'cors',
                credentials: 'include',
                body: finalBody,
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            //Handle different responses
            if (response.status === 200) {
                setForename('');
                setSurname('');
                setIsSuccess(true);
            }else {            
                setIsSuccess(false);
                setCompleteMessage((prevValue) => 
                    [...prevValue, response.title]);
            }
        }catch(error){
            setIsSuccess(false);
            setError("Unexpected error occurred, please try again.")
        }
    }

    //Handle update profile description 
    const handleDescriptionCall = async () => {
        const objMap = { userId, description };
        const finalBody = JSON.stringify(objMap);
        
        try{
            //Fetch data
            const response = await fetch(url + '/account/updateprofiledescription', {
                method: 'PUT',
                mode: 'cors',
                credentials: 'include',
                body: finalBody,
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            //Handle different responses
            if (response.status === 200) {
                setDescription('');
                setIsSuccess(true);
            } else {            
                setIsSuccess(false);
                setCompleteMessage((prevValue) => 
                    [...prevValue, response.title]);
            }
        }catch(error){
            setIsSuccess(false);
            setError("Unexpected error occurred, please try again.")
        }
    }

    //Handle update profile description 
    const handlePasswordCall = async () => {
        const objMap = { userId, oldPassword, newPassword };
        const finalBody = JSON.stringify(objMap);
        
        try{
            //Fetch data
            const response = await fetch(url + '/account/changepassword', {
                method: 'PUT',
                mode: 'cors',
                credentials: 'include',
                body: finalBody,
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            //Handle different responses
            if (response.status === 200) {
                setNewPassword('');
                setOldPassword('');
                setIsSuccess(true);
            } else {            
                setIsSuccess(false);
                setCompleteMessage((prevValue) => 
                    [...prevValue, response.title]);
            }
        }catch(error){
            setIsSuccess(false);
            setError("Unexpected error occurred, please try again.")
        }
    }

    //Handle update profile picture
    const handleProfilePictureCall = async () => {
        try {
            const formData = new FormData();
            formData.append('userId', userId);
            for (const file of profilePicture) {
                formData.append('profilePicture', file);
            }
    
            const response = await fetch(url + '/account/uploadprofilepicture', {
                method: 'PUT',
                mode: 'cors',
                credentials: 'include',
                body: formData,
            });
               
            if (response.status === 200) {
                setIsSuccess(true);
                setProfilePicture(null);                
            } else {
                setIsSuccess(false);
                setCompleteMessage((prevValue) => 
                    [...prevValue, response.title]);
            }
        } catch (error) {
            setIsSuccess(false);
            setError("Unexpected error occurred, please try again.");
        }
    }

    //Handle file change in FileUpload component
    const handleFilesChange = (file) => {  
        console.log("Handling files change");      
        setProfilePicture(() => {
            const picture = [...file];
            
            return picture;
        });
    };

    //Handle cancel post
    const handleCancelPost = () => {
        setForename('');
        setSurname('');
        setDescription('');
        setProfilePicture('');
        setError('');
        toggleModal();
    }

    const displaySuccess = (message) => {
        toast.success(message);
    }

  return (
    <>
        <button className="edit-icon" onClick={toggleModal}>
            <span>Edit</span>
        </button>
        {modal && (
            <div className='edit-modal'>
                <div onClick={toggleModal} className="edit-overlay"></div>
                <div className='edit-modal-body'>
                    <h1>Edit Details</h1>
                    <div className='edit-user-details'>
                        <input
                            type="text"
                            placeholder="Forename"
                            className='edit-user-input'
                            onChange={({ target }) => setForename(target.value)}
                            value={forename}
                        />
                        <input
                            type="text"
                            placeholder="Surname"
                            className="edit-user-input"
                            onChange={({ target }) => setSurname(target.value)}
                            value={surname}
                        />
                        <textarea
                            placeholder="Profile description"
                            className="edit-description-textarea"
                            rows='2'
                            onChange={({ target }) => setDescription(target.value)}
                            value={description}
                        />
                    </div>

                    <div className='edit-modal-password'>
                        <h3>Password</h3>
                        <input
                                type="password"
                                placeholder="Old Password"
                                className='edit-user-input'
                                onChange={({ target }) => setOldPassword(target.value)}
                                value={oldPassword}
                        />
                        <input
                            type="password"
                            placeholder="New Password"
                            className='edit-user-input'
                            onChange={({ target }) => setNewPassword(target.value)}
                            value={newPassword}
                        />
                    </div>
                    <div className="edit-modal-upload">
                        <h3>Profile Picture</h3>
                        <FileUpload fileLimit={fileLimit} onFilesChange={handleFilesChange} setError={setError} />
                    </div>
                    <div className='edit-modal-footer'>
                        {error && <p className="edit-error-message">{error}</p>}
                        <button className="edit-cancel-post" onClick={handleCancelPost} disabled={isPublishing}>
                            Cancel
                        </button>
                        <button className="edit-publish-post" onClick={handleSumbit} disabled={isPublishing}>
                            Confirm
                        </button>                        
                    </div>
                        
                    <button className="edit-close-modal" onClick={toggleModal} disabled={isPublishing}>
                        <CancelOutlinedIcon className='closeIcon'/>
                    </button>
                </div>  
            </div>
        )}
    </>
  )
}