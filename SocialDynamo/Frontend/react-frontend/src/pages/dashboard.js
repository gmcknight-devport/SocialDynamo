// import React, { useState, useEffect, useLayoutEffect, useRef } from 'react';
// import LoadingSpinner from '../components/loader';

// const PAGE_NUMBER = 1;
// const MaxSeed = 100;
// var currPage = 1;

// const TestData = () => {
//   const [data, setData] = useState([]);
//   const [containerHeight, setContainerHeight] = useState(0);
//   const containerRef = useRef();
//   const pageSize = 10; // Number of data points to load per page
//   const maxIterations = 20; // Maximum number of iterations to prevent infinite loop
//   const originalWindowHeight = window.innerHeight;
//   const [page, setPage] = useState(PAGE_NUMBER);
//   const [loading, setLoading] = useState(false);
//   const [resultsEnd, setResultsEnd] = useState(false);

//   // Simulated function to generate test data
//   const generateTestData = (numberOfPoints) => {
//     const testData = [];
//     for (let i = 1; i <= numberOfPoints; i++) {
//       testData.push({ id: i, name: `Data Point ${i}` });
//     }
    
//     return testData;
//   };

//   useLayoutEffect(() => {
//     // Measure the container height and update the state
//     setTimeout(() => {
//       if (containerRef.current) {
//         setContainerHeight(containerRef.current.clientHeight);
//       }
//     }, 0);
//   }, []);

//   useLayoutEffect(() => {
//     let iterations = 0;

//     const initialDataIteration = () => {
//       if (containerRef.current.clientHeight > originalWindowHeight) return;
      
//       addDataToPage();

//       console.log(`Iteration: ${iterations}`);
//       iterations++;
//       requestAnimationFrame(initialDataIteration); // Continue to the next iteration
//     };

//     requestAnimationFrame(initialDataIteration); // Start the loop
//   }, [containerHeight]);

//   //Function to add data
//   const addDataToPage = () => {
//     let testData = [];
//     console.log("In add data method");
//     ++currPage;
//     let startIndex = (currPage - 1) * 10
//     console.log(`Start index: ${startIndex}, maxSeed: ${MaxSeed}`);
    
//     for (let i = startIndex; i < startIndex + pageSize; i++) {        
//       testData.push({ id: i, name: `Data Point ${i}` });
//     }

//     currPage++;    
//     setData((prevData) => [...prevData, ...testData]);
//     setLoading(false);
//   }

// const handleScroll = () => {
//   const isScrollingToBottom =
//     window.innerHeight + document.documentElement.scrollTop + 1 >=
//     document.documentElement.scrollHeight

//   if (isScrollingToBottom && !loading) {
//     setLoading(true);

//     const newData = generateTestData(pageSize);

//     setData(prevData => [...prevData, ...newData]);
//     setPage(prevPage => prevPage + 1);

//     setTimeout(() => {
//       setLoading(false);
//     }, 1500);
//   }
// };

// useLayoutEffect(() => {
//   window.addEventListener('scroll', handleScroll);
//   return () => window.removeEventListener('scroll', handleScroll);
// }, []);

//   return (
//     <div ref={containerRef}>
//       <ul>
//         {data.map((item) => (
//           <li key={item.id}>{item.name}</li>
//         ))}
//       </ul>
//       {loading && <LoadingSpinner />}
//       {resultsEnd && "No More Posts"}
//     </div>
//   );
// };

// export default TestData;

//Implementation with api call!
import React, { useState, useLayoutEffect, useRef, useContext } from 'react';
import LoadingSpinner from '../components/loader';

const PAGE_NUMBER = 1;
const MaxSeed = 100;
var currPage = 1;

const TestData = () => {
  const [data, setData] = useState([]);
  const [containerHeight, setContainerHeight] = useState(0);
  const containerRef = useRef();
  const originalWindowHeight = window.innerHeight;
  const [page, setPage] = useState(PAGE_NUMBER);
  const [loading, setLoading] = useState(false);
  const [resultsEnd, setResultsEnd] = useState(false);

  const getPosts = async () => {
    
    const { user } = useContext(LoggedInUserContext);
    const userId = user.userId;

    //
    // Need 2 get requests. One for post and one for the persons information??
    // Use userId from first post to call 2nd api method
    //

    //Fetch data
    const postResponse = await fetch(`http://20.49.168.20:80/baseaggregate/feed/${userId}/${currPage}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    //Handle different responses
    const dataWrapper = await postResponse.json();
    const { statusCode, data } = dataWrapper;

    if (statusCode === 200) {
      
      //API call to get header information
      //Add mapped result to page
      //increase current page number

    } else {
      setResultsEnd(true);
    }

    currPage++
  };

  useLayoutEffect(() => {
    // Measure the container height and update the state
    setTimeout(() => {
      if (containerRef.current) {
        setContainerHeight(containerRef.current.clientHeight);
      }
    }, 0);
  }, []);

  //Load data until it exceeds length of the page
  useLayoutEffect(() => {
    let iterations = 0;

    const initialDataIteration = () => {
      if (containerRef.current.clientHeight > originalWindowHeight || resultsEnd) return;
      
      getPosts();

      iterations++;
      requestAnimationFrame(initialDataIteration); // Continue to the next iteration
    };

    requestAnimationFrame(initialDataIteration); // Start the loop
  }, [containerHeight]);

//Infinite scrolling logic
const handleScroll = () => {
  const isScrollingToBottom =
    window.innerHeight + document.documentElement.scrollTop + 1 >=
    document.documentElement.scrollHeight

  if (isScrollingToBottom && !loading && !resultsEnd) {
    setLoading(true);

    const newData = getPosts();

    setData(prevData => [...prevData, ...newData]);

    setTimeout(() => {
      setLoading(false);
    }, 1500);
  }
};

useLayoutEffect(() => {
  window.addEventListener('scroll', handleScroll);
  return () => window.removeEventListener('scroll', handleScroll);
}, []);

  return (
    <div ref={containerRef}>
      <ul>
        {data.map((item) => (
          <li key={item.id}>{item.name}</li>
        ))}
      </ul>
      {loading && <LoadingSpinner />}
      {resultsEnd && "No More Posts"}
    </div>
  );
};

export default TestData;