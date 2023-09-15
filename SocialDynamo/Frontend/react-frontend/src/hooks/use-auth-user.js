import { useState, useEffect } from 'react';

export function useAuthListener() {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const userFromLocalStorage = localStorage.getItem('user');
    if (userFromLocalStorage) {
      const parsedUser = JSON.parse(userFromLocalStorage);
      setUser(parsedUser);
    }
  }, []);

  return { user };
}

export default useAuthListener;