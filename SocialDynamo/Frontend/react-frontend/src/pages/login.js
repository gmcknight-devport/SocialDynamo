import { useState, useEffect } from 'react';
import { Link, useHistory } from 'react-router-dom';
import * as ROUTES from '../constants/routes';

export default function Login() {
  const history = useHistory();

  const [emailAddress, setEmailAddress] = useState('');
  const [password, setPassword] = useState('');

  const [error, setError] = useState('');
  const isInvalid =
    password === '' ||
    emailAddress === '' ||
    !/^[\w.%+-]+@[\w.-]+\.[A-Za-z]{2,}$/i.test(emailAddress);

  const handleLogin = async (event) => {
    event.preventDefault();
    
    const objMap = { emailAddress, password };
    const finalBody = JSON.stringify(objMap);
    
    //Fetch data
    const response = await fetch('http://20.49.168.20:80/authentication/login', {
      method: 'PUT',
      body: finalBody,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    //Handle different responses
    const dataWrapper = await response.json();
    const { statusCode, data } = dataWrapper;

    if (statusCode === 200) {
      const token = dataWrapper.value.token;
      const refreshToken = dataWrapper.value.refreshToken;
     
      // Set the tokens as HTTP-only cookies
      document.cookie = `token=${token}; path=/; Secure; SameSite=Strict`;
      document.cookie = `refreshToken=${refreshToken}; path=/; Secure; SameSite=Strict`;
   
      // Store the user in local storage
      const user = { UserId: dataWrapper.value.userId };
      sessionStorage.setItem('userId', JSON.stringify(user));
      storeUserInfo(user);

      history.push(ROUTES.DASHBOARD);
    } else {
      setEmailAddress('');
      setPassword('');
      setError(dataWrapper.value)
    }
  };

  const storeUserInfo = async (user) =>{
    //Fetch user data using userId
    const response = await fetch(`http://20.49.168.20:80/account/Profile/${user.UserId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${document.cookie.split('; ').find(row => row.startsWith('token=')).split('=')[1]}`
      },
    });
    
    const data = await response.json()
    const userData = { 
      name: data.forename + ' ' + data.surname, 
      profileDescription: data.profileDescription,
      numberFollowers: data.numberOfFollowers,
      profilePicture: data.profilePicture
    };
    const userDataString = JSON.stringify(userData);
    localStorage.setItem('userData', userDataString);
  }

  useEffect(() => {
    document.title = 'Login';
  }, []);
  
  return (
    <div className="container flex mx-auto max-w-screen-md items-center h-screen">
      <div className="flex w-3/5">
        <img src="/images/iphone-with-profile.jpg" alt="iPhone with Instagram app" />
      </div>
      <div className="flex flex-col w-2/5">
        <div className="flex flex-col items-center bg-white p-4 border border-gray-primary mb-4 rounded">
          <h1 className="flex justify-center w-full">
            <img src="/images/logo.png" alt="Instagram" className="mt-2 w-6/12 mb-4" />
          </h1>

          {error && <p className="mb-4 text-xs text-red-primary">{error}</p>}

          <form onSubmit={handleLogin} method="POST">
            <input
              aria-label="Enter your email address"
              type="text"
              placeholder="Email address"
              className="text-sm text-gray-base w-full mr-3 py-5 px-4 h-2 border border-gray-primary rounded mb-2"
              onChange={({ target }) => setEmailAddress(target.value)}
              value={emailAddress}
            />
            <input
              aria-label="Enter your password"
              type="password"
              placeholder="Password"
              className="text-sm text-gray-base w-full mr-3 py-5 px-4 h-2 border border-gray-primary rounded mb-2"
              onChange={({ target }) => setPassword(target.value)}
              value={password}
            />
            <button
              disabled={isInvalid}
              type="submit"
              className={`bg-blue-medium text-white w-full rounded h-8 font-bold
            ${isInvalid && 'opacity-50'}`}
            >
              Login
            </button>
          </form>
        </div>
        <div className="flex justify-center items-center flex-col w-full bg-white p-4 rounded border border-gray-primary">
          <p className="text-sm">
            Don't have an account?{` `}
            <Link to={ROUTES.SIGN_UP} className="font-bold text-blue-medium">
              Sign up
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
