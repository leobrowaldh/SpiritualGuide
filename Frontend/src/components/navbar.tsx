import { NavLink } from 'react-router';
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from '@azure/msal-react';
import { loginRequest } from '../auth/authConfig';

export default function NavBar() {
    const { instance } = useMsal();
    console.log('Accounts:', instance.getAllAccounts());

    const handleLoginRedirect = () => {
        instance.loginRedirect( loginRequest )
        .catch((error) => console.log(error));
    };

    const handleLogoutRedirect = () => {
        instance.logoutRedirect().catch((error) => console.log(error));
    };

    return (
        <nav className='flex items-center justify-between py-4 bg-cyan-600 dark:bg-blue-950 h-12 px-4 shadow-lg'>
            <NavLink to="/" className='text-white font-bold text-lg'>
                Spiritual Guide
            </NavLink>

            <div className='flex gap-2'>
                <AuthenticatedTemplate>
                    <button
                        onClick={handleLogoutRedirect}
                        className='bg-yellow-400 hover:bg-yellow-500 text-black font-semibold py-1 px-3 rounded transition-colors duration-200'
                    >
                        Sign out
                    </button>
                </AuthenticatedTemplate>

                <UnauthenticatedTemplate>
                    <button
                        onClick={handleLoginRedirect}
                        className='bg-white hover:bg-gray-100 text-black font-semibold py-1 px-3 rounded transition-colors duration-200'
                    >
                        Sign in
                    </button>
                </UnauthenticatedTemplate>
            </div>
        </nav>
    );
}
