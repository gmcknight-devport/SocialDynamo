import React from "react";
import "./Post.css";
import UserProfileHeader from "../user-profile-header";
import FavoriteBorderIcon from "@mui/icons-material/FavoriteBorder";
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import TelegramIcon from "@mui/icons-material/Telegram";
import BookmarkBorderIcon from "@mui/icons-material/BookmarkBorder";

//export default function Post({ user, postImage, likes, timestamp }) {
export default function Post({ post }) {
  return (
    <div className="post">
      <UserProfileHeader profilepictureblob={user.profilepicture} name={user.name} userid={user.userid}/>

      <div className="post__image">
        <img src={postImage} alt="Post Image" />
      </div>
      <div className="post__footer">
        <div className="post__footerIcons">
          <div className="post__iconsMain">
            <FavoriteBorderIcon className="postIcon" />
            <ChatBubbleOutlineIcon className="postIcon" />
            <TelegramIcon className="postIcon" />
          </div>
          <div className="post__iconSave">
            <BookmarkBorderIcon className="postIcon" />
          </div>
        </div>
        {post.likes.length} likes
      </div>
    </div>
  );
}