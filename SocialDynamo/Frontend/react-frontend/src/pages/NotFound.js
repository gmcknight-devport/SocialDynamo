import { useEffect } from 'react';
import SideNav from '../components/sidebar/SideNav';
import './NotFound.css';

export default function NotFound() {
  useEffect(() => {
    document.title = 'Not Found - Social Dynamo';
  }, []);

  return (
    <div className="not-found-container">
      <SideNav/>
      <div className="not-found">
        <p className="text">Not Found!</p>
      </div>
    </div>
  );
}
