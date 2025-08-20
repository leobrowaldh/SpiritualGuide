import './App.css'
import { AuthenticatedTemplate, UnauthenticatedTemplate } from '@azure/msal-react';
import { AppRouter } from './router/appRouter'
import Home from './pages/home';
import NavBar from './components/navbar';




function App() {
  return (
      <div className='dark:bg-neutral-800 dark:text-white flex flex-col h-screen'>
          <NavBar />
          <main className=''>
            <AuthenticatedTemplate>
              <AppRouter />
            </AuthenticatedTemplate>
            <UnauthenticatedTemplate>
              <Home />
            </UnauthenticatedTemplate>
          </main>
      </div>
  )
}

export default App
