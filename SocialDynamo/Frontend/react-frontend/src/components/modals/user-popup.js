import React, {useState} from 'react'
import PropTypes from 'prop-types';
import UserProfileHeader from './user-profile-header';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import "./user-popup.css"

export default function UserPopup({title, data}) {

    const [modal, setModal] = useState(false);

    const toggleModal = () => {
        setModal(!modal)
    }

    if(modal) {
        document.body.classList.add('active-modal')
    } else {
        document.body.classList.remove('active-modal')
    }

  return (
    <>
      <button onClick={toggleModal} className="btn-modal">
        Open
      </button>

      {modal && (
        <div className="modal">
          <div onClick={toggleModal} className="overlay"></div>
            <div className="modal-content">
                <h1>{title}</h1>
                {/* Map data array to UserProfileHeader components */}
                {data.map((userData, index) => {
                    return (
                        <div key={index}>
                            <UserProfileHeader                            
                                profilepictureblob={userData.profilePictureBlob}
                                name={userData.name}
                                userid={userData.userid}
                            />
                        </div>
                    );
                })}
                <button className="close-modal" onClick={toggleModal}>
                    <CancelOutlinedIcon className='closeIcon'/>
                </button>
            </div>
        </div>
      )}      
    </>
  )
}

UserPopup.propTypes = {
    title: PropTypes.string.isRequired,
    data: PropTypes.arrayOf(PropTypes.shape({
        profilePictureBlob: PropTypes.arrayBuffer, // Enforce profilePictureBlob as an ArrayBuffer
        name: PropTypes.string.isRequired, // Enforce that name is a string and is required
        userid: PropTypes.string.isRequired,
    })).isRequired,
};