import React, { useEffect, useRef } from 'react';

const PAGE_SIZE = 10; // Number of items per page

const InfiniteScroll = ({ data, loading, error, onLoadMore, renderData }) => {
  const containerRef = useRef();

  const handleScroll = () => {
    const container = containerRef.current;
    if (container) {
      const { scrollTop, scrollHeight, clientHeight } = container;
      if (scrollTop + clientHeight >= scrollHeight - 500 && !loading) {
        onLoadMore();
      }
    }
  };

  useEffect(() => {
    const container = containerRef.current;
    if (container) {
      container.addEventListener('scroll', handleScroll);
      return () => container.removeEventListener('scroll', handleScroll);
    }
  }, [handleScroll]);

  return (
    <div ref={containerRef}>
      {data.map((item) => (
        <div key={item.id} onClick={() => renderData.onClick(item)}>
          {renderData(item)}
        </div>
      ))}
      {loading && <p>Loading...</p>}
      {error && <p>Error fetching data.</p>}
    </div>
  );
};

export default InfiniteScroll;