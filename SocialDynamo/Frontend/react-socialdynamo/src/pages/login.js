import { useState, useEffect } from 'react';
import { Link, useHistory } from 'react-router-dom';
import * as ROUTES from '../constants/routes';
import HttpRequest from '../util/http-request';

export default function Login() {
  const history = useHistory();

  const [emailAddress, setEmailAddress] = useState('');
  const [password, setPassword] = useState('');

  const [error, setError] = useState('');
  const isInvalid =
    password === '' ||
    emailAddress === '' ||
    !/^[\w.%+-]+@[\w.-]+\.[A-Za-z]{2,}$/i.test(emailAddress);

  const handleLogin = (event) => {
    event.preventDefault();

    HttpRequest({
      path: '/authentication/login',
      requestType: 'PUT',
      body: {
        email: emailAddress,
        password: password,
      },
      onResponse: (status, data) => {
        if (status === 200) {
          history.push(ROUTES.DASHBOARD);
        } else {
          setEmailAddress('');
          setPassword('');
          setError(data.message);
        }
      },
    });
  };

  useEffect(() => {
    document.title = 'Login';
  }, []);

  return (
    
    <div className="cv-container">
      <div className="cv-content">
        <img src="/images/iphone-with-profile.jpg" alt="iPhone with Instagram app" />

        <div className="cv-form">
          <h1>
            <img src="/images/logo.png" alt="Instagram" className="logo-image" />
          </h1>

          {error && <p className="error-message">{error}</p>}

          <form onSubmit={handleLogin} method="POST">
            <input
              aria-label="Enter your email address"
              type="text"
              placeholder="Email address"
              className="input-field"
              onChange={(event) => setEmailAddress(event.target.value)}
              value={emailAddress}
            />
            <input
              aria-label="Enter your password"
              type="password"
              placeholder="Password"
              className="input-field"
              onChange={(event) => setPassword(event.target.value)}
              value={password}
            />
            <button
              disabled={isInvalid}
              type="submit"
              className="login-button"
              style={{ opacity: 0.5 }}
            >
              Login
            </button>
          </form>
        </div>

        <div className="cv-footer">
          <p className="text-sm">
            Don't have an account?{' '}
            <Link to={ROUTES.SIGN_UP} className="font-bold text-blue-medium">
              Sign up
            </Link>
          </p>
        </div>
      </div>
    
    
    </div>
  );
}