const CreateImageObject = async (imageArray) => { 
    const createImageObject = async (image) => {
      const img = new Image();
  
      // Create a promise that resolves when the image is loaded
      const imageLoaded = new Promise((resolve) => {
        img.onload = () => resolve();
      });
  
      img.src = image;
      // Wait for the image to load
      const hasLoaded = await imageLoaded;

      const imageObject = {
        original: img.src,
        thumbnail: img.src,
        height: img.naturalHeight,
        width: img.naturalWidth
      };

      return imageObject;
    };
  
    const processImages = async () => {        
      if (Array.isArray(imageArray)) 
        return await Promise.all(imageArray.map((img) => { return createImageObject(img); }));
      else return await createImageObject(imageArray);
    };
      
    return await processImages();
  };
  
  export default CreateImageObject;