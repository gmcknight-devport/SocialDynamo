import React from 'react'
import "./Home.css"
import SideNav from './navigation/SideNav'
import Feed from './feed/Feed'

function home() {
  return (
    <div className='Home'>
        <div className="Home__Nav">

        </div>
        <div className="Home__Feed">
          <feed />
        </div>
    </div>
  )
}

export default home