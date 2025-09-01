import { useState } from 'react';
import AnswerField from '../components/answerField';
import InputField from '../components/inputField';
import { IoMdSettings } from "react-icons/io";
import type { AskResponse } from '../models/responseModels';
import { useApiClient } from '../hooks/useApiClient';
import { Riple } from 'react-loading-indicators';

export default function AskPage() {

  const { apiClient } = useApiClient();
  const [answer, setAnswer] = useState('');
  const [loading, setLoading] = useState(false);

  async function handleAsk(input: string) {
    setLoading(true);
    try {
      const response = await apiClient.post('/ask', { question: input });
      const askResponse = response.data as AskResponse;
      setAnswer(`${askResponse.quote} - ${askResponse.author}`);
    } catch (err) {
      console.error(err);
      setAnswer("Something went wrong while fetching your answer.");
    } finally {
      setLoading(false);
    }
  }
  
  return (
    <div className="bg-gray-300 dark:bg-neutral-900 rounded-lg my-14 flex-1 flex flex-col p-6 gap-4 w-[90%] max-w-4xl mx-auto shadow-lg">
      <div className="h-14 bg-white dark:bg-neutral-800">
        <InputField onAsk={handleAsk} />
      </div>

      <div className="h-8 flex justify-start items-center">
        <IoMdSettings/>
      </div>

      <div className="bg-white dark:bg-neutral-800 rounded-lg flex-1 shadow-lg p-2">
        {loading ? (
          <div className='flex justify-center items-center h-full'>
            <Riple color="#32cd32" size="medium" text="" textColor="" />
          </div>
        ) : <AnswerField answer={answer} />}
        
      </div>

    </div>
  );
}