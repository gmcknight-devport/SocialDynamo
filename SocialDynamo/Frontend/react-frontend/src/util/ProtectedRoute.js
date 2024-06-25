import { Navigate, Outlet } from "react-router-dom";
import * as ROUTES from '../constants/Routes';

const ProtectedRoutes = () => {
  let auth = JSON.parse(localStorage.getItem('userId'));

  return(
    auth ? <Outlet/> : <Navigate to={ROUTES.LOGIN}/>
  )
}

export default ProtectedRoutes;