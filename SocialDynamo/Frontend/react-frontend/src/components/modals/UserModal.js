import React, {useState} from 'react'
import UserHeader from '../UserHeader';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import "./UserModal.css"

export default function UserModal({title, data, buttonComponent: buttonComponent}) {

    const [modal, setModal] = useState(false);
    
    const toggleModal = () => {
        setModal(!modal)
    }

    if(modal) {
        document.body.classList.add('user-active-modal')
    } else {
        document.body.classList.remove('user-active-modal')
    }

  return (
    <>
      <div onClick={toggleModal}>{buttonComponent}</div>
      {modal && (
        <div className="user-modal">
          <div onClick={toggleModal} className="user-overlay"></div>
            <div className="user-modal-content">
                <h1>{title}</h1>
                {/* Map data array to UserProfileHeader components */}
                {data && data.map((userData, index) => {
                    return (
                        <div key={index}>
                            <UserHeader                            
                                profilePicture={userData.profilePicture}
                                name={userData.forename + ' ' + userData.surname}
                                userId={userData.userId}
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