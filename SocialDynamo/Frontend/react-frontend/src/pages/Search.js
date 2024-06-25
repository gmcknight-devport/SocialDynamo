import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import Post from '../components/post/Post';
import UserHeader from '../components/UserHeader';
import SideNav from '../components/sidebar/SideNav';
import RefreshLogin from '../util/RefreshLogin';
import LoadingSpinner from '../components/Loader';
import "./Search.css"
import SearchIcon from '@mui/icons-material/Search';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';

const Search = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [images, setImages] = useState('');
  const [selectedPostIndex, setSelectedPostIndex] = useState('');
  const [modal, setModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const [hasUserResults, setHasUserResults] = useState(true);
  const [hasPostResults, setHasPostResults] = useState(true);
  const [searchResults, setSearchResults] = useState({ users: [], posts: [] });
  const url = JSON.parse(localStorage.getItem('url')) || null;
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    document.title = 'Search - Social Dynamo';

    const hasInitialSearchTerm = async () => {
      const state = location.state;

      if (state && state.hashtag) {
        await searchApiCall(state.hashtag);
      }
    }
    hasInitialSearchTerm();
  }, []);

  //Modal logic
  const toggleModal = () => {
    setModal(!modal)
  }

  if(modal) {
      document.body.classList.add('search-active-modal')        
  } else {
      document.body.classList.remove('search-active-modal')
  }

  const handleSearch = async () => {   
    await searchApiCall(searchTerm);    
  };

  const searchApiCall = async (term) => {
    setLoading(true);

    try {
      //Send data              
      const response = await fetch(url + `/baseaggregate/search/${term}`, {
        method: 'GET',
        mode: 'cors',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      setSearchTerm('');

      if(response.ok){        
        setImages([]);
        const data = await response.json();

        setSearchResults({
          users: data.users,
          posts: data.posts          
        });
        
        if(data.users.length === 0) setHasUserResults(false);
        else setHasUserResults(true);
        
        if(data.posts.length === 0) {
          setHasPostResults(false);
          setImages([]);
        }
        else {
          setHasPostResults(true);
          for (const post of data.posts) {  
            setImages((prevImages) => [...prevImages, post.files[0]]);
          }
        }

      }else if(response.status === 401){
        if(!await RefreshLogin(navigate)) return true;
        searchApiCall();
      }else{
        setSearchResults({users: [], posts: []});
        setImages([]);
        setHasUserResults(false);
        setHasPostResults(false);
        return;
      }
      
    } catch (error) {
      console.log(error);
    }finally{
      setLoading(false);
    }
  };

  const handleKeyPress = async (event) => {
    if (event.key === 'Enter') {
      await handleSearch();
    }
  };

  //Set post data and open modal
  const openPost = (event, index) => {
    const selectedPost = searchResults.posts[index];
  
    if (selectedPost) {
        setSelectedPostIndex(index);
        toggleModal();
    } else {
        console.error("Selected post is undefined.");
    }
  };

  const handleDeletePost = async(postId) =>{
    const objMap = { postId };
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
      const responseJson = await response.json();

      //Handle different responses
      if (response.ok) {
        setPosts(posts.filter(post => post.postId !== postId));
      } else if (response.status === 401){
        if(!await RefreshLogin(navigate)) return true;
        handleDeletePost();
      }else if (response.status === 400){            
        displayError(responseJson.title)
      }else{
        displayError("Unexpected error occurred, please try again.");
      }
    }catch(error){
      displayError("Unexpected error occurred, please try again.");
    } 
  }

  return (
    <div className='search'>
      <SideNav/>      
      <div className='search-title'>
        <h1 style={{textAlign: 'center'}}>Search</h1>
      </div>
      <div className='search-top'>
        <input
          className='search-bar'
          type="text"
          placeholder="Search for a user or hashtag"
          value={searchTerm}
          onChange={({target}) => setSearchTerm(target.value)}
          onKeyDown={handleKeyPress}
        />
        <SearchIcon className={"search-icon"} onClick={handleSearch}/>
      </div>
    
      <div className='results'>
        <div className='search-loading-spinner'>
          {loading && <LoadingSpinner />}
        </div>
        <div className='user-results'>
          {/* Map data array to UserProfileHeader components */}
          {searchResults.users && searchResults.users.map((userData, index) => {
            return (
              <div key={index}>
                <UserHeader                            
                  profilePicture={userData.profilePicture}
                  name={userData.forename + ' ' + userData.surname}
                  userId={userData.userId}
                />
              </div>
            );
          })}
          {!hasUserResults && <span className='search-user-results'>No users found</span>}
          {searchResults.users.length > 0 && <div className='bottom-border'></div>}
          </div>
          <div className='search-post-grid'>  
            {images && images.map((data, index) => {
              return (            
                <img className="search-image"    
                  key={index}                            
                  src={data}
                  alt='Image not found'
                  onClick={(event) => openPost(event, index)}  
                  onDelete={handleDeletePost}                              
                />                     
              );
            })}
            <div className='search-post-results'>
              {modal && (
                <div className='search-modal'>
                  <div onClick={toggleModal} className="search-modal-overlay"></div>
                  <div className='search-modal-body'>     
                      {<Post post={searchResults.posts[selectedPostIndex]}/>}
                      <button className="search-close-modal" onClick={toggleModal}>
                        <CancelOutlinedIcon className='closeIcon'/>
                      </button> 
                  </div>      
                </div>
              )}
            </div>
          
        </div>
          {!hasPostResults && <span className='search-posts-results'>No posts found</span>}
        </div>
        
    </div>
  );
};

export default Search;