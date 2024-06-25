import React, { useState } from 'react';
import { useDropzone } from 'react-dropzone';
import TaskIcon from '@mui/icons-material/Task';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline';

const FileUpload = ({ fileLimit, onFilesChange, setError }) => {
    const [uploadedFiles, setUploadedFiles] = useState([]);

    const onDrop = (acceptedFiles) => {
        if (uploadedFiles.length < fileLimit) {
            setUploadedFiles((prevFiles) => [...prevFiles, ...acceptedFiles]);
            setError('');
            onFilesChange([...uploadedFiles, ...acceptedFiles]);
        } else {
            setError('File limit reached');
        }
    };

    const handleRemoveFile = (filename) => {
        setUploadedFiles((files) => files.filter((file) => file.name !== filename));
        setError('');
        onFilesChange(uploadedFiles.filter((file) => file.name !== filename));
    };

    const { getRootProps, getInputProps } = useDropzone({
        onDrop,
        accept: 'image/png, image/jpeg',
    });

  return (    
    <div className='modal-upload'>
        <div className="modal-container">
            <div {...getRootProps({ className: 'dropzone' })}>
                <input {...getInputProps()} />
                <p>Drag and drop some files here, or click to select files</p>
                <br />
                <p className="main">Supported files</p>
                <p className="info">JPG, PNG</p>
            </div>
        </div>

        {/* Display the uploaded files */}
        <div className='uploaded-files'>
            {uploadedFiles.map((file, index) => (
                <li className="file-list" key={index}>
                    <TaskIcon />
                    <span>{file.name}</span>
                    <div className='delete-icon'>
                    <DeleteOutlineIcon onClick={() => handleRemoveFile(file.name)} />
                    </div>
                </li>
            ))}        
        </div>
    </div>
  );
};

export default FileUpload;
