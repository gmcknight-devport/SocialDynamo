import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import * as ROUTES from '../constants/Routes';
import './SignUp.css'

export default function SignUp() {
  const navigate = useNavigate();

  const [userId, setUserId] = useState('');
  const [forename, setForename] = useState('');
  const [surname, setSurname] = useState('');
  const [emailAddress, setEmailAddress] = useState('');
  const [password, setPassword] = useState('');

  const url = 'https://api.socdyn.com:443';
  const [message, setMessage] = useState('');
  const isInvalid =
    userId === '' ||
    forename === '' ||
    surname === '' ||
    password === '' ||
    emailAddress === '' ||
    !/^[\w.%+-]+@[\w.-]+\.[A-Za-z]{2,}$/i.test(emailAddress);

  const [shouldRedirect, setShouldRedirect] = useState(false);

  const handleSignUp = async (event) => {
    event.preventDefault();

    const objMap = { emailAddress, userId, password, forename, surname };
    const finalBody = JSON.stringify(objMap);

    try{
      // Fetch data
      const response = await fetch(url + '/authentication/register', {
        method: 'PUT',
        mode: 'cors',
        body: finalBody,
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (response.ok) {
        setMessage('User Created, redirecting for sign in');
        setShouldRedirect(true);
      } else {      
        const responseBody = await response.text();
        console.log(response);
        setMessage(responseBody);
      }
    }catch(error){
      setMessage("Unexpected error occurred, please try again.");
    }
  };

  useEffect(() => {
    document.title = 'Sign Up - Social Dynamo';

    if (shouldRedirect) {
      const redirectTimer = setTimeout(() => {
        navigate(ROUTES.LOGIN);
      }, 3500);

      return () => {
        clearTimeout(redirectTimer);
      };
    }
  }, [shouldRedirect, navigate]);

  return (
    <div className="signup-container">
      <div className="signup-image-container">
        <img src="/images/login-image.jpg" alt="social media image" />
      </div>
      <div className="signup-form-container">
        <div className="signup-form-content">
          <h1 className="signup-logo-container">
            <img src="/images/social-dynamo-logo.png" alt="Social Dynamo" className="signup-logo" />
          </h1>

          {message && <p className="signup-message">{message}</p>}

          <form onSubmit={handleSignUp} method="POST">
            <input
              aria-label="Enter your user ID"
              type="text"
              placeholder="UserID"
              className="signup-input"
              onChange={({ target }) => setUserId(target.value)}
              value={userId}
            />
            <input
              aria-label="Enter your first name"
              type="text"
              placeholder="Forename"
              className="signup-input"
              onChange={({ target }) => setForename(target.value)}
              value={forename}
            />
            <input
              aria-label="Enter your second name"
              type="text"
              placeholder="Surname"
              className="signup-input"
              onChange={({ target }) => setSurname(target.value)}
              value={surname}
            />
            <input
              aria-label="Enter your email address"
              type="text"
              placeholder="Email address"
              className="signup-input"
              onChange={({ target }) => setEmailAddress(target.value)}
              value={emailAddress}
            />
            <input
              aria-label="Enter your password"
              type="password"
              placeholder="Password"
              className="signup-input"
              onChange={({ target }) => setPassword(target.value)}
              value={password}
            />
            <button
              disabled={isInvalid}
              type="submit"
              className={`signup-input-button
            ${isInvalid && 'signup-button-disabled'}`}
            >
              Sign Up
            </button>
          </form>
        </div>
        <div className="signup-login-container">
          <p className="signup-login-text">
            Have an account?{` `}
            <Link to={ROUTES.LOGIN} className="signup-login-link">
              Login
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
