import { useState } from 'react'
import './App.css'
import settingsIcon from './assets/settings-icon.svg'
import { askQuestion, type AskResponse } from './services/apiService'

function NavBar(){
  return (
    <div className='bg-cyan-600 dark:bg-blue-950 h-12 p-3 shadow-lg'>
      Spiritual Guide
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
    <form method='post' onSubmit={handleSubmit} className="flex flex-row h-full shadow-lg rounded-xl">
      <input 
        name="userInput"
        className="flex-1 p-2 rounded-l" 
        placeholder="Ask anything..." 
        defaultValue=""
      />
      <button type='submit' className="bg-cyan-600 dark:bg-blue-950 hover:bg-sky-800 active:bg-indigo-400 px-4 rounded-r shadow-lg">
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
    <div className="p-4 shadow-lg relative w-full h-full">
      {answer ? (
        <p>{answer}</p>
      ) : (
        <img
          src="/gifs/zen-meditation.gif"
          alt="meditating zen master"
          className="absolute inset-0 w-full h-full object-fill"
        />
      )}
    </div>
  );
}



function Main() {
  const [answer, setAnswer] = useState('');

  async function handleAsk(input: string) {
    
    const response: AskResponse = await askQuestion(input);

    setAnswer(`${response.quote} - ${response.author}` || "No answer found");

  }

  return (
    <div className="bg-gray-300 dark:bg-neutral-900 rounded-lg my-14 flex-1 flex flex-col p-6 gap-4 w-[90%] max-w-4xl mx-auto shadow-lg">
      <div className="h-14 bg-white dark:bg-neutral-800">
        <InputField onAsk={handleAsk} />
      </div>

      <div className="h-8 flex justify-start items-center">
        <Settings />
      </div>

      <div className="bg-white dark:bg-neutral-800 rounded-lg flex-1 shadow-lg">
        <AnswerField answer={answer} />
      </div>
    </div>
  );
}


function App() {
  return (
    <div className='dark:bg-neutral-800 dark:text-white flex flex-col h-screen'>
      <NavBar />
      <Main />
    </div>
  )
}

export default App
