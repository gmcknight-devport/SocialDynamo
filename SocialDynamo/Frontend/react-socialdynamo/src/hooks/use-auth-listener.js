import { useState, useEffect } from 'react';

export default function useAuthListener() {
  const [user, setUser] = useState(JSON.parse(localStorage.getItem('authUser')));

  useEffect(() => {
    const authUser = JSON.parse(localStorage.getItem('authUser'));

    if (authUser) {
      setUser(authUser);
    } else {
      setUser(null);
    }
  }, []);

  return { user };
}