import React, { useEffect, useState, useRef, useCallback } from 'react'
import { useNavigate, useParams } from 'react-router-dom';
import './Profile.css';
import ProfileHeader from '../components/profile/ProfileHeader';
import LoadingSpinner from '../components/Loader';
import Post from '../components/post/Post';
import SideNav from '../components/sidebar/SideNav';
import RefreshLogin from '../util/RefreshLogin';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import { toast } from "react-toastify";

function profile() {
    const [modal, setModal] = useState(false);

    const {userId} = useParams();
    const [posts, setPosts] = useState([]);
    const [images, setImages] = useState([]);
    const [selectedPostIndex, setSelectedPostIndex] = useState('');

    const [loading, setLoading] = useState(false);
    const [resultsEnd, setResultsEnd] = useState(false);

    const url = JSON.parse(localStorage.getItem('url')) || null;
    const navigate = useNavigate();

    let page = useRef(1);

    //Initialise posts
    useEffect(() => {        
        document.title = 'Profile - Social Dynamo';
        page.current = 1;
          
        const fetchData = async () => {
            await getUserPosts();
        };

        fetchData();
    }, []);

    //Stop scrolling when post modal is open
    useEffect(() => {
        document.body.style.overflow = modal ? 'hidden' :  'unset';
        
        // Cleanup: Reset the style when the component is unmounted
        return () => {
            document.body.style.overflow = 'unset';
        };
    }, [modal]);

    //Modal logic
    const toggleModal = () => {
        setModal(!modal)
    }

    if(modal) {
        document.body.classList.add('active-modal')        
    } else {
        document.body.classList.remove('active-modal')
    }

    //Get user posts from api
    const getUserPosts = async () => {
        if (loading || resultsEnd) return;
        setLoading(true);

        //Ensure token is valid
        if(!await RefreshLogin(navigate)) return;

        try{
            //Fetch data
            const response = await fetch(url + `/baseaggregate/userposts/${userId}/${page.current}`, {
                method: 'GET',
                mode: 'cors',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            
            //Handle different responses
            if (response.ok) {         
                if(!response.headers.get('content-type').includes('application/json')) {
                    setResultsEnd(true);
                    return;
                } 

                const responseJson = await response.json();

                if(responseJson.length === 0){
                    setResultsEnd(true);
                    return;
                }

                page.current += 1;                
                setPosts((prevData) => [...prevData, ...responseJson]);  
                
                for (const post of responseJson) {                
                    setImages((prevImages) => [...prevImages, post.files[0]]);
                }

            }else if(response.status === 400){
                console.log("In 400: " + response);
                setResultsEnd(true);            
            }else {
                displayError(response.title);
                setResultsEnd(true);
            }
        }catch(error){
            displayError("Unexpected error occurred, unable to fetch posts.");
            console.log(error);
        }
        finally {
            setLoading(false);
        }
    };

    //Infinite scrolling logic
    const handleScroll = async () => {
        if (window.innerHeight + document.documentElement.scrollTop >= document.documentElement.scrollHeight) {
            await getUserPosts();
        }
    };
  
    useEffect(() => {
        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, [handleScroll]);

    //Set post data and open modal
    const openPost = (event, index) => {
        const selectedPost = posts[index];

        if (selectedPost) {
            setSelectedPostIndex(index);
            toggleModal();
        } else {
            console.error("Selected post is undefined.");
        }
    };

    const handleDeletePost = useCallback( async (postId) => {
        const objMap = { userId, postId };
        const finalBody = JSON.stringify(objMap);

        try{
          //Fetch data
          const response = await fetch(url + '/posts/deletepost', {
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
            setPosts(posts.filter(post => post.postId !== postId));
            window.location.reload();
          } else if (response.status === 401){
            if(!await RefreshLogin(navigate)) return true;
            await handleDeletePost(postId);
          }else if (response.status === 400){            
            displayError(response.title)
          }else{
            displayError("Unexpected error occurred, please try again.");
          }
        }catch(error){
          displayError("Unexpected error occurred, please try again.");
          console.log(error);
        } 
    });

    const displayError = (message) => {
        toast.error(message);
    }

    return (
        <div className='profile'>
            <SideNav/>
            <div className='head'>
                <ProfileHeader userId={userId}/>
            </div>            
            <div className='loading-profile'>
                {loading && <LoadingSpinner />}
            </div>
            <div className='post-grid'>  
                {images && images.map((data, index) => {
                    return (            
                        <img className="image"    
                            key={index}                            
                            src={data}
                            alt='Image not found'
                            onClick={(event) => openPost(event, index)}             
                        />                     
                    );
                })}                  
            </div>                 
            <div className='profile-end'>                
                {resultsEnd && <span>No More Posts</span>} 
            </div>
            {modal && (
                <div className='profile-modal'>
                    <div onClick={toggleModal} className="profile-modal-overlay"></div>
                    <div className='profile-modal-body'>     
                        {<Post post={posts[selectedPostIndex]} onDelete={handleDeletePost}/>}
                        <button className="profile-close-modal" onClick={toggleModal}>
                            <CancelOutlinedIcon className='closeIcon'/>
                        </button> 
                    </div>      
                </div>
            )}
        </div>
    )
}

export default profile