import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import CookieConsent from "react-cookie-consent";
import * as ROUTES from '../constants/Routes';
import './Login.css';

export default function Login() {
  const navigate = useNavigate();

  const [emailAddress, setEmailAddress] = useState('');
  const [password, setPassword] = useState('');
  const url = 'https://api.socdyn.com:443';

  const [error, setError] = useState('');
  const isInvalid =
    password === '' ||
    emailAddress === '' ||
    !/^[\w.%+-]+@[\w.-]+\.[A-Za-z]{2,}$/i.test(emailAddress);

  const handleLogin = async (event) => {
    event.preventDefault();
    
    const objMap = { emailAddress, password };
    const finalBody = JSON.stringify(objMap);
    
    try{
      //Fetch data
      const response = await fetch(url + '/authentication/login', {
        method: 'PUT',
        credentials: 'include',
        mode: 'cors',
        body: finalBody,
        headers: {
          'Content-Type': 'application/json',
        },
      });

      const responseJson = await response.json();

      if(response.ok){
        const user = { userId: responseJson.value.userId };
        localStorage.setItem('userId', JSON.stringify(user));
        localStorage.setItem('url', JSON.stringify(url));
        localStorage.removeItem("RefreshFailed");

        navigate(ROUTES.DASHBOARD);
      }else if(response.status === 400){
        setEmailAddress('');
        setPassword('');
        setError(responseJson.value);        
      }
    }catch(error){
      setEmailAddress('');
      setPassword('');
      setError("Unexpected error occurred, please try again.");
      console.log(error);
    }
  };

  useEffect(() => {
    document.title = 'Login - Social Dynamo';
    if(JSON.parse(localStorage.getItem("RefreshFailed"))) setError("Couldn't refresh token, please login again.");
  }, []);
  
  return (
    <div className="login-container">
      <CookieConsent
        location="bottom"
        cookieName="myAppCookieConsent"
        style={{ backgroundColor: "black", border: "1px solid white", borderRadius:"5px", width: "80vw", justifyContent: "center", margin: "auto", marginLeft: "10vw" }}
        expires={150}
      >
        This website uses cookies for authentication only. Refusing them may cause errors.
      </CookieConsent>

      <div className="login-image-container">
        <img src="/images/login-image.jpg" alt="social media image" />
      </div>
      <div className="login-form-container">
        <div className="login-form-content">
          <h1 className="login-logo-container">
            <img src="/images/social-dynamo-logo.png" alt="Social Dynamo" className="signup-logo" />
          </h1>

          {error && <p className="login-message">{error}</p>}

          <form onSubmit={handleLogin} method="POST">
            <input
              aria-label="Enter your email address"
              type="text"
              placeholder="Email address"
              className="login-input"
              onChange={({ target }) => setEmailAddress(target.value)}
              value={emailAddress}
            />
            <input
              aria-label="Enter your password"
              type="password"
              placeholder="Password"
              className="login-input"
              onChange={({ target }) => setPassword(target.value)}
              value={password}
            />
            <button
              disabled={isInvalid}
              type="submit"
              className={`login-input-button
            ${isInvalid && 'login-button-disabled'}`}
            >
              Login
            </button>
          </form>
        </div>
        <div className="login-signup-container">
          <p className="login-signup-text">
            Don't have an account?{` `}
            <Link to={ROUTES.SIGN_UP} className="login-signup-link">
              Sign up
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
