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

type InputFieldProps = {
  onAsk: (input: string) => void;
};

function InputField({ onAsk }: InputFieldProps) {

  function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault(); // prevent page reload
    const formData = new FormData(e.currentTarget);
    const input = formData.get('userInput') as string;
    onAsk(input);
  }

  return (
    <form method='post' onSubmit={handleSubmit} className="flex flex-row h-full">
      <input 
        name="userInput"
        className="flex-1 border p-2 rounded-l" 
        placeholder="Ask anything..." 
        defaultValue=""
      />
      <button type='submit' className="bg-blue-500 text-white px-4 rounded-r">
        Ask
      </button>
    </form>
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

type AnswerFieldProps = {
  answer: string;
};

function AnswerField({ answer }: AnswerFieldProps) {
  return (
    <div className="p-4 h-full">
      {answer || "Answers appear here"}
    </div>
  );
}


function Main() {
  const [answer, setAnswer] = useState('');

  function handleAsk(input: string) {
    // Replace with your real API call
    console.log("User input:", input);
    setAnswer(`You asked: ${input}`);
  }

  return (
    <div className="rounded-lg my-14 flex-1 flex flex-col p-6 gap-4 border w-[90%] max-w-4xl mx-auto">
      <div className="h-14">
        <InputField onAsk={handleAsk} />
      </div>

      <div className="h-8 flex justify-start items-center">
        <Settings />
      </div>

      <div className="rounded-lg flex-1 min-h-[200px] border overflow-auto">
        <AnswerField answer={answer} />
      </div>
    </div>
  );
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
