import './App.css'
import { AuthenticatedTemplate, MsalProvider, UnauthenticatedTemplate } from '@azure/msal-react';
import { AppRouter } from './router/appRouter'
import Home from './pages/home';
import NavBar from './components/navbar';
import type { PublicClientApplication } from '@azure/msal-browser';
import { BrowserRouter } from 'react-router';
import { useEffect } from 'react';




function App({ instance }: { instance: PublicClientApplication }) {
  useEffect(() => {
    console.log('Pinging API to wake it up...');
    fetch(`${import.meta.env.VITE_API_BASE_URL}/ping`).catch(() => {});
  }, []);
  return (
    <MsalProvider instance={instance}>
      <BrowserRouter>
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
      </BrowserRouter>
    </MsalProvider>
  )
}

export default App
