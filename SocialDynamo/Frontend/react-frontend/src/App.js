import { lazy, Suspense } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import ReactLoader from './components/Loader';
import * as ROUTES from './constants/Routes';
import ProtectedRoutes from './util/ProtectedRoute';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const Login = lazy(() => import('./pages/Login'));
const SignUp = lazy(() => import('./pages/SignUp'));
const Dashboard = lazy(() => import('./pages/Dashboard'));
const Profile = lazy(() => import('./pages/Profile'));
const NotFound = lazy(() => import('./pages/NotFound'));
const Search = lazy(() => import('./pages/Search'));

export default function App() {
  return (
    <>
      <Router>
        <Suspense fallback={<ReactLoader />}>
          <Routes>            
            <Route element={<ProtectedRoutes/>}>
              <Route element={<Profile/>} path={ROUTES.PROFILE}/>
              <Route element={<Dashboard/>} path={ROUTES.DASHBOARD} exact/>
              <Route element={<Search/>} path={ROUTES.SEARCH}/>
              <Route element={<NotFound/>} path={"*"}/>
            </Route>
            <Route element={<Login/>} path={ROUTES.LOGIN}/>
            <Route element={<SignUp/>} path={ROUTES.SIGN_UP} exact/>
          </Routes>
        </Suspense>
        <ToastContainer limit={1} position='top-center' theme='light' closeToast/>
      </Router>
    </> 
  );
}
