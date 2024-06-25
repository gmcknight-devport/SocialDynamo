const BlobToImage = async (props) => {
  const { byteArray } = props;

  // Check if byteArray is null or undefined
  if (byteArray === null || byteArray === undefined) {
    return [];
  }

  const createImageObject = async (blob) => {
    const imageUrl = URL.createObjectURL(blob);

    // Use an Image object to get the natural height
    const img = new Image();

    // Create a promise that resolves when the image is loaded
    const imageLoaded = new Promise((resolve) => {
      img.onload = () => resolve();
    });

    img.src = imageUrl;

    // Wait for the image to load
    await imageLoaded;

    const imageObject = {
      original: imageUrl,
      thumbnail: imageUrl,
      height: img.naturalHeight,
      width: img.naturalWidth
    };

    return imageObject;
  };

  const processImages = async () => {
    if (Array.isArray(byteArray)) {
      const imagePromises = byteArray.map((byteData) => {
        const blob = new Blob([new Uint8Array(byteData)], { type: 'image/jpeg' });
        return createImageObject(blob);
      });

      return await Promise.all(imagePromises);
    } else {
      const blob = new Blob([new Uint8Array(byteArray)], { type: 'image/jpeg' });
      const image = await createImageObject(blob);
      return image;
    }
  };

  return await processImages();
};

export default BlobToImage;