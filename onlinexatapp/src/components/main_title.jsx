import React from 'react'
import logo from '../img/dark_logo.png'
import "../styles/maintitle.css"

export default function MainTitle() {
  return (
    <div className='maintitle_container'>
        <span><img src={logo} alt=""/></span>
        <h1>ONLINE CHAT</h1>
    </div>
  )
}
