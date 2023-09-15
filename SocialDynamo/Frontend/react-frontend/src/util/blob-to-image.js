import React from 'react';

const BlobToImage = (props) => {

  // Check if blobData is null or undefined
  if (props === null || props === undefined) {
    return null;
  }

  const { blobData } = props;

  if (Array.isArray(blobData)) {
      // Handle a collection of binary blobs
      return (
        <div>
          {blobData.map((blob, index) => (
            <div key={index}>
              <img src={URL.createObjectURL(blob)} alt={`Image ${index}`} />
            </div>
          ))}
        </div>
      );
    } else {
      // Handle a single binary blob
      const imageUrl = URL.createObjectURL(blobData);
      return <img src={imageUrl} alt="Image couldn't be found" />;
    }
  };

export default BlobToImage;