import { useState } from 'react'
import './App.css'

function NavBar(){
  return (
    <div className='h-12 p-3 border border-red-500'>
      Navbar
    </div>
  )
}

function InputField(){
  return (
    <div className='flex flex-row'>
      <input className='flex-1' placeholder='Ask anything...' />
      <button className=''>Ask</button>
    </div>
  )
}

function Settings(){
  return(
    <button>*</button>
  )
}

function AnswerField(){
  return(
    <div className='border'>
      Answers come up here
    </div>
  )
}

function Main(){
  return(
    <div className='flex-1 flex flex-col p-6 gap-4 border border-red-500 justify-center items-center'>
      <InputField />
      <Settings />
      <AnswerField />
    </div>

  )
}

function App() {
  return (
    <div className='h-screen flex flex-col'>
      <NavBar />
      <Main />
    </div>
  )
}

export default App
