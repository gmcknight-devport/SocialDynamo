import { useEffect } from 'react';
import PropTypes from 'prop-types';

export default function NotFound() {
  return <p>Not Found</p>
}

// import { useEffect } from 'react';

// export default function NotFound() {
//   useEffect(() => {
//     document.title = 'Not Found - Instagram';
//   }, []);

//   return (
//     <div className="bg-gray-background">
//       <Header />
//       <div className="mx-auto max-w-screen-lg">
//         <p className="text-center text-2xl">Not Found!</p>
//       </div>
//     </div>
//   );
// }