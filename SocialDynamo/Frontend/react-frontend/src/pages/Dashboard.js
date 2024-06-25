import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import LoadingSpinner from '../components/Loader';
import Post from '../components/post/Post';
import SideNav from '../components/sidebar/SideNav';
import RefreshLogin from '../util/RefreshLogin';
import { toast } from "react-toastify";
import './Dashboard.css';

const Dashboard = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [resultsEnd, setResultsEnd] = useState(false);
  const url = JSON.parse(localStorage.getItem('url')) || null;
  const userId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
  const navigate = useNavigate();
  let page = useRef(1);

  useEffect(() => {
    document.title = 'Dashboard - Social Dynamo';
    page.current = 1;

    const fetchData = async () => {
      await getPosts();
    };

    fetchData();
  }, []);

  const getPosts = async () => {
    if (loading || resultsEnd) return;
    setLoading(true);

    //Ensure token is valid
    if(!await RefreshLogin(navigate)) return;

    try{
      //Fetch data
      const postResponse = await fetch(url + `/baseaggregate/feed/${userId}/${page.current}`, {
        method: 'GET',
        mode: 'cors',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      //Handle different responses
      if (postResponse.ok) {
        if(!postResponse.headers.get('content-type').includes('application/json')){
          setResultsEnd(true);
          return true;
        }

        const responseJson = await postResponse.json();

        if(responseJson.value.length === 0){
          setResultsEnd(true);
          return;
        }

        page.current += 1;
        setData((prevFiles) => [...prevFiles, ...responseJson.value]);

      }else if(postResponse.status === 400){
        displayError(postResponse.title);
        setResultsEnd(true);
        return true;
      } else {
        setResultsEnd(true);
        return true;
      }
    }catch(error){
      setResultsEnd(true);
      console.log(error);
    }
    finally {
      setLoading(false);
    }
  };

//Infinite scrolling logic
const handleScroll = async () => {
  if (window.innerHeight + document.documentElement.scrollTop >= document.documentElement.scrollHeight) {
    await getPosts();
  }
};

useEffect(() => {
  window.addEventListener('scroll', handleScroll);
  return () => window.removeEventListener('scroll', handleScroll);
}, [handleScroll]);

const displayError = (message) => {
  toast.error(message);
}

  return (
    <div className='dash-base'>
      <SideNav/>
      <div className='dash-results' >
        {data && data.map((item, index) => {
          return (
            <Post key={index} post={item} onDelete={null}/>
          );
        })}        
      </div>
      <div className='dash-end'>
        {loading && !resultsEnd && <LoadingSpinner />}
        {resultsEnd && <span>No more posts</span>}
      </div>
    </div>
  );
};

export default Dashboard;