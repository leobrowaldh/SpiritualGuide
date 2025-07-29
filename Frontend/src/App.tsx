import './App.css'

import { AppRouter } from './router/appRouter'

function NavBar(){
  return (
    <div className='bg-cyan-600 dark:bg-blue-950 h-12 p-3 shadow-lg'>
      Spiritual Guide
    </div>
  )
}


function App() {
  return (
    <div className='dark:bg-neutral-800 dark:text-white flex flex-col h-screen'>
      <NavBar />
      <main>
        <AppRouter />
      </main>
    </div>
  )
}

export default App
