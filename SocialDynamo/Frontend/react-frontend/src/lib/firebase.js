import Firebase from 'firebase/compat/app';
import 'firebase/compat/firestore';
import 'firebase/compat/auth';

const config = {
  apiKey: "AIzaSyBkqNZn_-CsnI5jlGT4LWKn57fLXKNUGF4",
  authDomain: "instagram-5b60f.firebaseapp.com",
  projectId: "instagram-5b60f",
  storageBucket: "instagram-5b60f.appspot.com",
  messagingSenderId: "539550887378",
  appId: "1:539550887378:web:46246e2c41c7edf2732c0b"
};

const firebase = Firebase.initializeApp(config);
const { FieldValue } = Firebase.firestore;

console.log('firebase', firebase);

export { firebase, FieldValue };
