import { lazy, Suspense, useState, useEffect } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import ReactLoader from './components/loader';
import * as ROUTES from './constants/routes';
import AuthUserContext from './context/auth-user';
import useAuthListener from './hooks/use-auth-user';

import ProtectedRoute from './helpers/protected-route';

const Login = lazy(() => import('./pages/login'));
const SignUp = lazy(() => import('./pages/sign-up'));
const Dashboard = lazy(() => import('./pages/dashboard'));
const Profile = lazy(() => import('./pages/profile'));
const NotFound = lazy(() => import('./pages/not-found'));
const Test = lazy(() => import('./pages/test'));

export default function App() {
  const { user } = useAuthListener();
  const [loading, setLoading] = useState(true);
  const [parsedUser] = useState(null)

  // useEffect(() => {
  //   const userFromLocalStorage = localStorage.getItem('user');
  
  //   if (!userFromLocalStorage === undefined) {      
  //       parsedUser = JSON.parse(userFromLocalStorage);
  //       setUser(parsedUser);
  //   }
  
  //   setLoading(false);
  // }, []);

  // if (loading) {
  //   return <ReactLoader />;
  // }

  // const handleLogin = (user) => {
  //   setUser(user);
  // };

  return (
    <AuthUserContext.Provider value={{ user }}>
      
        <Router>
          <Suspense fallback={<ReactLoader />}>
            <Switch>
              <Route path={ROUTES.LOGIN} component={Login} />
              <Route path={ROUTES.SIGN_UP} component={SignUp} />
              <Route path={ROUTES.PROFILE} component={Profile} />
              <Route path={ROUTES.TEST} component={Test} />
              <ProtectedRoute user={user} path={ROUTES.DASHBOARD} exact>
                <Dashboard />
              </ProtectedRoute>
              <Route component={NotFound} />
            </Switch>
          </Suspense>
        </Router>
      
    </AuthUserContext.Provider>
  );
}