import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../auth/authConfig";


export default function Home() {
  const { instance } = useMsal();
  
  const handleRedirect = () => {
          instance
              .loginRedirect({
                  ...loginRequest,
                  prompt: 'create',
              })
              .catch((error) => console.log(error));
      };

  return (
    <div className="flex flex-col items-center justify-center h-screen bg-gray-100 dark:bg-neutral-900">
      <h1 className="text-4xl font-bold mb-4 text-center p-6">Welcome to Spiritual Guide</h1>
      <p className="text-lg mb-6">Your portal to the self</p>
      <div className="flex space-x-4">
        <button onClick={handleRedirect} className="bg-blue-900 text-white px-4 py-2 rounded hover:bg-blue-600">
          try it out for free
        </button>
        <button className="bg-gray-300 dark:bg-neutral-700 text-black dark:text-white px-4 py-2 rounded hover:bg-gray-400 dark:hover:bg-neutral-600">
          Learn More
        </button>
      </div>
    </div>
  );
}