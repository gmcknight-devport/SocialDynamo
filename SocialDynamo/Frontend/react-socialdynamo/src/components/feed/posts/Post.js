import React from 'react'
import "./Post.css"
import PostModel from "./models"

function Post(PostModel) {
  return (
    <div className='Post'>
        <div className='post__header'>
            <div className="post__headerauthor">
                <Avatar>{PostModel.ProfilePicture}</Avatar>

            </div>
            
        </div>
        <div className="post__image">
            <img src=''></img>
        </div>
        <div className="post__footer"></div>
    </div>
  )
}

export default Post