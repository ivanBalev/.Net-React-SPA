const toBase64 = (file: File): Promise<string> => {
    return new Promise((resolve, reject) => {
      // fileReader doesn't use promises but simple callbacks
      // so we wrap it into a promise
      const fileReader = new FileReader();
      fileReader.onload = () => resolve(fileReader.result as string);
      fileReader.onerror = () => reject();
      fileReader.readAsDataURL(file);
    });
  };
  
  export default toBase64;
  