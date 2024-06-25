import React, {useState, useEffect, useLayoutEffect} from 'react';
import { useNavigate } from 'react-router-dom';
import './ProfileHeader.css';
import UserModal from '../modals/UserModal';
import EditProfileModal from '../modals/EditProfileModal';
import RefreshLogin from '../../util/RefreshLogin';
import { Avatar } from '@mui/material';
import { toast } from "react-toastify";

export default function ProfileHeader({userId}) {
    const loggedInUserId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
    const isLoggedInUserProfile = userId === loggedInUserId ? true : false;
    const url = JSON.parse(localStorage.getItem('url')) || null;
    const [imageUrl, setImageUrl] = useState('');
    const avatarStyle = {width: "16vh", height: "16vh"};
    const navigate = useNavigate();

    const [userData, setUserData] = useState('');
    const [followers, setFollowers] = useState('');
    const [followerCount, setFollowerCount] = useState(0);    
    const [following, setFollowing] = useState('');
    const [followingCount, setFollowingCount] = useState(0);
    const [isCurrentFollower, setIsCurrentFollower] = useState(false);

    useEffect(async () => {        
        const fetchData = async () => {                               
            await fetchProfileInformation(); 
            await fetchFollowers();
            await fetchFollowing();
        }

        await fetchData();
    }, []);

    //Call to get profile information from API
    const fetchProfileInformation = async () => {
        try{
            //Fetch data
            const response = await fetch(url + `/account/Profile/${userId}`, {
                method: 'GET',
                mode: 'cors',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            
            //Handle different responses
            if (response.ok) {
                const responseJson = await response.json();
                setUserData(responseJson);   
                await fetchPictureFromBlob(responseJson.profilePicture);     
            } else if (response.status === 401){
                if(!await RefreshLogin(navigate)) return true;
                fetchProfileInformation();
            } else {
                displayError(response.title);
            }
        }catch(error){
            displayError("Unexpected error occurred, unable to fetch user profile.");
            console.log(error);
        }
    }

    //Get url for profile picture
    const fetchPictureFromBlob = async (image) => {
        const img = `data:image/jpeg;base64,${image}`;
        setImageUrl(img);
    };
   
    //Call to get user follower information from API
    const fetchFollowers = async () => {
        try{
            //Fetch data
            const response = await fetch(url + `/account/followers/${userId}`, {
                method: 'GET',
                mode: 'cors',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            
            //Handle different responses
            if (response.ok) {
                if(!response.headers.get('content-type').includes('application/json')) return;
          
                const responseJson = await response.json();          
                setFollowers(responseJson);
                setFollowerCount(responseJson.length)

                //Check if logged in user already follows profile
                if(!isLoggedInUserProfile){
                    responseJson.forEach((user, index) =>{
                        if(user.userId === loggedInUserId)
                            setIsCurrentFollower(true);
                    });
                }
            } else if (response.status === 401){
                if(!await RefreshLogin(navigate)) return true;
                fetchFollowers();
            }else {
                displayError(response.title);
            }
        }catch(error){
            displayError("Unexpected error occurred, unable to fetch followers.");
            console.log(error);
        }
    }

    //Call to get user follower information from API
    const fetchFollowing = async () => {
        try{
            //Fetch data
            const response = await fetch(url + `/account/following/${userId}`, {
                method: 'GET',
                mode: 'cors',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            
            //Handle different responses
            if (response.ok) {
                if(!response.headers.get('content-type').includes('application/json')){
                    setFollowingCount(0);
                    return;
                }
          
                const responseJson = await response.json();
                setFollowing(responseJson);
                setFollowingCount(responseJson.length);
            } else if (response.status === 401){
                if(!await RefreshLogin(navigate)) return true;
                fetchFollowing();
            }else {
                displayError(response.title);
            }
        }catch(error){
            displayError("Unexpected error occurred, unable to fetch following.");
            console.log(error);
        }
    }

    const followUser = async () => {
        const followerId = loggedInUserId;
        const objMap = { userId, followerId };
        const finalBody = JSON.stringify(objMap);

        try{
            //Fetch data
            const response = await fetch(url + '/account/addfollower', {
                method: 'PUT',
                mode: 'cors',
                credentials: 'include',
                body: finalBody,
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            
            //Handle different responses
            if (response.ok) {
                setIsCurrentFollower(!isCurrentFollower);

                if (isCurrentFollower) {
                    setFollowerCount(followers.length - 1);
                    setFollowers((prevState) => prevState.filter(follower => follower.userId !== loggedInUserId));
                    return;
                } 

                const newFollower = await fetchNewFollowerInfo();
                setFollowerCount(followers.length + 1);
                setFollowers((prevState) => [...prevState, newFollower]);
                
            } else if (response.status === 401){
                if(!await RefreshLogin(navigate)) return true;
                followUser();
            }else {            
                displayError(response.title)
            }
        }catch(error){
            displayError("Unexpected error occurred, please try again.");
            console.log(error)
        }
    }

    const fetchNewFollowerInfo = async () => {
        try{
            //Fetch data
            const response = await fetch(url + `/account/Profile/${loggedInUserId}`, {
                method: 'GET',
                mode: 'cors',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            
            //Handle different responses
            if(response.ok){
                const responseJson = await response.json(); 
                const newFollower = {
                    profilePicture: responseJson.profilePicture,
                    forename: responseJson.forename,
                    surname: responseJson.surname,
                    userId: loggedInUserId
                };

                return newFollower;
            }else if (response.status === 401){
                if(!await RefreshLogin(navigate)) return true;
                fetchProfileInformation();
            } else{
                console.log(response.title);
            }            
        }catch(error){
            displayError("Unexpected error occurred, unable to fetch user profile.");
            console.log(error);
        }
    }

    const displayError = (message) => {
        toast.error(message);
    }

    return (
        <>    
        <div className='profile-header-container'> 
        {userData &&         
            <div className='profile-header'>                
                <div className='profile-picture'> 
                    {imageUrl && (
                        <Avatar src={imageUrl} sx={avatarStyle}/>
                    )}                
                </div>          
                <div className='follow-followers'>
                    <UserModal
                        title="Followers"
                        data={followers}
                        buttonComponent={<span>{followerCount}<br/>Followers</span>}
                    />
                </div>                    
                <div className='follow-following'>
                    <UserModal
                        title="Following"
                        data={following}                        
                        buttonComponent={<span>{followingCount}<br/>Following</span>}
                    />     
                </div>            
                <div className='follow-button-container'>
                    {!isLoggedInUserProfile && (                            
                        <button className='follow-button' onClick={followUser}> 
                            {!isCurrentFollower ? <span>Follow</span> : <span>Following</span>}
                        </button>                            
                    )}
                </div>  
                <div className='user-details'>                                   
                    <span className='span-name'>{userData.forename + ' ' + userData.surname}</span>
                    <br/>
                    <span className='span-userid'>{userId}</span>
                    <br/>
                </div> 
                <p className='span-description'>{userData.profileDescription}</p>
                <br/>

                {isLoggedInUserProfile && (  
                    <div className='edit-button'>                      
                        <EditProfileModal/>
                    </div>
                )}
            </div>    
            }
        </div>  
        </>
    )
}