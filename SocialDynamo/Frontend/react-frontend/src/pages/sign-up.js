import { useState, useEffect } from 'react';
import { Link, useHistory } from 'react-router-dom';
import * as ROUTES from '../constants/routes';

export default function SignUp() {
  const history = useHistory();

  const [userId, setUserId] = useState('');
  const [forename, setForename] = useState('');
  const [surname, setSurname] = useState('');
  const [emailAddress, setEmailAddress] = useState('');
  const [password, setPassword] = useState('');

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

    // Fetch data
    const response = await fetch('http://20.49.168.20:80/authentication/register', {
      method: 'PUT',
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
      setMessage(responseBody);
    }
  };

  useEffect(() => {
    document.title = 'Sign Up - Instagram';

    if (shouldRedirect) {
      const redirectTimer = setTimeout(() => {
        history.push(ROUTES.LOGIN);
      }, 3500);

      return () => {
        clearTimeout(redirectTimer);
      };
    }
  }, [shouldRedirect, history]);

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

          {message && <p className="mb-4 text-xs text-red-primary">{message}</p>}

          <form onSubmit={handleSignUp} method="POST">
            <input
              aria-label="Enter your user ID"
              type="text"
              placeholder="UserID"
              className="text-sm text-gray-base w-full mr-3 py-5 px-4 h-2 border border-gray-primary rounded mb-2"
              onChange={({ target }) => setUserId(target.value)}
              value={userId}
            />
            <input
              aria-label="Enter your first name"
              type="text"
              placeholder="Forename"
              className="text-sm text-gray-base w-full mr-3 py-5 px-4 h-2 border border-gray-primary rounded mb-2"
              onChange={({ target }) => setForename(target.value)}
              value={forename}
            />
            <input
              aria-label="Enter your second name"
              type="text"
              placeholder="Surname"
              className="text-sm text-gray-base w-full mr-3 py-5 px-4 h-2 border border-gray-primary rounded mb-2"
              onChange={({ target }) => setSurname(target.value)}
              value={surname}
            />
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
              Sign Up
            </button>
          </form>
        </div>
        <div className="flex justify-center items-center flex-col w-full bg-white p-4 rounded border border-gray-primary">
          <p className="text-sm">
            Have an account?{` `}
            <Link to={ROUTES.LOGIN} className="font-bold text-blue-medium">
              Login
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
