import { useState } from 'react'
import './App.css'
import settingsIcon from './assets/settings-icon.svg'

function NavBar(){
  return (
    <div className='h-12 p-3 border'>
      Navbar
    </div>
  )
}

function InputField() {
  function handleAsk() {
    console.log("Ask button clicked");
  }
  return (
    <div className="flex flex-row h-full">
      <input 
        className="flex-1 border p-2 rounded-l" 
        placeholder="Ask anything..." 
      />
      <button className="bg-blue-500 text-white px-4 rounded-r" onClick={handleAsk}>
        Ask
      </button>
    </div>
  )
}

function Settings(){
  function handleSettingsClick() {
    console.log("Settings clicked");
  }
  return(
    <button>
      <img src={settingsIcon} alt="settings icon" className="w-8 h-8" onClick={handleSettingsClick} />
    </button>
  )
}

function AnswerField() {
  return (
    <div className="p-4 h-full">
      Answers appear here
    </div>
  )
}

function Main() {
  return (
    <div className="rounded-lg my-14 flex-1 flex flex-col p-6 gap-4 border w-[90%] max-w-4xl mx-auto my-4">
      <div className="h-14">
        <InputField />
      </div>

      <div className="h-8 flex justify-start items-center">
        <Settings />
      </div>

      <div className="rounded-lg flex-1 min-h-[200px] border overflow-auto">
        <AnswerField />
      </div>
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
